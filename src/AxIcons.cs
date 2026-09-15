using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace ArxisStudio.Icons;

/// <summary>
/// Набор иконок студии: контурные пути в системе координат 16×16 для
/// <see cref="AxIcon"/>. Плагин может пользоваться этими же путями или дать свои.
/// </summary>
/// <remarks>
/// Путь разбирается при первом обращении к иконке, а не при загрузке типа: экрану нужны
/// единицы иконок из набора, и разбирать остальные до первого кадра незачем.
/// </remarks>
public static class AxIcons
{
    /// <summary>Пути набора, нарисованные силуэтом: их <see cref="AxIcon"/> заливает сам.</summary>
    private static readonly ConditionalWeakTable<Geometry, object> Silhouettes = new();

    /// <summary>Нарисован ли путь набора силуэтом, а не контуром.</summary>
    /// <param name="icon">Путь; чужой путь набору не принадлежит, и ответ для него — нет.</param>
    internal static bool IsFilled(Geometry? icon) => icon is not null && Silhouettes.TryGetValue(icon, out _);

    private static Geometry P(string path) => Geometry.Parse(path);

    private static Geometry Filled(Geometry icon)
    {
        Silhouettes.AddOrUpdate(icon, icon);

        return icon;
    }

    // <иконки:actions> — собрано tools/build.py из icons/actions, руками не править
    /// <summary>Ветка репозитория.</summary>
    public static Geometry Branch { get => field ??= P("M3.5 11.5A2 2 0 1 1 7.5 11.5A2 2 0 1 1 3.5 11.5M9.5 4.5A2 2 0 1 1 13.5 4.5A2 2 0 1 1 9.5 4.5M5.5 10V7.5A3 3 0 0 1 8.5 4.5H9"); }

    /// <summary>Диалог (чат, сообщество).</summary>
    public static Geometry Bubble { get => field ??= P("M3.5 3.5L13.5 3.5L13.5 10.5L8.5 10.5L5.5 13.5L5.5 10.5L3.5 10.5Z"); }

    /// <summary>Галочка.</summary>
    public static Geometry Check { get => field ??= P("M3.5 8.5L6.5 11.5L12.5 5.5"); }

    /// <summary>Шеврон вниз.</summary>
    public static Geometry ChevronDown { get => field ??= P("M4.5 6.5L8.5 10.5L12.5 6.5"); }

    /// <summary>Шеврон вправо.</summary>
    public static Geometry ChevronRight { get => field ??= P("M6.5 4.5L10.5 8.5L6.5 12.5"); }

    /// <summary>Крестик: закрыть, сбросить.</summary>
    public static Geometry Close { get => field ??= P("M4.5 4.5L12.5 12.5M12.5 4.5L4.5 12.5"); }

    /// <summary>Панели дашборда.</summary>
    public static Geometry Dashboard { get => field ??= P("M3.5 3.5H7.5V8.5H3.5ZM9.5 3.5H13.5V6.5H9.5ZM3.5 10.5H7.5V13.5H3.5ZM9.5 8.5H13.5V13.5H9.5Z"); }

