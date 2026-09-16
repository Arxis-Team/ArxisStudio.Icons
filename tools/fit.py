# -*- coding: utf-8 -*-
"""Переносит путь набора в клетку другого размера, не уводя его с пиксельной сетки.

Набор нарисован в клетке 16, и в пиксели он ложится только там, где на единицу сетки приходится
целое число пикселей: при 100 % это один пиксель, при 200 % — два. При 125, 150 и 175 % единица
занимает 1.25, 1.5 и 1.75 пикселя, и оси штрихов, стоящие на n.5, оказываются на четверти пикселя:
ядра у штриха нет, есть две серые половины. Замером по всему набору — доля пикселей, закрашенных
целиком: 54 % при 100 %, 28 % при 125 %, 34 % при 150 %.

Лечится это не толщиной, а клеткой. Значок в 16 точек при 125 % занимает ровно 20 пикселей, и путь,
пересчитанный под клетку 20, снова получает пиксель на единицу. Тот же замер после пересчёта: 53 %
при 125 %, 53 % при 150 %, 52 % при 175 % — уровень масштаба, под который набор нарисован.

Пересчёт обязан сохранять не координаты, а вид штриха, и поэтому идёт по видам:

* ось (координата на n.5) садится на ближайшую n.5 новой клетки — там пиксельный центр;
* конец, край заливки, целая координата — на ближайшее целое: это граница пикселя;
* диагональ под 45° остаётся под 45°: снимается её начало, а конец отсчитывается от него целым
  числом единиц. Снимать оба конца порознь нельзя — 34 диагонали из 101 переставали быть 45°,
  и шеврон получал плечи разной длины;
* окружность, записанная двумя полудугами, собирается заново из снятого центра и снятого радиуса:
  сними её концы порознь — и радиус разойдётся с расстоянием между ними, а растеризатор растянет
  дугу, чтобы свести концы.

    python tools/fit.py            — показать, что выйдет для клетки 20
    python tools/fit.py 24         — то же для клетки 24
"""
import io
import os
import sys

import svgpath

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ICONS = os.path.join(ROOT, 'icons')

# Клетки, под которые набор пересчитывается.
#
# Обычный значок в шестнадцать точек занимает при 125, 150 и 175 % ровно 20, 24 и 28 пикселей;
# мелкий, в двенадцать, — 12, 15, 18 и 21 при 100…175 %. При 200 % оба попадают на клетку, где на
# единицу приходится два пикселя, и своя клетка им не нужна: 32 и 24.
#
# Мелкий без своей клетки — худший случай набора: двенадцать точек на клетку 16 дают три четверти
# пикселя на единицу, и замером по всему набору доля сплошных пикселей там 1.3 %.
CELLS = (12, 15, 18, 20, 21, 24, 28)

EPS = 1e-6


def is_half(value):
    return abs((value - 0.5) - round(value - 0.5)) < EPS


def is_whole(value):
    return abs(value - round(value)) < EPS


def num(value):
    text = ('%.4f' % value).rstrip('0').rstrip('.')

    return '0' if text in ('', '-0') else text


def snap(value, k):
    """Координата в новой клетке: на своей доле пикселя.

    Свободная координата — та, что не стоит ни на целом, ни на полупикселе, — только
    множится. Такие есть у звезды и у глаза: они нарисованы не по сетке, и посадить их
    на неё значит не уточнить рисунок, а сломать его — у звезды два луча сливались в
    горизонтальную полку.
    """
    scaled = value * k

    if is_half(value):
        return _round(scaled - 0.5) + 0.5

    if is_whole(value):
        return _round(scaled)

    return scaled


def _round(value):
    """Округление «от нуля» на половине: у Python оно банковское, и 2.5 давало 2."""
    return int(value + 0.5) if value >= 0 else -int(-value + 0.5)


