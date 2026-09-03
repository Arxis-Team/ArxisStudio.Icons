# -*- coding: utf-8 -*-
"""Весь набор, собранный конструктором grid.Icon.

Каждая иконка — несколько операций на сетке, а не числа, набранные руками.
Симметрия здесь не проверяется, а строится: половина рисуется, вторая
отражается. Что нельзя собрать из прямых, дуг и диагоналей под 45° — глаз,
звезда, кривая иконки пути — записано целиком через raw, и таких мест
единицы, каждое названо.

Общий ритм:

* контейнеры палитры стоят на рамке F — 3.5…13.5 в обе стороны;
* однострочные контролы — на рамке B — 3.5…13.5 × 5.5…11.5;
* статусы — окружность радиуса 5 в центре;
* центральный штрих всегда на 8.5 — это центральный пиксель клетки.
"""
from grid import Icon

F = (3.5, 3.5, 13.5, 13.5)
B = (3.5, 5.5, 13.5, 11.5)
C = 8.5


def frame():
    return Icon().rect(*F)


def bar():
    return Icon().rect(*B)


def chevron_down(icon, cx, cy, arm):
    """Шеврон остриём вниз: вершина в (cx, cy), плечи под 45°."""
    return icon.polyline((cx - arm, cy - arm), (cx, cy), (cx + arm, cy - arm))


def chevron_up(icon, cx, cy, arm):
    return icon.polyline((cx - arm, cy + arm), (cx, cy), (cx + arm, cy + arm))


def chevron_left(icon, cx, cy, arm):
    return icon.polyline((cx + arm, cy - arm), (cx, cy), (cx + arm, cy + arm))


def chevron_right(icon, cx, cy, arm):
    return icon.polyline((cx - arm, cy - arm), (cx, cy), (cx - arm, cy + arm))


def status():
    return Icon().circle(C, C, 5)


# ---------------------------------------------------------------------------
# Семейство 1 — действия и объекты
# ---------------------------------------------------------------------------

ACTIONS = {}


def action(name):
    def register(build):
        ACTIONS[name] = build
        return build

    return register


@action('ChevronDown')
def _():
    return chevron_down(Icon(), C, 10.5, 4)


@action('ChevronUp')
def _():
    return chevron_up(Icon(), C, 6.5, 4)


@action('ChevronLeft')
def _():
    return chevron_left(Icon(), 6.5, C, 4)


@action('ChevronRight')
def _():
    return chevron_right(Icon(), 10.5, C, 4)


@action('ChevronDownSmall')
def _():
    return chevron_down(Icon(), C, 9.5, 3)


@action('CollapseAll')
def _():
    icon = Icon().hline(3.5, 4, 13).hline(13.5, 4, 13)
    chevron_down(icon, C, 7.5, 2)
    return chevron_up(icon, C, 9.5, 2)


@action('ExpandAll')
def _():
    icon = Icon().hline(3.5, 4, 13).hline(13.5, 4, 13)
    chevron_up(icon, C, 5.5, 2)
    return chevron_down(icon, C, 11.5, 2)


@action('Plus')
def _():
    return Icon().vline(C, 4, 13).hline(C, 4, 13)


@action('Minus')
def _():
    return Icon().hline(C, 4, 13)


@action('Close')
def _():
    return Icon().diag(4.5, 4.5, 12.5, 12.5).diag(12.5, 4.5, 4.5, 12.5)


@action('Check')
def _():
    return Icon().polyline((3.5, 8.5), (6.5, 11.5), (12.5, 5.5))


@action('Copy')
def _():
    return Icon().rect(6.5, 6.5, 13.5, 13.5).polyline((11, 3.5), (3.5, 3.5), (3.5, 11))


@action('Edit')
def _():
    # Карандаш под 45°: корпус — замкнутый контур, обойма — перпендикулярная черта.
    return (Icon()
            .poly((11.5, 3.5), (13.5, 5.5), (6.5, 12.5), (4.5, 12.5), (4.5, 10.5))
            .diag(9.5, 5.5, 11.5, 7.5))


