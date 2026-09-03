# -*- coding: utf-8 -*-
"""Конструктор иконок, который не умеет рисовать мимо сетки.

Клетка 16×16, оптический центр — пиксель (8, 8), то есть координата 8.5.
Живая область для середины штриха — 3.5…13.5: одиннадцать пикселей, с 3-го
по 13-й, и центральный из них — восьмой. Нечётная ширина нужна ровно затем,
чтобы у центрального штриха был свой пиксель, а не граница между двумя.

Правила, которые конструктор проверяет при каждом вызове:

* середина осевого штриха стоит на n.5 — так обводка занимает пиксель целиком;
* концы открытого штриха стоят на целых — так плоский срез совпадает с
  границей пикселя, и штрих не тянет за собой полупрозрачный хвост;
* углы замкнутых фигур стоят на n.5 — их закрывает острый стык;
* центр окружности стоит на n.5, радиус целый — крайние точки попадают на
  сетку так же, как прямые;
* у диагоналей концы стоят в центрах пикселей, а наклон — 45°: такая линия
  проходит через центры пикселей и сглаживается симметрично;
* заливка стоит на целых — её край и есть край пикселя.

Симметрия — не свойство чисел, а свойство операций: mirror_x и mirror_y
отражают относительно 8.5, и второй половине неоткуда разойтись с первой.
"""

CENTER = 8.5
EPS = 1e-9


def _num(value):
    text = ('%.4f' % value).rstrip('0').rstrip('.')

    return '0' if text in ('', '-0') else text


def is_half(value):
    return abs((value - 0.5) - round(value - 0.5)) < EPS


def is_whole(value):
    return abs(value - round(value)) < EPS


def half(value, what):
    if not is_half(value):
        raise ValueError('%s: %s должен стоять на полупикселе' % (what, _num(value)))

    return value


def whole(value, what):
    if not is_whole(value):
        raise ValueError('%s: %s должен стоять на целом' % (what, _num(value)))

    return value


