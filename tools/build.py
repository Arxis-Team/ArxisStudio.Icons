# -*- coding: utf-8 -*-
"""Собирает src/AxIcons.cs из файлов icons/.

Источник правды — сами SVG: их открывает и правит дизайнер, их видно в диффе.
AxIcons.cs — производная, и правится не руками, а этим скриптом.

Переписывается только размеченная область; всё, что вокруг, — заголовок,
пояснения, реестр заливки — остаётся на месте. Так проза живёт там, где её
читают, а не в шаблоне генератора.

Геттер разбирает путь при первом обращении, а не при загрузке типа: набор
целиком — десятки разборов до первого кадра, а экрану нужны единицы. Иконка,
нарисованная силуэтом (fill="currentColor" в файле), встаёт в реестр заливки,
и AxIcon зальёт её сам — автору не нужно помнить, какие пути в наборе силуэты.

Рядом с путём клетки 16 идут пути остальных клеток набора — те же линии,
пересчитанные под размеры, которые значок занимает на разных масштабах экрана
(tools/fit.py). В клетке 16 единица занимает при 125 % 1.25 пикселя, оси штрихов
оказываются на четверти пикселя, и ядра у штриха нет вовсе; в своей клетке единица
снова равна пикселю. Разбираются эти пути так же лениво и только той клеткой,
которая сейчас на экране.

    python tools/build.py

Порядок внутри семейства — по группе, потом по имени: он должен быть
предсказуемым, а не зависеть от того, в каком порядке когда-то дописывали.
"""
import io
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
TARGET = os.path.join(ROOT, 'src', 'AxIcons.cs')
ICONS = os.path.join(ROOT, 'icons')

OPEN = '// <иконки:{family}> — собрано tools/build.py из icons/{family}, руками не править'
CLOSE = '// </иконки:{family}>'

import fit

PATH = re.compile(r'<path d="(?P<path>[^"]+)"', re.S)
FILL = re.compile(r'<svg[^>]*\sfill="(?P<fill>[^"]+)"', re.S)
TITLE = re.compile(r'<title>(?P<title>.*?)</title>', re.S)
DESC = re.compile(r'<desc>(?P<desc>.*?)</desc>', re.S)


def unescape(text):
    return text.replace('&lt;', '<').replace('&gt;', '>').replace('&amp;', '&')


def read(family):
    """Читает семейство: имя, подпись, группа, путь, силуэт ли."""
    folder = os.path.join(ICONS, family)
    found = []

    for name in sorted(os.listdir(folder)):
        if not name.endswith('.svg'):
            continue

        text = io.open(os.path.join(folder, name), encoding='utf-8').read()

        found.append({
            'name': os.path.splitext(name)[0],
            'title': unescape(TITLE.search(text).group('title').strip()),
            'group': unescape(DESC.search(text).group('desc').strip()) if DESC.search(text) else '',
            'path': unescape(PATH.search(text).group('path').strip()),
            'filled': FILL.search(text).group('fill') != 'none',
        })

    return sorted(found, key=lambda icon: (icon['group'], icon['name']))


def render(icons, indent):
    """Собирает объявления: группа комментарием, иконка — свойством."""
    pad = ' ' * indent
    lines = []
    group = None

    for icon in icons:
        if icon['group'] != group:
            group = icon['group']

            if group:
                lines.append('')
                lines.append(pad + '// ' + group)

        head = pad + 'public static Geometry ' + icon['name'] + ' { get => field ??= '
        call = 'Filled(P(' if icon['filled'] else 'P('
        tail = '); }' if icon['filled'] else '; }'
        fitted = [fit.fit(icon['path'], cell) for cell in fit.CELLS]

        lines.append(pad + '/// <summary>' + icon['title'] + '</summary>')
        lines.append(head + call + '"' + icon['path'] + '",')
        lines.append(pad + '    [')

        for at, path in enumerate(fitted):
            lines.append(pad + '        "' + path + '"' + (',' if at + 1 < len(fitted) else ''))

        lines.append(pad + '    ])' + tail)
        lines.append('')

    while lines and not lines[0]:
        lines.pop(0)

    while lines and not lines[-1]:
        lines.pop()

    return lines


def replace(text, family, indent):
    """Меняет размеченную область на свежесобранную."""
    start = OPEN.format(family=family)
    end = CLOSE.format(family=family)

    at = text.find(start)
    to = text.find(end)

    if at < 0 or to < 0:
        print('в AxIcons.cs нет области ' + family, file=sys.stderr)
        sys.exit(1)

    head = text[:text.index('\n', at) + 1]
    tail = text[text.rfind('\n', 0, to) + 1:]
    body = '\n'.join(render(read(family), indent))

    return head + body + '\n' + tail


def main():
    text = io.open(TARGET, encoding='utf-8-sig').read().replace('\r\n', '\n')

    text = replace(text, 'actions', 4)

    io.open(TARGET, 'w', encoding='utf-8', newline='\r\n').write(text)

    print('собрано:', len(read('actions')))


main()