@action('Trash')
def _():
    return (Icon()
            .hline(4.5, 4, 13)
            .polyline((6.5, 4), (6.5, 3.5), (10.5, 3.5), (10.5, 4))
            .polyline((4.5, 5), (4.5, 13.5), (12.5, 13.5), (12.5, 5))
            .vline(7.5, 7, 12)
            .vline(9.5, 7, 12))


@action('Undo')
def _():
    # Стрелка влево и дуга, уходящая вниз вправо: радиус 3 из центра (10.5, 9.5).
    return (Icon()
            .polyline((6.5, 3.5), (3.5, 6.5), (6.5, 9.5))
            .raw('M4 6.5H10.5A3 3 0 0 1 10.5 12.5H8'))


@action('Redo')
def _():
    return (Icon()
            .polyline((10.5, 3.5), (13.5, 6.5), (10.5, 9.5))
            .raw('M13 6.5H6.5A3 3 0 0 0 6.5 12.5H9'))


@action('Refresh')
def _():
    # Семь восьмых окружности радиуса 5 и угол-стрелка в разрыве.
    return (Icon()
            .raw('M13.5 8.5A5 5 0 1 1 12.5 5.5L13.5 6.5')
            .polyline((13.5, 3), (13.5, 6.5), (10, 6.5)))


@action('Download')
def _():
    icon = Icon().vline(C, 3, 10).hline(13.5, 4, 13)
    return chevron_down(icon, C, 10.5, 3)


@action('Sort')
def _():
    icon = Icon().vline(5.5, 5, 13).vline(11.5, 4, 12)
    chevron_up(icon, 5.5, 4.5, 2)
    return chevron_down(icon, 11.5, 12.5, 2)


@action('Filter')
def _():
    return Icon().poly((3.5, 4.5), (13.5, 4.5), (9.5, 8.5), (9.5, 13.5), (7.5, 13.5), (7.5, 8.5))


@action('Search')
def _():
    return Icon().circle(7.5, 7.5, 4).diag(10.5, 10.5, 13.5, 13.5)


@action('ZoomIn')
def _():
    return ACTIONS['Search']().vline(7.5, 5, 10).hline(7.5, 5, 10)


@action('ZoomOut')
def _():
    return ACTIONS['Search']().hline(7.5, 5, 10)


@action('Settings')
def _():
    return (Icon()
            .hline(5.5, 4, 13).circle(6.5, 5.5, 2)
            .hline(11.5, 4, 13).circle(10.5, 11.5, 2))


@action('Gear')
def _():
    icon = Icon().circle(C, C, 4).circle(C, C, 1)
    icon.vline(C, 3, 5).diag(11.5, 5.5, 12.5, 4.5)
    icon.mirror_x()
    return icon.mirror_y()


@action('Pin')
def _():
    return (Icon()
            .hline(3.5, 6, 11)
            .polyline((6.5, 4), (6.5, 7.5), (4.5, 9.5), (12.5, 9.5), (10.5, 7.5), (10.5, 4))
            .vline(C, 10, 14))


@action('Star')
def _():
    # Пять лучей на сетку не ложатся; звезда записана целиком, симметрично вокруг 8.5.
    return Icon().raw(
        'M8.5 2.8L9.85 6.54L13.83 6.67L10.69 9.11L11.79 12.93L8.5 10.7'
        'L5.21 12.93L6.31 9.11L3.17 6.67L7.15 6.54Z')


@action('Lock')
def _():
    return (Icon()
            .rect(4.5, 7.5, 12.5, 12.5)
            .raw('M6.5 8V6.5A2 2 0 0 1 10.5 6.5V8')
            .vline(C, 9, 11))


@action('Unlock')
def _():
    return (Icon()
            .rect(4.5, 7.5, 12.5, 12.5)
            .raw('M6.5 8V5.5A2 2 0 0 1 10.5 5.5V6')
            .vline(C, 9, 11))


@action('Package')
def _():
    return (Icon()
            .poly((8.5, 3.5), (12.5, 5.5), (12.5, 11.5), (8.5, 13.5), (4.5, 11.5), (4.5, 5.5))
            .polyline((4.5, 5.5), (8.5, 7.5), (12.5, 5.5))
            .vline(C, 8, 13))


