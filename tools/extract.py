# -*- coding: utf-8 -*-
"""Разбирает AxIcons.cs на отдельные SVG — по файлу на иконку.

Одноразовый переезд: после него источником правды становятся сами файлы
icons/, а AxIcons.cs собирается из них генератором build.py. Скрипт оставлен
в репозитории не для повторных запусков, а как запись о том, откуда взялись
файлы и что в них перенесено без потерь: путь, подпись и группа.
"""
import io
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SOURCE = os.path.join(ROOT, 'src', 'AxIcons.cs')
TARGET = os.path.join(ROOT, 'icons')

# Заголовок SVG: viewBox — та же клетка 16×16, в которой нарисован набор;
# обводка объявлена currentColor, потому что цвет иконке даёт тема.
TEMPLATE = (
    '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="none"\n'
    '     stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round">\n'
    '  <title>{title}</title>\n'
    '  <desc>{group}</desc>\n'
    '  <path d="{path}"/>\n'
    '</svg>\n')

# Подпись иконки — одна строка и вплотную к свойству: многострочные summary в
# файле есть только у классов, и принимать их за иконки нельзя.
ICON = re.compile(
    r'^[ \t]*/// <summary>(?P<summary>[^\n]*?)</summary>\n'
    r'[ \t]*public static Geometry (?P<name>\w+) \{ get; \} = P\([ \t]*\n?[ \t]*"(?P<path>[^"]+)"\);',
    re.M)

# Группа — обычный комментарий, не XML-документация и не линейка из дефисов.
GROUP = re.compile(r'^[ \t]*//(?!/)[ \t]*(?P<group>[^\n]*?)[ \t]*$', re.M)


def families(text):
    """Делит файл на два семейства: основной набор и вложенный Toolbox."""
    at = text.index('public static class Toolbox')

    return [('actions', text[:at]), ('toolbox', text[at:])]


def groups(text):
    """Позиции комментариев-разделителей: ими набор разбит на группы."""
    found = []

    for match in GROUP.finditer(text):
        line = match.group('group')

        # Линейки из дефисов и служебные пояснения группами не считаются.
        if not line or line.startswith('---') or len(line) > 40 or '.' in line:
            continue

        found.append((match.start(), line))

    return found


def group_of(at, found):
    name = ''

    for start, line in found:
        if start > at:
            break

        name = line

    return name


def escape(text):
    return text.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;')


def main():
    text = io.open(SOURCE, encoding='utf-8-sig').read().replace('\r\n', '\n')
    written = 0

    for family, chunk in families(text):
        folder = os.path.join(TARGET, family)
        os.makedirs(folder, exist_ok=True)

        found = groups(chunk)

        for match in ICON.finditer(chunk):
            name = match.group('name')
            body = TEMPLATE.format(
                stroke='1',
                title=escape(match.group('summary').strip()),
                group=escape(group_of(match.start(), found)),
                path=escape(match.group('path')))

            io.open(os.path.join(folder, name + '.svg'), 'w', encoding='utf-8', newline='\n').write(body)
            written += 1

    print('иконок вынуто:', written)

    if written != 161:
        print('ОЖИДАЛОСЬ 161 — разбор потерял иконки', file=sys.stderr)
        sys.exit(1)


main()
