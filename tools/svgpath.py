# -*- coding: utf-8 -*-
"""Разбор и сборка атрибута d: команды, точки и обратно в строку.

Отдельно от нормализатора, потому что разбор пути нужен и ему, и проверкам:
одно место, где записано, как читается путь, — одно место, где это может быть
неверно.
"""
import re

TOKEN = re.compile(r'[MmLlHhVvCcSsQqTtAaZz]|-?\d*\.?\d+(?:[eE][-+]?\d+)?')

# Сколько чисел забирает команда и какие из них — координаты точки.
ARITY = {
    'M': 2, 'L': 2, 'T': 2,
    'H': 1, 'V': 1,
    'C': 6, 'S': 4, 'Q': 4,
    'A': 7,
    'Z': 0,
}


def tokens(text):
    return TOKEN.findall(text)


def parse(text):
    """Разбирает путь в список команд: (буква, [числа]).

    Повторяющиеся наборы чисел после одной буквы разворачиваются в отдельные
    команды — так дальше не надо помнить, что «M x y x y» это M и L.
    """
    found = []
    letter = None
    numbers = []

    for token in tokens(text):
        if token.isalpha():
            if letter is not None:
                found.extend(split(letter, numbers))

            letter = token
            numbers = []
        else:
            numbers.append(float(token))

    if letter is not None:
        found.extend(split(letter, numbers))

    return found


def split(letter, numbers):
    """Разворачивает повторы: у M второй набор — это L, у остальных та же буква."""
    size = ARITY[letter.upper()]

    if size == 0:
        return [(letter, [])]

    found = []
    at = 0

    while at + size <= len(numbers):
        current = letter

        if found and letter in 'Mm':
            current = 'L' if letter == 'M' else 'l'

        found.append((current, numbers[at:at + size]))
        at += size

    return found


def absolute(commands):
    """Переводит путь в абсолютные команды: так точки видно, а форма та же."""
    found = []
    x = y = 0.0
    start = (0.0, 0.0)

    for letter, numbers in commands:
        upper = letter.upper()
        relative = letter.islower()

        if upper == 'Z':
            found.append(('Z', []))
            x, y = start
            continue

        values = list(numbers)

        if upper == 'H':
            values = [values[0] + x] if relative else values
            x = values[0]
            found.append(('H', values))
            continue

        if upper == 'V':
            values = [values[0] + y] if relative else values
            y = values[0]
            found.append(('V', values))
            continue

        if upper == 'A':
            if relative:
                values[5] += x
                values[6] += y

            x, y = values[5], values[6]
            found.append(('A', values))
            continue

        if relative:
            for at in range(0, len(values), 2):
                values[at] += x
                values[at + 1] += y

        x, y = values[-2], values[-1]
        found.append((upper, values))

        if upper == 'M':
            start = (x, y)

    return found


def points(commands):
    """Точки пути в порядке следования: (индекс команды, x, y).

    Только конечные точки команд — управляющие точки кривых не трогаем, чтобы
    не менять их натяжение.
    """
    found = []
    x = y = 0.0

    for at, (letter, numbers) in enumerate(commands):
        if letter == 'Z':
            continue

        if letter == 'H':
            x = numbers[0]
        elif letter == 'V':
            y = numbers[0]
        else:
            x, y = numbers[-2], numbers[-1]

        found.append((at, x, y))

    return found


def number(value):
    """Печатает число коротко: без хвостовых нулей, но без потери точности."""
    text = ('%.4f' % value).rstrip('0').rstrip('.')

    return '0' if text in ('', '-0') else text


def render(commands):
    """Собирает путь обратно в строку."""
    parts = []

    for letter, numbers in commands:
        parts.append(letter + ' '.join(number(value) for value in numbers))

    return ''.join(parts)
