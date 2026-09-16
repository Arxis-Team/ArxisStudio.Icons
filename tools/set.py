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


def badge():
    """Значок состояния: силуэт, а не контур.

    Контур кружка в шестнадцать точек не ложится в пиксели ни при каком масштабе: обводка идёт по
    дуге, и попасть в границу пикселя она может лишь в четырёх касаниях. Замер по набору давал
    десятую долю сплошных против единицы у прямых штрихов — значок читался мутным пятном рядом с
    чёткой полосой инструментов. Силуэт же сплошной внутри, и сглаживание достаётся только ободу.

    Так нарисованы значки состояний в Rider, Visual Studio Code и консоли Unity: залитый кружок с
    вырубленным знаком. Вырубка — второй подпуть: чётное-нечётное правило заливки делает из него
    дыру, сквозь которую виден фон.
    """
    return Icon(filled=True).circle(C, C, 5.5)


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
    """Венец из восьми зубьев вокруг ступицы: четыре по осям, четыре наискось.

    Зуб — часть силуэта, а не чёрточка рядом с кольцом: кольцо с лучами это
    ThemeLight, то есть солнце. Ломаная — четверть венца, от середины верхнего
    зуба до середины правого; режется по середине зуба, а не по углу, чтобы
    плоские срезы сошлись поперёк штриха без щербины.
    """
    icon = Icon().polyline(
        (C, 3.5), (9.5, 3.5), (9.5, 5.5), (10.5, 5.5),
        (11.5, 4.5), (12.5, 5.5), (11.5, 6.5), (11.5, 7.5),
        (13.5, 7.5), (13.5, C))
    icon.mirror_y().mirror_x()

    # Ступица — после отражений: иначе их было бы четыре одна в другой.
    return icon.circle(C, C, 1)


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
    """Жук силуэтом: голова, спинка со швом и по две лапки на сторону.

    Контурный жук на шестнадцати точках рассыпался: усики и шесть лапок — это восемь штрихов
    длиной в два пикселя, и каждый из них растворялся в сглаживании. Силуэт держит форму, а
    спинку делит вырубленный шов — он же отличает жука от капли.

    Лапки стоят там, где тело широкое, и краями упираются в него: заезжай они внутрь, чётное-
    нечётное правило заливки вырезало бы на месте пересечения дыру.

    Усиков нет намеренно: на шестнадцати точках они отрывались от головы — круг в этом месте уже
    сошёлся в точку, — и читались двумя случайными крапинами над жуком.
    """
    icon = (Icon(filled=True)
            .rect(3, 8, 5, 9)
            .rect(3, 10, 5, 11)
            .mirror_x())

    # Голова и спинка симметричны сами по себе и рисуются после отражения.
    return (icon
            .circle(C, 5.5, 1.5)
            .circle(C, 9.5, 3.5)
            .rect(8, 7, 9, 12))


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
    # Косой крест вырубается сырым путём: у его развилок координаты неизбежно половинные —
    # две полосы под 45° пересекаются посередине клетки, — а силуэт грамматика держит на целых.
    # Наклон при этом ровно 45°, и пересчёт под другую клетку его сохраняет.
    return badge().raw('M6 7L7 6L8.5 7.5L10 6L11 7L9.5 8.5L11 10L10 11L8.5 9.5L7 11L6 10L7.5 8.5Z')


@action('Success')
def _():
    # Галочка — та же полоса под 45°, и развилки у неё такие же половинные.
    return badge().raw('M6 8.5L7.5 10L11 6.5L12 7.5L7.5 12L5 9.5Z')


@action('Warning')
def _():
    return badge().rect(8, 5, 9, 10).rect(8, 11, 9, 12)


@action('Info')
def _():
    return badge().rect(8, 5, 9, 6).rect(8, 7, 9, 12)


@action('Question')
def _():
    # Крюк вопроса — кольцо с вырезом: внешняя дуга сверху, внутренняя обратно, и хвост вниз к
    # точке. Кривая здесь неизбежна, зато лежит она внутри силуэта, а не отвечает за его край.
    return (badge()
            .raw('M6 7.5A2.5 2.5 0 0 1 11 7.5A2.5 2.5 0 0 1 9 9.94V11H8V9A1 1 0 0 0 10 7.5'
                 'A1.5 1.5 0 0 0 7 7.5Z')
            .rect(8, 12, 9, 13))


@action('WarningTriangle')
def _():
    # Вершина срезана на пиксель: острие ровно посередине клетки пришлось бы на половинную
    # координату, а силуэт стоит на целых. Срез в один пиксель на экране не виден.
    return (Icon(filled=True)
            .poly((8, 3), (9, 3), (14, 14), (3, 14))
            .rect(8, 7, 9, 11)
            .rect(8, 12, 9, 13))


@action('ThemeDark')
def _():
    return Icon().raw('M8.5 3.5A4 4 0 0 0 13.5 8.5A5 5 0 1 1 8.5 3.5Z')


@action('ThemeLight')
def _():
    """Солнце: восемь лучей вокруг круга.

    Отражение удваивает всё, что стоит на его оси, и заводить луч сбоку оно не
    умеет: круг с верхним лучом, отражённые дважды, давали четыре круга, два
    верхних луча и два нижних — а боковых не было вовсе. Поэтому наискось —
    отражением, а по осям и круг — по одному разу.
    """
    icon = Icon().diag(4.5, 4.5, 5.5, 5.5).mirror_x().mirror_y()
    icon.vline(C, 3, 5).vline(C, 12, 14).hline(C, 3, 5).hline(C, 12, 14)

    return icon.circle(C, C, 2)


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


def build_all():
    """Все иконки: (семейство, имя, путь, залито)."""
    for name, build in ACTIONS.items():
        icon = build()
        yield 'actions', name, icon.d(), icon.filled