@action('Plugin')
def _():
    return (Icon()
            .vline(6.5, 3, 6).vline(10.5, 3, 6)
            .raw('M4.5 5.5H12.5V8.5A2 2 0 0 1 10.5 10.5H6.5A2 2 0 0 1 4.5 8.5Z')
            .vline(C, 11, 14))


@action('Terminal')
def _():
    icon = frame().hline(11.5, 9, 12)
    return chevron_right(icon, 7.5, 8.5, 2)


@action('HotReload')
def _():
    return Icon().poly((9.5, 3.5), (5.5, 8.5), (8.5, 8.5), (7.5, 13.5), (11.5, 8.5), (8.5, 8.5))


@action('Debug')
def _():
    icon = (Icon()
            .raw('M6.5 7.5H10.5V10.5A2 2 0 0 1 8.5 12.5A2 2 0 0 1 6.5 10.5Z')
            .diag(6.5, 4.5, 7.5, 5.5)
            .hline(8.5, 3, 6)
            .diag(5.5, 11.5, 4.5, 12.5))
    return icon.mirror_x()


@action('Image')
def _():
    return (frame()
            .polyline((3.5, 11.5), (6.5, 8.5), (9.5, 11.5), (11.5, 9.5), (13.5, 11.5))
            .dot(10.5, 6.5))


@action('Media')
def _():
    return frame().poly((6.5, 5.5), (9.5, 8.5), (6.5, 11.5))


@action('Bubble')
def _():
    return Icon().poly((3.5, 3.5), (13.5, 3.5), (13.5, 10.5), (8.5, 10.5), (5.5, 13.5), (5.5, 10.5), (3.5, 10.5))


@action('User')
def _():
    return Icon().circle(C, 6.5, 2).raw('M4.5 14V13.5A4 4 0 0 1 12.5 13.5V14')


@action('Keyboard')
def _():
    return (bar()
            .dot(5.5, 7.5).dot(7.5, 7.5).dot(9.5, 7.5).dot(11.5, 7.5)
            .hline(9.5, 6, 11))


@action('Layers')
def _():
    return (Icon()
            .poly((8.5, 3.5), (13.5, 6.5), (8.5, 9.5), (3.5, 6.5))
            .polyline((3.5, 9.5), (8.5, 12.5), (13.5, 9.5)))


@action('Component')
def _():
    return frame().rect(6.5, 6.5, 10.5, 10.5)


@action('AlignLeft')
def _():
    return Icon().vline(3.5, 3, 14).hline(6.5, 5, 12).hline(8.5, 5, 14).hline(10.5, 5, 10)


@action('Move')
def _():
    icon = Icon().vline(C, 3, 14).hline(C, 3, 14)
    chevron_up(icon, C, 3.5, 2)
    chevron_down(icon, C, 13.5, 2)
    chevron_left(icon, 3.5, C, 2)
    return chevron_right(icon, 13.5, C, 2)


@action('Pointer')
def _():
    return Icon().poly((4.5, 3.5), (4.5, 12.5), (6.5, 10.5), (8.5, 13.5), (10.5, 12.5), (8.5, 9.5), (11.5, 9.5))


@action('Preview')
def _():
    # Веко — кривые, симметричные относительно 8.5; зрачок — окружность радиуса 2.
    return (Icon()
            .raw('M3.5 8.5C5.5 5.5 7 4.5 8.5 4.5C10 4.5 11.5 5.5 13.5 8.5'
                 'C11.5 11.5 10 12.5 8.5 12.5C7 12.5 5.5 11.5 3.5 8.5Z')
            .circle(C, C, 2))


@action('PreviewOff')
def _():
    return (Icon()
            .raw('M3.5 8.5C5.5 5.5 7 4.5 8.5 4.5C10 4.5 11.5 5.5 13.5 8.5'
                 'C11.5 11.5 10 12.5 8.5 12.5C7 12.5 5.5 11.5 3.5 8.5Z')
            .diag(3.5, 13.5, 13.5, 3.5))


@action('Text')
def _():
    return Icon().hline(4.5, 4, 13).vline(C, 5, 12).hline(12.5, 6, 11)


@action('Split')
def _():
    return frame().vline(C, 4, 13)


