# -*- coding: utf-8 -*-
"""Записывает набор из set.py в файлы icons/.

Подпись и группа берутся из уже лежащего файла — они не про геометрию и
переживают перерисовку. Меняются только путь и способ красить: контур или
силуэт. Новых имён скрипт не заводит и старых не удаляет: состав набора —
отдельное решение, а не побочный эффект.

    python tools/draw.py            — записать
    python tools/build.py           — пересобрать AxIcons.cs из файлов
"""
import io
import os
import re
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import set as icons  # noqa: E402  (имя модуля намеренно короткое)

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ICONS = os.path.join(ROOT, 'icons')

PATH = re.compile(r'<path d="[^"]*"')
FILL = re.compile(r'fill="[^"]*"')
STROKE = re.compile(r'stroke="[^"]*"')
CAP = re.compile(r'stroke-linecap="[^"]*"')
JOIN = re.compile(r'stroke-linejoin="[^"]*"')


def main():
    written = 0
    missing = []

    for family, name, d, filled in icons.build_all():
        file = os.path.join(ICONS, family, name + '.svg')

        if not os.path.exists(file):
            missing.append(family + '/' + name)
            continue

        text = io.open(file, encoding='utf-8').read()
        text = PATH.sub('<path d="%s"' % d, text, count=1)
        text = FILL.sub('fill="%s"' % ('currentColor' if filled else 'none'), text, count=1)
        text = STROKE.sub('stroke="%s"' % ('none' if filled else 'currentColor'), text, count=1)
        # Плоский срез и острый стык — то, чем набор теперь рисуется везде.
        text = CAP.sub('stroke-linecap="butt"', text, count=1)
        text = JOIN.sub('stroke-linejoin="miter"', text, count=1)

        io.open(file, 'w', encoding='utf-8', newline='\n').write(text)
        written += 1

    known = set()

    for family in ('actions', 'toolbox'):
        for name in os.listdir(os.path.join(ICONS, family)):
            if name.endswith('.svg'):
                known.add(family + '/' + name[:-4])

    drawn = set(family + '/' + name for family, name, _, _ in icons.build_all())

    print('записано:', written)

    if missing:
        print('в наборе нет файла для:', ', '.join(sorted(missing)), file=sys.stderr)

    forgotten = sorted(known - drawn)

    if forgotten:
        print('не перерисованы:', ', '.join(forgotten), file=sys.stderr)

    if missing or forgotten:
        sys.exit(1)


main()
