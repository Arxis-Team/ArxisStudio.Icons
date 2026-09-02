# -*- coding: utf-8 -*-
"""Ставит прямые осевые штрихи на полупиксель.

Обводка 1.2 ложится в один пиксель, когда её середина стоит на x.5: тогда
чернила занимают от x до x+1. Стоящая на целом координате она делится между
двумя пикселями поровну — и штрих читается как серая размазня без ядра.
Это и есть «смазанные контуры».

Трогается только то, что можно трогать безопасно:

* горизонтальный отрезок — двигается его y, вертикальный — его x;
* точка, которой путь входит в дугу или кривую, заморожена: сдвинь её — и
  окружность перестанет быть окружностью;
* диагонали не выравниваются вовсе: их пиксельная сетка не спасает, а наклон
  от сдвига поехал бы.

Силуэты (Play, Stop, Pause, точки меню) ставятся на целую координату, контуры
— на полупиксель: об этом говорит атрибут fill самого файла.

    python tools/normalize.py            — посмотреть, что изменится
    python tools/normalize.py --write    — записать
"""
import io
import os
import re
import sys

import svgpath

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ICONS = os.path.join(ROOT, 'icons')

PATH = re.compile(r'(?P<head><path d=")(?P<path>[^"]+)(?P<tail>")')

# У обводки и у заливки правило разное, и это не мелочь.
#
# Обводка центрирована на пути: чтобы чернила заняли пиксель целиком, середина
# штриха обязана стоять на x.5 — тогда они лягут от x до x+1.
#
# Заливка кончается на самом пути: край, стоящий на x.5, делит крайний пиксель
# пополам и даёт по контуру полупрозрачный ореол. Ей нужна целая координата.
def snap(value, filled):
    return round(value) if filled else round(value - 0.5) + 0.5


def frozen(commands):
    """Индексы точек, которые двигать нельзя: концы дуг и кривых."""
    locked = set()
    points = svgpath.points(commands)

    for at, (letter, _) in enumerate(commands):
        if letter not in ('A', 'C', 'S', 'Q', 'T'):
            continue

        # Заморожен и конец кривой, и точка, из которой она вышла.
        for order, (index, _, _) in enumerate(points):
            if index == at:
                locked.add(order)

                if order > 0:
                    locked.add(order - 1)

    return locked


def normalize(path, filled=False):
    """Возвращает путь, поставленный на сетку, и число сдвинутых точек."""
    commands = svgpath.absolute(svgpath.parse(path))
    points = svgpath.points(commands)
    locked = frozen(commands)

    # Куда сдвинуть каждую точку: сначала собираем намерения, потом применяем.
    shift = {}

    for first, second in segments(commands, points):
        if first in locked or second in locked:
            continue

        (_, x0, y0), (_, x1, y1) = points[first], points[second]

        if abs(y1 - y0) < 1e-6 and abs(x1 - x0) > 1e-6:
            # Горизонталь: на сетку встаёт её высота.
            target = snap(y0, filled)
            shift.setdefault(first, {})['y'] = target
            shift.setdefault(second, {})['y'] = target
        elif abs(x1 - x0) < 1e-6 and abs(y1 - y0) > 1e-6:
            target = snap(x0, filled)
            shift.setdefault(first, {})['x'] = target
            shift.setdefault(second, {})['x'] = target

    if not shift:
        return svgpath.render(commands), 0

    moved = apply(commands, points, shift)

    return svgpath.render(commands), moved


def segments(commands, points):
    """Прямые отрезки пути парами точек — включая замыкающий.

    Отрезок, которым Z возвращает путь в начало, командой не записан, но
    нарисован он так же, как остальные: левый край залитого квадрата — как раз
    он. Пропустить его значило бы выровнять три стороны из четырёх.
    """
    found = []
    start = None

    for order in range(len(points)):
        letter = commands[points[order][0]][0]

        if letter == 'M':
            start = order
        elif letter in ('L', 'H', 'V'):
            found.append((order - 1, order))

    # Замыкание ищется по самому Z: он стоит после последней точки подпути.
    for at, (letter, _) in enumerate(commands):
        if letter != 'Z':
            continue

        last = max((order for order, point in enumerate(points) if point[0] < at), default=None)
        opening = max((order for order, point in enumerate(points)
                       if point[0] < at and commands[point[0]][0] == 'M'), default=None)

        if last is not None and opening is not None and last != opening:
            found.append((last, opening))

    return found


def apply(commands, points, shift):
    """Переписывает координаты команд по собранным намерениям."""
    moved = 0

    for order, wanted in shift.items():
        at, x, y = points[order]
        letter, numbers = commands[at]

        new_x = wanted.get('x', x)
        new_y = wanted.get('y', y)

        if abs(new_x - x) < 1e-9 and abs(new_y - y) < 1e-9:
            continue

        moved += 1

        if letter == 'H':
            commands[at] = ('H', [new_x])
        elif letter == 'V':
            commands[at] = ('V', [new_y])
        else:
            numbers = list(numbers)
            numbers[-2], numbers[-1] = new_x, new_y
            commands[at] = (letter, numbers)

    return moved


def main():
    write = '--write' in sys.argv
    changed = 0
    total = 0

    for family in ('actions', 'toolbox'):
        folder = os.path.join(ICONS, family)

        for name in sorted(os.listdir(folder)):
            if not name.endswith('.svg'):
                continue

            file = os.path.join(folder, name)
            text = io.open(file, encoding='utf-8').read()
            match = PATH.search(text)
            path = match.group('path')
            filled = 'fill="none"' not in text
            fixed, moved = normalize(path, filled)

            total += 1

            if fixed == path:
                continue

            changed += 1
            print('%-24s %d точек' % (name[:-4], moved))

            if write:
                io.open(file, 'w', encoding='utf-8', newline='\n').write(
                    text[:match.start('path')] + fixed + text[match.end('path'):])

    print('\nиконок %d, тронуто %d%s' % (total, changed, '' if write else ' (пробный прогон)'))


main()