@action('Structure')
def _():
    return Icon().hline(4.5, 3, 14).hline(8.5, 6, 14).hline(12.5, 9, 14)


@action('Tabs')
def _():
    return (Icon()
            .rect(3.5, 6.5, 13.5, 13.5)
            .polyline((3.5, 6.5), (3.5, 3.5), (8.5, 3.5), (8.5, 6.5)))


@action('Template')
def _():
    return frame().circle(C, 7.5, 2)


@action('DataGrid')
def _():
    return frame().hline(6.5, 4, 13).hline(10.5, 4, 13).vline(C, 7, 13)


@action('Grid')
def _():
    return frame().vline(C, 4, 13).hline(C, 4, 13)


@action('GridSnap')
def _():
    return Icon().hline(6.5, 3, 14).hline(10.5, 3, 14).vline(6.5, 3, 14).vline(10.5, 3, 14)


@action('List')
def _():
    return frame().hline(6.5, 6, 11).hline(8.5, 6, 11).hline(10.5, 6, 11)


@action('Window')
def _():
    return frame().hline(6.5, 4, 13)


@action('WindowMaximize')
def _():
    return Icon().rect(4.5, 4.5, 12.5, 12.5)


@action('WindowMinimize')
def _():
    return Icon().hline(C, 4, 13)


@action('WindowRestore')
def _():
    return (Icon()
            .rect(4.5, 6.5, 10.5, 12.5)
            .polyline((6.5, 6.5), (6.5, 4.5), (12.5, 4.5), (12.5, 10.5), (10.5, 10.5)))


@action('FitToScreen')
def _():
    return Icon().polyline((3.5, 7), (3.5, 3.5), (7, 3.5)).mirror_x().mirror_y()


@action('ExternalLink')
def _():
    return (Icon()
            .polyline((7, 3.5), (3.5, 3.5), (3.5, 13.5), (13.5, 13.5), (13.5, 10))
            .polyline((10, 3.5), (13.5, 3.5), (13.5, 7))
            .diag(13.5, 3.5, 8.5, 8.5))


@action('Binding')
def _():
    # Два звена под 45°: дуги радиуса 2 сцеплены в центре, всё симметрично поворотом на 180°.
    return Icon().raw(
        'M9.5 6.5L10.5 5.5A2 2 0 0 1 13.5 8.5L12.5 9.5'
        'M7.5 10.5L6.5 11.5A2 2 0 0 1 3.5 8.5L4.5 7.5'
        'M6.5 10.5L10.5 6.5')


@action('Branch')
def _():
    return (Icon()
            .circle(5.5, 11.5, 2).circle(11.5, 4.5, 2)
            .raw('M5.5 10V7.5A3 3 0 0 1 8.5 4.5H9'))


@action('Docs')
def _():
    return (Icon()
            .raw('M8.5 4.5C7.5 3.5 6 3.5 3.5 3.5V12.5C6 12.5 7.5 13 8.5 13.5')
            .mirror_x()
            .vline(C, 5, 13))


@action('Dashboard')
def _():
    return (Icon()
            .rect(3.5, 3.5, 7.5, 8.5).rect(9.5, 3.5, 13.5, 6.5)
            .rect(3.5, 10.5, 7.5, 13.5).rect(9.5, 8.5, 13.5, 13.5))


@action('Document')
def _():
    return (Icon()
            .poly((4.5, 3.5), (9.5, 3.5), (12.5, 6.5), (12.5, 13.5), (4.5, 13.5))
            .polyline((9.5, 3.5), (9.5, 6.5), (12.5, 6.5)))


@action('DocumentCode')
def _():
    icon = ACTIONS['Document']()
    chevron_left(icon, 6.5, 10.5, 1)
    return chevron_right(icon, 10.5, 10.5, 1)


@action('Folder')
def _():
    return Icon().poly((3.5, 12.5), (3.5, 4.5), (7.5, 4.5), (9.5, 6.5), (13.5, 6.5), (13.5, 12.5))


@action('FolderOpen')
def _():
    return (Icon()
            .polyline((13.5, 8), (13.5, 6.5), (9.5, 6.5), (7.5, 4.5), (3.5, 4.5), (3.5, 12.5), (12, 12.5))
            .polyline((3.5, 12.5), (5.5, 8.5), (13.5, 8.5), (11.5, 12.5)))


