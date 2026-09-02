# -*- coding: utf-8 -*-
"""Собирает src/AxIcons.cs из файлов icons/.

Источник правды — сами SVG: их открывает и правит дизайнер, их видно в диффе.
AxIcons.cs — производная, и правится не руками, а этим скриптом.

Переписывается только размеченная область; всё, что вокруг, — заголовок,
пояснения, вложенный класс — остаётся на месте. Так проза живёт там, где её
читают, а не в шаблоне генератора.

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

PATH = re.compile(r'<path d="(?P<path>[^"]+)"', re.S)
TITLE = re.compile(r'<title>(?P<title>.*?)</title>', re.S)
DESC = re.compile(r'<desc>(?P<desc>.*?)</desc>', re.S)


def unescape(text):
    return text.replace('&lt;', '<').replace('&gt;', '>').replace('&amp;', '&')


def read(family):
    """Читает семейство: имя, подпись, группа, путь."""
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

        lines.append(pad + '/// <summary>' + icon['title'] + '</summary>')
        lines.append(pad + 'public static Geometry ' + icon['name'] + ' { get; } = P("' + icon['path'] + '");')
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
    text = replace(text, 'toolbox', 8)

    io.open(TARGET, 'w', encoding='utf-8', newline='\r\n').write(text)

    print('собрано:', sum(len(read(family)) for family in ('actions', 'toolbox')))


main()