    /// <summary>Таблица данных.</summary>
    public static Geometry DataGrid { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM4 6.5H13M4 10.5H13M8.5 7V13"); }

    /// <summary>Документ.</summary>
    public static Geometry Document { get => field ??= P("M4.5 3.5L9.5 3.5L12.5 6.5L12.5 13.5L4.5 13.5ZM9.5 3.5L9.5 6.5L12.5 6.5"); }

    /// <summary>Стрелка загрузки.</summary>
    public static Geometry Download { get => field ??= P("M8.5 3V10M4 13.5H13M5.5 7.5L8.5 10.5L11.5 7.5"); }

    /// <summary>Папка.</summary>
    public static Geometry Folder { get => field ??= P("M3.5 12.5L3.5 4.5L7.5 4.5L9.5 6.5L13.5 6.5L13.5 12.5Z"); }

    /// <summary>Сетка (Grid).</summary>
    public static Geometry Grid { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM8.5 4V13M4 8.5H13"); }

    /// <summary>Изображение.</summary>
    public static Geometry Image { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM3.5 11.5L6.5 8.5L9.5 11.5L11.5 9.5L13.5 11.5M10 6.5H11"); }

    /// <summary>Список.</summary>
    public static Geometry List { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM6 6.5H11M6 8.5H11M6 10.5H11"); }

    /// <summary>Медиа: кадр с треугольником.</summary>
    public static Geometry Media { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM6.5 5.5L9.5 8.5L6.5 11.5Z"); }

    /// <summary>Куб (пакет, плагин).</summary>
    public static Geometry Package { get => field ??= P("M8.5 3.5L12.5 5.5L12.5 11.5L8.5 13.5L4.5 11.5L4.5 5.5ZM4.5 5.5L8.5 7.5L12.5 5.5M8.5 8V13"); }

    /// <summary>Треугольник воспроизведения.</summary>
    public static Geometry Play { get => field ??= Filled(P("M6 4L12 8L6 12Z")); }

    /// <summary>Плюс.</summary>
    public static Geometry Plus { get => field ??= P("M8.5 4V13M4 8.5H13"); }

    /// <summary>Лупа поиска.</summary>
    public static Geometry Search { get => field ??= P("M3.5 7.5A4 4 0 1 1 11.5 7.5A4 4 0 1 1 3.5 7.5M10.5 10.5L13.5 13.5"); }

    /// <summary>Ползунки настроек.</summary>
    public static Geometry Settings { get => field ??= P("M4 5.5H13M4.5 5.5A2 2 0 1 1 8.5 5.5A2 2 0 1 1 4.5 5.5M4 11.5H13M8.5 11.5A2 2 0 1 1 12.5 11.5A2 2 0 1 1 8.5 11.5"); }

    /// <summary>Вкладки.</summary>
    public static Geometry Tabs { get => field ??= P("M3.5 6.5H13.5V13.5H3.5ZM3.5 6.5L3.5 3.5L8.5 3.5L8.5 6.5"); }

    /// <summary>Шаблоны: рамка с точкой.</summary>
    public static Geometry Template { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM6.5 7.5A2 2 0 1 1 10.5 7.5A2 2 0 1 1 6.5 7.5"); }

    /// <summary>Предупреждение: круг с восклицательным знаком.</summary>
    public static Geometry Warning { get => field ??= P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M8.5 5V9M8 11.5H9"); }

    /// <summary>Окно приложения.</summary>
    public static Geometry Window { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM4 6.5H13"); }


    // Действия
    /// <summary>Два листа: копировать.</summary>
    public static Geometry Copy { get => field ??= P("M6.5 6.5H13.5V13.5H6.5ZM11 3.5L3.5 3.5L3.5 11"); }

    /// <summary>Карандаш: переименовать, изменить.</summary>
    public static Geometry Edit { get => field ??= P("M11.5 3.5L13.5 5.5L6.5 12.5L4.5 12.5L4.5 10.5ZM9.5 5.5L11.5 7.5"); }

    /// <summary>Минус: убрать, уменьшить.</summary>
    public static Geometry Minus { get => field ??= P("M4 8.5H13"); }

    /// <summary>Вернуть отменённую правку.</summary>
    public static Geometry Redo { get => field ??= P("M10.5 3.5L13.5 6.5L10.5 9.5M13 6.5H6.5A3 3 0 0 0 6.5 12.5H9"); }

    /// <summary>Обновить, перечитать.</summary>
    public static Geometry Refresh { get => field ??= P("M13.5 8.5A5 5 0 1 1 12.5 5.5L13.5 6.5M13.5 3L13.5 6.5L10 6.5"); }

    /// <summary>Корзина: удалить.</summary>
    public static Geometry Trash { get => field ??= P("M4 4.5H13M6.5 4L6.5 3.5L10.5 3.5L10.5 4M4.5 5L4.5 13.5L12.5 13.5L12.5 5M7.5 7V12M9.5 7V12"); }

    /// <summary>Отменить последнюю правку.</summary>
    public static Geometry Undo { get => field ??= P("M6.5 3.5L3.5 6.5L6.5 9.5M4 6.5H10.5A3 3 0 0 1 10.5 12.5H8"); }


    // Дизайнер форм
    /// <summary>Выровнять по левому краю.</summary>
    public static Geometry AlignLeft { get => field ??= P("M3.5 3V14M5 6.5H12M5 8.5H14M5 10.5H10"); }

    /// <summary>Звенья: привязка данных.</summary>
    public static Geometry Binding { get => field ??= P("M9.5 6.5L10.5 5.5A2 2 0 0 1 13.5 8.5L12.5 9.5M7.5 10.5L6.5 11.5A2 2 0 0 1 3.5 8.5L4.5 7.5M6.5 10.5L10.5 6.5"); }

    /// <summary>Ромб-компонент.</summary>
    public static Geometry Component { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM6.5 6.5H10.5V10.5H6.5Z"); }

    /// <summary>Вписать в экран.</summary>
    public static Geometry FitToScreen { get => field ??= P("M3.5 7L3.5 3.5L7 3.5M13.5 7L13.5 3.5L10 3.5M3.5 10L3.5 13.5L7 13.5M13.5 10L13.5 13.5L10 13.5"); }

    /// <summary>Линии сетки холста с привязкой.</summary>
    public static Geometry GridSnap { get => field ??= P("M3 6.5H14M3 10.5H14M6.5 3V14M10.5 3V14"); }

    /// <summary>Слои.</summary>
    public static Geometry Layers { get => field ??= P("M8.5 3.5L13.5 6.5L8.5 9.5L3.5 6.5ZM3.5 9.5L8.5 12.5L13.5 9.5"); }

    /// <summary>Закрытый замок: слой заблокирован.</summary>
    public static Geometry Lock { get => field ??= P("M4.5 7.5H12.5V12.5H4.5ZM6.5 8V6.5A2 2 0 0 1 10.5 6.5V8M8.5 9V11"); }

    /// <summary>Стрелки во все стороны: перемещение.</summary>
    public static Geometry Move { get => field ??= P("M8.5 3V14M3 8.5H14M6.5 5.5L8.5 3.5L10.5 5.5M6.5 11.5L8.5 13.5L10.5 11.5M5.5 6.5L3.5 8.5L5.5 10.5M11.5 6.5L13.5 8.5L11.5 10.5"); }

    /// <summary>Курсор-указатель: инструмент выбора.</summary>
    public static Geometry Pointer { get => field ??= P("M4.5 3.5L4.5 12.5L6.5 10.5L8.5 13.5L10.5 12.5L8.5 9.5L11.5 9.5Z"); }

    /// <summary>Глаз: предпросмотр.</summary>
    public static Geometry Preview { get => field ??= P("M3.5 8.5C5.5 5.5 7 4.5 8.5 4.5C10 4.5 11.5 5.5 13.5 8.5C11.5 11.5 10 12.5 8.5 12.5C7 12.5 5.5 11.5 3.5 8.5ZM6.5 8.5A2 2 0 1 1 10.5 8.5A2 2 0 1 1 6.5 8.5"); }

    /// <summary>Перечёркнутый глаз: предпросмотр выключен.</summary>
    public static Geometry PreviewOff { get => field ??= P("M3.5 8.5C5.5 5.5 7 4.5 8.5 4.5C10 4.5 11.5 5.5 13.5 8.5C11.5 11.5 10 12.5 8.5 12.5C7 12.5 5.5 11.5 3.5 8.5ZM3.5 13.5L13.5 3.5"); }

    /// <summary>Разделённый вид: дизайн и разметка рядом.</summary>
    public static Geometry Split { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM8.5 4V13"); }

    /// <summary>Литера T: текст.</summary>
    public static Geometry Text { get => field ??= P("M4 4.5H13M8.5 5V12M6 12.5H11"); }

    /// <summary>Открытый замок.</summary>
    public static Geometry Unlock { get => field ??= P("M4.5 7.5H12.5V12.5H4.5ZM6.5 8V5.5A2 2 0 0 1 10.5 5.5V6M8.5 9V11"); }


    // Запуск и отладка
    /// <summary>Жук: отладка.</summary>
    public static Geometry Debug { get => field ??= P("M6.5 4.5L7.5 5.5M3 8.5H6M5.5 11.5L4.5 12.5M10.5 4.5L9.5 5.5M14 8.5H11M11.5 11.5L12.5 12.5M6.5 7.5H10.5V10.5A2 2 0 0 1 8.5 12.5A2 2 0 0 1 6.5 10.5Z"); }

    /// <summary>Молния по кругу: горячая перезагрузка.</summary>
    public static Geometry HotReload { get => field ??= P("M9.5 3.5L5.5 8.5L8.5 8.5L7.5 13.5L11.5 8.5L8.5 8.5Z"); }

    /// <summary>Пауза. IsFilled.</summary>
    public static Geometry Pause { get => field ??= Filled(P("M5 4H7V12H5ZM10 4H12V12H10Z")); }

    /// <summary>Квадрат: остановить. IsFilled.</summary>
    public static Geometry Stop { get => field ??= Filled(P("M5 5H12V12H5Z")); }

    /// <summary>Приглашение консоли.</summary>
    public static Geometry Terminal { get => field ??= P("M3.5 3.5H13.5V13.5H3.5ZM9 11.5H12M5.5 6.5L7.5 8.5L5.5 10.5"); }


    // Окно, тема, прочее
    /// <summary>Стрелка из рамки: внешняя ссылка.</summary>
    public static Geometry ExternalLink { get => field ??= P("M7 3.5L3.5 3.5L3.5 13.5L13.5 13.5L13.5 10M10 3.5L13.5 3.5L13.5 7M13.5 3.5L8.5 8.5"); }

    /// <summary>Шестерёнка: вход в настройки (ползунки остаются за Settings).</summary>
    public static Geometry Gear { get => field ??= P("M8.5 3.5L9.5 3.5L9.5 5.5L10.5 5.5L11.5 4.5L12.5 5.5L11.5 6.5L11.5 7.5L13.5 7.5L13.5 8.5M8.5 13.5L9.5 13.5L9.5 11.5L10.5 11.5L11.5 12.5L12.5 11.5L11.5 10.5L11.5 9.5L13.5 9.5L13.5 8.5M8.5 3.5L7.5 3.5L7.5 5.5L6.5 5.5L5.5 4.5L4.5 5.5L5.5 6.5L5.5 7.5L3.5 7.5L3.5 8.5M8.5 13.5L7.5 13.5L7.5 11.5L6.5 11.5L5.5 12.5L4.5 11.5L5.5 10.5L5.5 9.5L3.5 9.5L3.5 8.5M7.5 8.5A1 1 0 1 1 9.5 8.5A1 1 0 1 1 7.5 8.5"); }

    /// <summary>Клавиатура: шорткаты.</summary>
    public static Geometry Keyboard { get => field ??= P("M3.5 5.5H13.5V11.5H3.5ZM5 7.5H6M7 7.5H8M9 7.5H10M11 7.5H12M6 9.5H11"); }

    /// <summary>Кнопка-булавка: закрепить.</summary>
    public static Geometry Pin { get => field ??= P("M6 3.5H11M6.5 4L6.5 7.5L4.5 9.5L12.5 9.5L10.5 7.5L10.5 4M8.5 10V14"); }

    /// <summary>Звезда: избранное.</summary>
    public static Geometry Star { get => field ??= P("M8.5 2.8L9.85 6.54L13.83 6.67L10.69 9.11L11.79 12.93L8.5 10.7L5.21 12.93L6.31 9.11L3.17 6.67L7.15 6.54Z"); }

    /// <summary>Месяц: тёмная тема.</summary>
    public static Geometry ThemeDark { get => field ??= P("M8.5 3.5A4 4 0 0 0 13.5 8.5A5 5 0 1 1 8.5 3.5Z"); }

    /// <summary>Солнце: светлая тема.</summary>
    public static Geometry ThemeLight { get => field ??= P("M4.5 4.5L5.5 5.5M12.5 4.5L11.5 5.5M4.5 12.5L5.5 11.5M12.5 12.5L11.5 11.5M8.5 3V5M8.5 12V14M3 8.5H5M12 8.5H14M6.5 8.5A2 2 0 1 1 10.5 8.5A2 2 0 1 1 6.5 8.5"); }

    /// <summary>Пользователь.</summary>
    public static Geometry User { get => field ??= P("M6.5 6.5A2 2 0 1 1 10.5 6.5A2 2 0 1 1 6.5 6.5M4.5 14V13.5A4 4 0 0 1 12.5 13.5V14"); }

    /// <summary>Развернуть окно.</summary>
    public static Geometry WindowMaximize { get => field ??= P("M4.5 4.5H12.5V12.5H4.5Z"); }

    /// <summary>Свернуть окно.</summary>
    public static Geometry WindowMinimize { get => field ??= P("M4 8.5H13"); }

    /// <summary>Восстановить окно из развёрнутого.</summary>
    public static Geometry WindowRestore { get => field ??= P("M4.5 6.5H10.5V12.5H4.5ZM6.5 6.5L6.5 4.5L12.5 4.5L12.5 10.5L10.5 10.5"); }


    // Поиск, списки, таблицы
    /// <summary>Воронка: фильтр списка.</summary>
    public static Geometry Filter { get => field ??= P("M3.5 4.5L13.5 4.5L9.5 8.5L9.5 13.5L7.5 13.5L7.5 8.5Z"); }

    /// <summary>Три точки горизонтально: свёрнутое меню. IsFilled.</summary>
    public static Geometry MoreHorizontal { get => field ??= Filled(P("M3 8.5A1.5 1.5 0 1 1 6 8.5A1.5 1.5 0 1 1 3 8.5M7 8.5A1.5 1.5 0 1 1 10 8.5A1.5 1.5 0 1 1 7 8.5M11 8.5A1.5 1.5 0 1 1 14 8.5A1.5 1.5 0 1 1 11 8.5")); }

    /// <summary>Три точки вертикально: меню строки. IsFilled.</summary>
    public static Geometry MoreVertical { get => field ??= Filled(P("M7 4.5A1.5 1.5 0 1 1 10 4.5A1.5 1.5 0 1 1 7 4.5M7 8.5A1.5 1.5 0 1 1 10 8.5A1.5 1.5 0 1 1 7 8.5M7 12.5A1.5 1.5 0 1 1 10 12.5A1.5 1.5 0 1 1 7 12.5")); }

    /// <summary>Стрелки вверх-вниз: сортировка.</summary>
    public static Geometry Sort { get => field ??= P("M5.5 5V13M11.5 4V12M3.5 6.5L5.5 4.5L7.5 6.5M9.5 10.5L11.5 12.5L13.5 10.5"); }

    /// <summary>Лупа с плюсом: приблизить.</summary>
    public static Geometry ZoomIn { get => field ??= P("M3.5 7.5A4 4 0 1 1 11.5 7.5A4 4 0 1 1 3.5 7.5M10.5 10.5L13.5 13.5M7.5 5V10M5 7.5H10"); }

    /// <summary>Лупа с минусом: отдалить.</summary>
    public static Geometry ZoomOut { get => field ??= P("M3.5 7.5A4 4 0 1 1 11.5 7.5A4 4 0 1 1 3.5 7.5M10.5 10.5L13.5 13.5M5 7.5H10"); }


    // Проект и файлы
    /// <summary>Книга: документация.</summary>
    public static Geometry Docs { get => field ??= P("M8.5 4.5C7.5 3.5 6 3.5 3.5 3.5V12.5C6 12.5 7.5 13 8.5 13.5M8.5 4.5C9.5 3.5 11 3.5 13.5 3.5V12.5C11 12.5 9.5 13 8.5 13.5M8.5 5V13"); }

    /// <summary>Лист с угловыми скобками: файл разметки или кода.</summary>
    public static Geometry DocumentCode { get => field ??= P("M4.5 3.5L9.5 3.5L12.5 6.5L12.5 13.5L4.5 13.5ZM9.5 3.5L9.5 6.5L12.5 6.5M7.5 9.5L6.5 10.5L7.5 11.5M9.5 9.5L10.5 10.5L9.5 11.5"); }

    /// <summary>Раскрытая папка.</summary>
    public static Geometry FolderOpen { get => field ??= P("M13.5 8L13.5 6.5L9.5 6.5L7.5 4.5L3.5 4.5L3.5 12.5L12 12.5M3.5 12.5L5.5 8.5L13.5 8.5L11.5 12.5"); }

    /// <summary>Вилка-разъём: плагин.</summary>
    public static Geometry Plugin { get => field ??= P("M6.5 3V6M10.5 3V6M4.5 5.5H12.5V8.5A2 2 0 0 1 10.5 10.5H6.5A2 2 0 0 1 4.5 8.5ZM8.5 11V14"); }

    /// <summary>Структура: иерархия элементов документа.</summary>
    public static Geometry Structure { get => field ??= P("M3 4.5H14M6 8.5H14M9 12.5H14"); }


    // Раскрытие
    /// <summary>Мелкий шеврон вниз для тесной строки (класс small).</summary>
    public static Geometry ChevronDownSmall { get => field ??= P("M5.5 6.5L8.5 9.5L11.5 6.5"); }

    /// <summary>Шеврон влево: назад, свернуть панель.</summary>
    public static Geometry ChevronLeft { get => field ??= P("M10.5 4.5L6.5 8.5L10.5 12.5"); }

    /// <summary>Шеврон вверх: свернуть раскрытое.</summary>
    public static Geometry ChevronUp { get => field ??= P("M4.5 10.5L8.5 6.5L12.5 10.5"); }

    /// <summary>Свернуть всё дерево.</summary>
    public static Geometry CollapseAll { get => field ??= P("M4 3.5H13M4 13.5H13M6.5 5.5L8.5 7.5L10.5 5.5M6.5 11.5L8.5 9.5L10.5 11.5"); }

    /// <summary>Развернуть всё дерево.</summary>
    public static Geometry ExpandAll { get => field ??= P("M4 3.5H13M4 13.5H13M6.5 7.5L8.5 5.5L10.5 7.5M6.5 9.5L8.5 11.5L10.5 9.5"); }


    // Статусы
    /// <summary>Круг с крестом: ошибка.</summary>
    public static Geometry Error { get => field ??= P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M6.5 6.5L10.5 10.5M10.5 6.5L6.5 10.5"); }

    /// <summary>Круг с «i»: сведения.</summary>
    public static Geometry Info { get => field ??= P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M8 5.5H9M8.5 7V12"); }

    /// <summary>Круг с вопросом: справка.</summary>
    public static Geometry Question { get => field ??= P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M6.5 6.5A2 2 0 1 1 8.5 8.5V10M8 11.5H9"); }

    /// <summary>Круг с галочкой: успех.</summary>
    public static Geometry Success { get => field ??= P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M5.5 8.5L7.5 10.5L11.5 6.5"); }

    /// <summary>Треугольник с восклицательным знаком: предупреждение в баннере и списке проблем.</summary>
    public static Geometry WarningTriangle { get => field ??= P("M8.5 3.5L13.5 13.5L3.5 13.5ZM8.5 7V10M8 11.5H9"); }
    // </иконки:actions>
}