@action('Error')
def _():
    return status().diag(6.5, 6.5, 10.5, 10.5).diag(10.5, 6.5, 6.5, 10.5)


@action('Success')
def _():
    return status().polyline((5.5, 8.5), (7.5, 10.5), (11.5, 6.5))


@action('Warning')
def _():
    return status().vline(C, 5, 9).dot(C, 11.5)


@action('Info')
def _():
    return status().dot(C, 5.5).vline(C, 7, 12)


@action('Question')
def _():
    return status().raw('M6.5 6.5A2 2 0 1 1 8.5 8.5V10').dot(C, 11.5)


@action('WarningTriangle')
def _():
    return (Icon()
            .poly((8.5, 3.5), (13.5, 13.5), (3.5, 13.5))
            .vline(C, 7, 10)
            .dot(C, 11.5))


@action('ThemeDark')
def _():
    return Icon().raw('M8.5 3.5A4 4 0 0 0 13.5 8.5A5 5 0 1 1 8.5 3.5Z')


@action('ThemeLight')
def _():
    return Icon().circle(C, C, 2).vline(C, 3, 5).diag(4.5, 4.5, 5.5, 5.5).mirror_x().mirror_y()


@action('Play')
def _():
    return Icon(filled=True).poly((6, 4), (12, 8), (6, 12))


@action('Stop')
def _():
    return Icon(filled=True).rect(5, 5, 12, 12)


@action('Pause')
def _():
    return Icon(filled=True).rect(5, 4, 7, 12).rect(10, 4, 12, 12)


@action('MoreHorizontal')
def _():
    # Точки — залитые кружки радиуса 1.5: диаметр 3, края на целых.
    return Icon(filled=True).circle(4.5, C, 1.5).circle(C, C, 1.5).circle(12.5, C, 1.5)


@action('MoreVertical')
def _():
    return Icon(filled=True).circle(C, 4.5, 1.5).circle(C, C, 1.5).circle(C, 12.5, 1.5)


# ---------------------------------------------------------------------------
# Семейство 2 — глифы контролов для палитры дизайнера
# ---------------------------------------------------------------------------

TOOLBOX = {}


def glyph(name):
    def register(build):
        TOOLBOX[name] = build
        return build

    return register


@glyph('Button')
def _():
    return bar().hline(C, 6, 11)


@glyph('RepeatButton')
def _():
    icon = bar().hline(C, 5, 9)
    chevron_right(icon, 10.5, C, 1)
    return chevron_right(icon, 12.5, C, 1)


@glyph('ToggleButton')
def _():
    return bar().vline(C, 6, 11)


@glyph('SplitButton')
def _():
    icon = bar().vline(9.5, 6, 11).hline(C, 5, 8)
    return chevron_down(icon, 11.5, 8.5, 1)


@glyph('DropDownButton')
def _():
    icon = bar().hline(C, 5, 9)
    return chevron_down(icon, 11.5, 8.5, 1)


@glyph('HyperlinkButton')
def _():
    return Icon().hline(6.5, 4, 13).hline(8.5, 4, 10).hline(10.5, 4, 10)


@glyph('ButtonSpinner')
def _():
    icon = bar().vline(9.5, 6, 11).hline(C, 5, 8)
    chevron_up(icon, 11.5, 7.5, 1)
    return chevron_down(icon, 11.5, 9.5, 1)


@glyph('NumericUpDown')
def _():
    icon = bar().vline(9.5, 6, 11).hline(C, 5, 7)
    chevron_up(icon, 11.5, 7.5, 1)
    return chevron_down(icon, 11.5, 9.5, 1)


@glyph('TextBox')
def _():
    return bar().hline(C, 5, 9).vline(10.5, 7, 10)


@glyph('MaskedTextBox')
def _():
    return bar().dot(5.5, C).dot(7.5, C).dot(9.5, C).hline(C, 11, 13)


@glyph('AutoCompleteBox')
def _():
    return Icon().rect(3.5, 4.5, 13.5, 7.5).vline(5.5, 5, 7).rect(3.5, 9.5, 13.5, 12.5)