class Icon:
    """Один путь: части накапливаются и собираются в атрибут d."""

    def __init__(self, filled=False):
        self.filled = filled
        self.parts = []

    # -- прямые ------------------------------------------------------------

    def hline(self, y, x0, x1):
        """Горизонтальный штрих: высота на полупикселе, концы на целых."""
        if self.filled:
            raise ValueError('hline — для контуров; силуэт рисуется rect/poly')

        half(y, 'hline y')
        whole(x0, 'hline x0')
        whole(x1, 'hline x1')
        self.parts.append('M%s %sH%s' % (_num(x0), _num(y), _num(x1)))

        return self

    def vline(self, x, y0, y1):
        if self.filled:
            raise ValueError('vline — для контуров; силуэт рисуется rect/poly')

        half(x, 'vline x')
        whole(y0, 'vline y0')
        whole(y1, 'vline y1')
        self.parts.append('M%s %sV%s' % (_num(x), _num(y0), _num(y1)))

        return self

    def dot(self, x, y):
        """Точка — штрих длиной в пиксель: у плоского среза нулевой длины нет."""
        return self.hline(y, x - 0.5, x + 0.5)

    def diag(self, x0, y0, x1, y1):
        """Диагональ под 45° из центра пикселя в центр пикселя."""
        for value, what in ((x0, 'diag x0'), (y0, 'diag y0'), (x1, 'diag x1'), (y1, 'diag y1')):
            half(value, what)

        if abs(abs(x1 - x0) - abs(y1 - y0)) > EPS:
            raise ValueError('diag: наклон обязан быть 45°, а тут %s×%s' % (_num(x1 - x0), _num(y1 - y0)))

        self.parts.append('M%s %sL%s %s' % (_num(x0), _num(y0), _num(x1), _num(y1)))

        return self

    def line(self, x0, y0, x1, y1):
        """Произвольный отрезок между центрами пикселей — там, где 45° не выходит."""
        for value, what in ((x0, 'line x0'), (y0, 'line y0'), (x1, 'line x1'), (y1, 'line y1')):
            half(value, what)

        self.parts.append('M%s %sL%s %s' % (_num(x0), _num(y0), _num(x1), _num(y1)))

        return self

    # -- фигуры ------------------------------------------------------------

    def rect(self, x0, y0, x1, y1):
        """Прямоугольник: у контура углы на n.5, у силуэта — на целых."""
        check = whole if self.filled else half

        for value, what in ((x0, 'rect x0'), (y0, 'rect y0'), (x1, 'rect x1'), (y1, 'rect y1')):
            check(value, what)

        self.parts.append('M%s %sH%sV%sH%sZ' % (_num(x0), _num(y0), _num(x1), _num(y1), _num(x0)))

        return self

    def polyline(self, *points):
        """Открытая ломаная: углы на n.5 (их закрывает стык), концы — на целых
        вдоль своего отрезка, чтобы плоский срез лёг на границу пикселя. Конец,
        упирающийся в другой штрих, можно оставить на n.5 — его пиксель закрыт
        тем штрихом."""
        if self.filled:
            raise ValueError('polyline — для контуров')

        if len(points) < 2:
            raise ValueError('polyline: нужны хотя бы две точки')

        for at, (x, y) in enumerate(points[1:-1], start=1):
            half(x, 'polyline x%d' % at)
            half(y, 'polyline y%d' % at)

        for at in (0, len(points) - 1):
            x, y = points[at]
            other = points[1] if at == 0 else points[-2]

            if abs(y - other[1]) < EPS:
                half(y, 'polyline y%d' % at)          # горизонтальный конец
            elif abs(x - other[0]) < EPS:
                half(x, 'polyline x%d' % at)          # вертикальный конец
            else:
                half(x, 'polyline x%d' % at)
                half(y, 'polyline y%d' % at)

            if not (is_half(x) or is_whole(x)) or not (is_half(y) or is_whole(y)):
                raise ValueError('polyline: конец %d стоит мимо сетки' % at)

        text = ''.join(('M' if at == 0 else 'L') + '%s %s' % (_num(x), _num(y)) for at, (x, y) in enumerate(points))
        self.parts.append(text)

        return self

    def poly(self, *points, close=True):
        """Ломаная по центрам пикселей; у силуэта — по целым."""
        check = whole if self.filled else half
        text = []

        for at, (x, y) in enumerate(points):
            check(x, 'poly x%d' % at)
            check(y, 'poly y%d' % at)
            text.append(('M' if at == 0 else 'L') + '%s %s' % (_num(x), _num(y)))

        self.parts.append(''.join(text) + ('Z' if close else ''))

        return self

    def circle(self, cx, cy, r):
        """Окружность: центр на n.5, радиус целый — крайние точки на сетке."""
        half(cx, 'circle cx')
        half(cy, 'circle cy')

        if self.filled:
            # У силуэта крайние точки должны лечь на целые: радиус n.5.
            half(r, 'circle r (силуэт)')
        else:
            whole(r, 'circle r')

        self.parts.append(
            'M%s %sA%s %s 0 1 1 %s %sA%s %s 0 1 1 %s %s' % (
                _num(cx - r), _num(cy), _num(r), _num(r), _num(cx + r), _num(cy),
                _num(r), _num(r), _num(cx - r), _num(cy)))

        return self

    def arc(self, x0, y0, r, x1, y1, large=0, sweep=1):
        """Дуга между центрами пикселей; радиус целый, чтобы касания попадали на сетку."""
        for value, what in ((x0, 'arc x0'), (y0, 'arc y0'), (x1, 'arc x1'), (y1, 'arc y1')):
            half(value, what)

        self.parts.append('M%s %sA%s %s 0 %d %d %s %s' % (
            _num(x0), _num(y0), _num(r), _num(r), large, sweep, _num(x1), _num(y1)))

        return self

    def raw(self, text):
        """Путь как есть — для кривых, которым сетка не поможет. Каждое место — сознательно."""
        self.parts.append(text)

        return self

    # -- симметрия ---------------------------------------------------------

    def mirror_x(self):
        """Добавляет зеркальную копию всего нарисованного относительно x = 8.5."""
        self.parts.extend(_mirror(part, 'x') for part in list(self.parts))

        return self

    def mirror_y(self):
        self.parts.extend(_mirror(part, 'y') for part in list(self.parts))

        return self

    def d(self):
        return ''.join(self.parts)


def _mirror(part, axis):
    """Отражает абсолютный путь относительно центра по одной оси."""
    import re

    out = []
    tokens = re.findall(r'[A-Za-z]|-?\d*\.?\d+', part)
    letter = None
    numbers = []

    def flush():
        if letter is None:
            return

        if letter == 'H':
            out.append(('V' if False else 'H') + ' '.join(_num(2 * CENTER - v if axis == 'x' else v) for v in numbers))
        elif letter == 'V':
            out.append('V' + ' '.join(_num(2 * CENTER - v if axis == 'y' else v) for v in numbers))
        elif letter == 'A':
            # rx ry rot large sweep x y — отражение меняет направление обхода.
            fixed = []

            for at in range(0, len(numbers), 7):
                rx, ry, rot, large, sweep, x, y = numbers[at:at + 7]
                x = 2 * CENTER - x if axis == 'x' else x
                y = 2 * CENTER - y if axis == 'y' else y
                fixed.append(' '.join(_num(v) for v in (rx, ry, rot, large, 1 - sweep, x, y)))

            out.append('A' + 'A'.join(fixed))
        elif letter == 'Z':
            out.append('Z')
        else:
            fixed = []

            for at in range(0, len(numbers), 2):
                x, y = numbers[at], numbers[at + 1]
                x = 2 * CENTER - x if axis == 'x' else x
                y = 2 * CENTER - y if axis == 'y' else y
                fixed.append('%s %s' % (_num(x), _num(y)))

            out.append(letter + ' '.join(fixed))

    for token in tokens:
        if token.isalpha():
            flush()
            letter = token
            numbers = []
        else:
            numbers.append(float(token))

    flush()

    return ''.join(out)