def fit(text, cell):
    """Пересчитывает путь клетки 16 под клетку cell."""
    k = cell / 16.0
    out = []
    at = None          # где стоит перо в старых координатах
    now = None         # где оно стоит в новых
    start = None
    started = None

    commands = _circles(svgpath.parse(text), k)

    for letter, numbers in commands:
        if letter == 'круг':
            cx, cy, r = numbers
            out.append('M%s %sA%s %s 0 1 1 %s %sA%s %s 0 1 1 %s %s' % (
                num(cx - r), num(cy), num(r), num(r), num(cx + r), num(cy),
                num(r), num(r), num(cx - r), num(cy)))
            at = now = start = started = None
            continue

        if letter == 'Z':
            out.append('Z')
            at, now = start, started
            continue

        if letter == 'M':
            at = (numbers[0], numbers[1])
            now = (snap(at[0], k), snap(at[1], k))
            start, started = at, now
            out.append('M%s %s' % (num(now[0]), num(now[1])))
            continue

        if letter == 'H':
            at = (numbers[0], at[1])
            now = (snap(at[0], k), now[1])
            out.append('H%s' % num(now[0]))
            continue

        if letter == 'V':
            at = (at[0], numbers[0])
            now = (now[0], snap(at[1], k))
            out.append('V%s' % num(now[1]))
            continue

        if letter == 'L':
            to = (numbers[0], numbers[1])
            step = (to[0] - at[0], to[1] - at[1])

            if abs(abs(step[0]) - abs(step[1])) < EPS and abs(step[0]) > EPS:
                # Диагональ под 45°: длина берётся целым числом единиц, знаки — прежние.
                arm = max(1, _round(abs(step[0]) * k))
                now = (now[0] + arm * _sign(step[0]), now[1] + arm * _sign(step[1]))
            else:
                now = (snap(to[0], k), snap(to[1], k))

            at = to
            out.append('L%s %s' % (num(now[0]), num(now[1])))
            continue

        if letter == 'A':
            rx, ry, rot, large, sweep, x, y = numbers
            to = (x, y)

            # Полуокружность в паре с такой же — это круг: центр посередине концов.
            if abs(rx - ry) < EPS and abs(abs(to[0] - at[0]) - 2 * rx) < EPS and abs(to[1] - at[1]) < EPS:
                centre = ((at[0] + to[0]) / 2, at[1])
                cx, cy = snap(centre[0], k), snap(centre[1], k)
                r = _round(rx * k) if not is_half(rx) else _round(rx * k - 0.5) + 0.5
                r = max(r, 1)
                now = (cx + r if to[0] > at[0] else cx - r, cy)
                out.append('A%s %s %s %s %s %s %s' % (
                    num(r), num(r), num(rot), num(large), num(sweep), num(now[0]), num(now[1])))
                at = to
                continue

            r = max(_round(rx * k) if not is_half(rx) else _round(rx * k - 0.5) + 0.5, 1)
            now = (snap(to[0], k), snap(to[1], k))
            at = to
            out.append('A%s %s %s %s %s %s %s' % (
                num(r), num(r), num(rot), num(large), num(sweep), num(now[0]), num(now[1])))
            continue

        # Кривые: снимаются все точки подряд — сетка им всё равно не помощница.
        fixed = []

        for step in range(0, len(numbers), 2):
            fixed.extend((snap(numbers[step], k), snap(numbers[step + 1], k)))

        at = (numbers[-2], numbers[-1])
        now = (fixed[-2], fixed[-1])
        out.append(letter + ' '.join(num(value) for value in fixed))

    return ''.join(out)


def _sign(value):
    return 1 if value > 0 else -1


def _circles(commands, k):
    """Заменяет пары полудуг — то есть окружности — одной командой «круг».

    Снятые порознь концы полудуг расходятся с радиусом, и растеризатор тянет дугу, чтобы их свести:
    круг выходит яйцом. Центр и радиус снимаются один раз, а концы считаются от них.
    """
    out = []
    at = 0

    while at < len(commands):
        letter, numbers = commands[at]
        pair = commands[at + 1:at + 3]

        if (letter == 'M' and len(pair) == 2 and all(one[0] == 'A' for one in pair)
                and _closes(numbers, pair)):
            left, top = numbers
            r = pair[0][1][0]
            centre = (left + r, top)
            cx, cy = snap(centre[0], k), snap(centre[1], k)
            radius = max(_round(r * k) if not is_half(r) else _round(r * k - 0.5) + 0.5, 1)
            out.append(('круг', (cx, cy, radius)))
            at += 3
            continue

        out.append((letter, numbers))
        at += 1

    return out


def _closes(start, pair):
    """Две дуги одного радиуса, вернувшиеся в начало, — это окружность."""
    first, second = pair[0][1], pair[1][1]

    return (abs(first[0] - first[1]) < EPS and abs(second[0] - second[1]) < EPS
            and abs(first[0] - second[0]) < EPS
            and abs(second[5] - start[0]) < EPS and abs(second[6] - start[1]) < EPS
            and abs(first[5] - (start[0] + 2 * first[0])) < EPS)


def read():
    """Пути набора: имя файла — путь."""
    found = {}

    for family in sorted(os.listdir(ICONS)):
        folder = os.path.join(ICONS, family)

        if not os.path.isdir(folder):
            continue

        for name in sorted(os.listdir(folder)):
            if not name.endswith('.svg'):
                continue

            text = io.open(os.path.join(folder, name), encoding='utf-8').read()
            start = text.index('<path d="') + len('<path d="')
            end = text.index('"', start)
            found[os.path.splitext(name)[0]] = text[start:end]

    return found


if __name__ == '__main__':
    cell = int(sys.argv[1]) if len(sys.argv) > 1 else CELLS[0]

    for name, path in read().items():
        print('%-22s %s' % (name, fit(path, cell)))