@glyph('TextBlock')
def _():
    return Icon().hline(5.5, 4, 13).hline(8.5, 4, 13).hline(11.5, 4, 10)


@glyph('SelectableTextBlock')
def _():
    return Icon().hline(5.5, 4, 13).rect(3.5, 7.5, 10.5, 10.5).hline(12.5, 4, 10)


@glyph('Label')
def _():
    return Icon().hline(C, 4, 10).dot(12.5, C)


@glyph('TextArea')
def _():
    return frame().hline(6.5, 5, 12).hline(8.5, 5, 12).hline(10.5, 5, 9)


@glyph('CheckBox')
def _():
    return (Icon()
            .rect(3.5, 5.5, 8.5, 11.5)
            .polyline((4.5, 8.5), (5.5, 9.5), (7.5, 7.5))
            .hline(C, 10, 14))


@glyph('RadioButton')
def _():
    return Icon().circle(6.5, C, 3).circle(6.5, C, 1).hline(C, 11, 14)


@glyph('ComboBox')
def _():
    icon = bar().hline(C, 5, 9)
    return chevron_down(icon, 11.5, 8.5, 1)


@glyph('ComboBoxItem')
def _():
    return (Icon()
            .rect(3.5, 6.5, 13.5, 10.5)
            .polyline((4.5, 8.5), (5.5, 9.5), (7.5, 7.5))
            .hline(C, 9, 12))


@glyph('SegmentedControl')
def _():
    return bar().vline(6.5, 6, 11).vline(10.5, 6, 11)


@glyph('Slider')
def _():
    return Icon().hline(C, 3, 14).circle(10.5, C, 2)


@glyph('ToggleSwitch')
def _():
    return (Icon()
            .raw('M6.5 5.5H10.5A3 3 0 0 1 10.5 11.5H6.5A3 3 0 0 1 6.5 5.5Z')
            .circle(10.5, C, 1))


@glyph('Chip')
def _():
    return (Icon()
            .raw('M6.5 5.5H10.5A3 3 0 0 1 10.5 11.5H6.5A3 3 0 0 1 6.5 5.5Z')
            .hline(C, 7, 10))


@glyph('Calendar')
def _():
    return (Icon()
            .rect(3.5, 4.5, 13.5, 13.5).hline(7.5, 4, 13)
            .vline(6.5, 3, 5).vline(10.5, 3, 5)
            .dot(5.5, 9.5).dot(8.5, 9.5).dot(11.5, 9.5).dot(5.5, 11.5).dot(8.5, 11.5))


@glyph('CalendarDatePicker')
def _():
    return (Icon()
            .rect(3.5, 5.5, 9.5, 11.5).hline(C, 5, 8)
            .rect(10.5, 5.5, 13.5, 9.5).hline(7.5, 11, 13))


@glyph('DatePicker')
def _():
    return bar().vline(7.5, 6, 11).vline(10.5, 6, 11).hline(C, 5, 7).hline(C, 8, 10).hline(C, 11, 13)


@glyph('TimePicker')
def _():
    return status().vline(C, 6, 9).hline(C, 9, 11)


@glyph('Carousel')
def _():
    icon = Icon().rect(5.5, 4.5, 11.5, 12.5)
    chevron_left(icon, 3.5, C, 1)
    return chevron_right(icon, 13.5, C, 1)


@glyph('ItemsControl')
def _():
    return (Icon()
            .hline(5.5, 6, 14).hline(8.5, 6, 14).hline(11.5, 6, 14)
            .dot(4.5, 5.5).dot(4.5, 8.5).dot(4.5, 11.5))


@glyph('ListBoxItem')
def _():
    return Icon().rect(3.5, 6.5, 13.5, 10.5).hline(C, 5, 10)


@glyph('TreeView')
def _():
    return frame().hline(6.5, 5, 12).vline(5.5, 7, 12).hline(9.5, 7, 12).hline(11.5, 7, 12)


@glyph('TreeViewItem')
def _():
    return chevron_right(Icon(), 6.5, C, 2).hline(C, 8, 14)


@glyph('PipsPager')
def _():
    return Icon().circle(4.5, C, 1).dot(C, C).dot(12.5, C)


@glyph('Menu')
def _():
    return Icon().rect(3.5, 4.5, 13.5, 7.5).vline(6.5, 5, 7).vline(9.5, 5, 7).rect(3.5, 9.5, 9.5, 13.5)


@glyph('MenuItem')
def _():
    icon = Icon().rect(3.5, 6.5, 13.5, 10.5).hline(C, 5, 10)
    return chevron_right(icon, 12.5, C, 1)


@glyph('ContextMenu')
def _():
    return (Icon()
            .rect(4.5, 3.5, 12.5, 13.5)
            .hline(5.5, 6, 11).hline(7.5, 5, 12).hline(9.5, 6, 11).hline(11.5, 6, 11))


@glyph('Popup')
def _():
    return Icon().rect(3.5, 3.5, 10.5, 10.5).rect(6.5, 6.5, 13.5, 13.5)


@glyph('ToolTip')
def _():
    return Icon().rect(4.5, 5.5, 12.5, 9.5).polyline((6.5, 9.5), (8.5, 11.5), (10.5, 9.5))


@glyph('TeachingTip')
def _():
    return (Icon()
            .poly((3.5, 3.5), (13.5, 3.5), (13.5, 10.5), (8.5, 10.5), (5.5, 13.5), (5.5, 10.5), (3.5, 10.5))
            .vline(C, 5, 8)
            .dot(C, 9.5))


@glyph('QuickSearch')
def _():
    return frame().hline(6.5, 4, 13).vline(5.5, 4, 6).hline(9.5, 5, 12).hline(11.5, 5, 10)


@glyph('SearchField')
def _():
    return bar().circle(6.5, C, 2).diag(8.5, 10.5, 9.5, 11.5).hline(C, 11, 13)


@glyph('ScrollBar')
def _():
    return Icon().rect(6.5, 3.5, 10.5, 13.5).hline(5.5, 7, 10).hline(11.5, 7, 10)


@glyph('ScrollViewer')
def _():
    return Icon().rect(3.5, 3.5, 11.5, 13.5).vline(13.5, 5, 9)


@glyph('GridSplitter')
def _():
    return Icon().rect(3.5, 3.5, 7.5, 13.5).rect(9.5, 3.5, 13.5, 13.5).vline(C, 7, 10)


@glyph('Separator')
def _():
    return Icon().hline(C, 3, 7).hline(C, 8, 9).hline(C, 10, 14)


@glyph('Border')
def _():
    return frame().rect(5.5, 5.5, 11.5, 11.5)


@glyph('Canvas')
def _():
    return frame().rect(6.5, 6.5, 11.5, 10.5)


@glyph('Panel')
def _():
    return frame()


@glyph('ContentControl')
def _():
    return frame().circle(C, C, 2)


@glyph('DockPanel')
def _():
    return frame().hline(6.5, 4, 13).vline(5.5, 7, 13).vline(11.5, 7, 13)


@glyph('RelativePanel')
def _():
    return frame().rect(8.5, 8.5, 12.5, 11.5).hline(8.5, 4, 8).vline(10.5, 4, 8)


@glyph('SplitView')
def _():
    return frame().vline(6.5, 4, 13)


@glyph('StackPanel')
def _():
    return Icon().rect(3.5, 3.5, 13.5, 6.5).rect(3.5, 10.5, 13.5, 13.5)


@glyph('UniformGrid')
def _():
    return frame().hline(6.5, 4, 13).hline(10.5, 4, 13).vline(C, 4, 13)


@glyph('WrapPanel')
def _():
    return Icon().rect(3.5, 3.5, 6.5, 6.5).mirror_x().mirror_y()


@glyph('Viewbox')
def _():
    return (frame()
            .diag(5.5, 5.5, 10.5, 10.5)
            .polyline((5.5, 8), (5.5, 5.5), (8, 5.5))
            .polyline((10.5, 8), (10.5, 10.5), (8, 10.5)))


@glyph('Expander')
def _():
    icon = Icon().rect(3.5, 3.5, 13.5, 6.5).rect(3.5, 9.5, 13.5, 13.5)
    return chevron_down(icon, 5.5, 5.5, 1)


@glyph('GroupBox')
def _():
    return (Icon()
            .rect(3.5, 5.5, 13.5, 13.5)
            .polyline((5.5, 5.5), (5.5, 3.5), (9.5, 3.5), (9.5, 5.5)))


@glyph('TabItem')
def _():
    return Icon().polyline((4.5, 7.5), (4.5, 4.5), (10.5, 4.5), (10.5, 7.5)).hline(7.5, 3, 14)


@glyph('TabStrip')
def _():
    return Icon().rect(3.5, 5.5, 13.5, 9.5).vline(6.5, 6, 9).vline(10.5, 6, 9).hline(11.5, 4, 7)


@glyph('TabStripItem')
def _():
    return Icon().rect(4.5, 5.5, 12.5, 9.5).hline(11.5, 4, 13)


@glyph('BreadcrumbBar')
def _():
    icon = Icon().hline(C, 3, 6).hline(C, 9, 12)
    chevron_right(icon, 7.5, C, 1)
    return chevron_right(icon, 13.5, C, 1)


@glyph('ProgressBar')
def _():
    return Icon().rect(3.5, 6.5, 13.5, 10.5).vline(C, 7, 10)


@glyph('Spinner')
def _():
    return Icon().raw('M8.5 3.5A5 5 0 1 1 3.5 8.5')


@glyph('RefreshContainer')
def _():
    icon = Icon().rect(3.5, 6.5, 13.5, 13.5).vline(C, 3, 5)
    return chevron_down(icon, C, 5.5, 1)


@glyph('PathIcon')
def _():
    return Icon().raw('M3.5 11.5C5.5 5.5 8.5 11.5 13.5 4.5')


@glyph('NotificationCard')
def _():
    return Icon().rect(3.5, 4.5, 13.5, 12.5).vline(5.5, 5, 12).hline(7.5, 7, 12).hline(9.5, 7, 11)


@glyph('WindowNotificationManager')
def _():
    return frame().rect(8.5, 5.5, 12.5, 8.5)


@glyph('TitleBar')
def _():
    return Icon().rect(3.5, 4.5, 13.5, 12.5).hline(7.5, 4, 13).dot(10.5, 5.5).dot(12.5, 5.5)


@glyph('ToolBar')
def _():
    return Icon().hline(4.5, 3, 14).rect(3.5, 7.5, 6.5, 10.5).rect(10.5, 7.5, 13.5, 10.5)


@glyph('ToolWindow')
def _():
    return frame().vline(6.5, 4, 13).hline(6.5, 7, 13)


@glyph('WindowControls')
def _():
    return (Icon()
            .hline(C, 3, 5)
            .rect(6.5, 6.5, 10.5, 10.5)
            .diag(11.5, 7.5, 13.5, 9.5).diag(13.5, 7.5, 11.5, 9.5))


@glyph('Badge')
def _():
    return Icon().rect(3.5, 7.5, 9.5, 13.5).circle(11.5, 5.5, 2)


@glyph('Banner')
def _():
    return bar().circle(5.5, C, 1).hline(C, 8, 13)


@glyph('Card')
def _():
    return frame().hline(7.5, 4, 13).hline(10.5, 5, 12)


@glyph('Avatar')
def _():
    return frame().circle(C, 7.5, 2).raw('M5.5 13V12.5A3 3 0 0 1 11.5 12.5V13')


@glyph('Dialog')
def _():
    return frame().hline(6.5, 4, 13).hline(10.5, 7, 9).hline(10.5, 10, 12)


@glyph('GroupHeader')
def _():
    return Icon().rect(3.5, 6.5, 7.5, 10.5).hline(C, 9, 14)


@glyph('CodeBlock')
def _():
    icon = frame()
    chevron_left(icon, 4.5, C, 2)
    return chevron_right(icon, 12.5, C, 2)


def build_all():
    """Все иконки: (семейство, имя, путь, залито)."""
    for name, build in ACTIONS.items():
        icon = build()
        yield 'actions', name, icon.d(), icon.filled

    for name, build in TOOLBOX.items():
        icon = build()
        yield 'toolbox', name, icon.d(), icon.filled
