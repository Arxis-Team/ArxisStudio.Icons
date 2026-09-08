using Avalonia.Media;

namespace ArxisStudio.Icons;

/// <summary>
/// Набор иконок студии: контурные пути в системе координат 16×16 для
/// <see cref="AxIcon"/>. Плагин может пользоваться этими же путями или дать свои.
/// </summary>
public static class AxIcons
{
    private static Geometry P(string path) => Geometry.Parse(path);

    // <иконки:actions> — собрано tools/build.py из icons/actions, руками не править
    /// <summary>Ветка репозитория.</summary>
    public static Geometry Branch { get; } = P("M3.5 11.5A2 2 0 1 1 7.5 11.5A2 2 0 1 1 3.5 11.5M9.5 4.5A2 2 0 1 1 13.5 4.5A2 2 0 1 1 9.5 4.5M5.5 10V7.5A3 3 0 0 1 8.5 4.5H9");

    /// <summary>Диалог (чат, сообщество).</summary>
    public static Geometry Bubble { get; } = P("M3.5 3.5L13.5 3.5L13.5 10.5L8.5 10.5L5.5 13.5L5.5 10.5L3.5 10.5Z");

    /// <summary>Галочка.</summary>
    public static Geometry Check { get; } = P("M3.5 8.5L6.5 11.5L12.5 5.5");

    /// <summary>Шеврон вниз.</summary>
    public static Geometry ChevronDown { get; } = P("M4.5 6.5L8.5 10.5L12.5 6.5");

    /// <summary>Шеврон вправо.</summary>
    public static Geometry ChevronRight { get; } = P("M6.5 4.5L10.5 8.5L6.5 12.5");

    /// <summary>Крестик: закрыть, сбросить.</summary>
    public static Geometry Close { get; } = P("M4.5 4.5L12.5 12.5M12.5 4.5L4.5 12.5");

    /// <summary>Панели дашборда.</summary>
    public static Geometry Dashboard { get; } = P("M3.5 3.5H7.5V8.5H3.5ZM9.5 3.5H13.5V6.5H9.5ZM3.5 10.5H7.5V13.5H3.5ZM9.5 8.5H13.5V13.5H9.5Z");

    /// <summary>Таблица данных.</summary>
    public static Geometry DataGrid { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM4 6.5H13M4 10.5H13M8.5 7V13");

    /// <summary>Документ.</summary>
    public static Geometry Document { get; } = P("M4.5 3.5L9.5 3.5L12.5 6.5L12.5 13.5L4.5 13.5ZM9.5 3.5L9.5 6.5L12.5 6.5");

    /// <summary>Стрелка загрузки.</summary>
    public static Geometry Download { get; } = P("M8.5 3V10M4 13.5H13M5.5 7.5L8.5 10.5L11.5 7.5");

    /// <summary>Папка.</summary>
    public static Geometry Folder { get; } = P("M3.5 12.5L3.5 4.5L7.5 4.5L9.5 6.5L13.5 6.5L13.5 12.5Z");

    /// <summary>Сетка (Grid).</summary>
    public static Geometry Grid { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM8.5 4V13M4 8.5H13");

    /// <summary>Изображение.</summary>
    public static Geometry Image { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM3.5 11.5L6.5 8.5L9.5 11.5L11.5 9.5L13.5 11.5M10 6.5H11");

    /// <summary>Список.</summary>
    public static Geometry List { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6 6.5H11M6 8.5H11M6 10.5H11");

    /// <summary>Медиа: кадр с треугольником.</summary>
    public static Geometry Media { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6.5 5.5L9.5 8.5L6.5 11.5Z");

    /// <summary>Куб (пакет, плагин).</summary>
    public static Geometry Package { get; } = P("M8.5 3.5L12.5 5.5L12.5 11.5L8.5 13.5L4.5 11.5L4.5 5.5ZM4.5 5.5L8.5 7.5L12.5 5.5M8.5 8V13");

    /// <summary>Треугольник воспроизведения.</summary>
    public static Geometry Play { get; } = P("M6 4L12 8L6 12Z");

    /// <summary>Плюс.</summary>
    public static Geometry Plus { get; } = P("M8.5 4V13M4 8.5H13");

    /// <summary>Лупа поиска.</summary>
    public static Geometry Search { get; } = P("M3.5 7.5A4 4 0 1 1 11.5 7.5A4 4 0 1 1 3.5 7.5M10.5 10.5L13.5 13.5");

    /// <summary>Ползунки настроек.</summary>
    public static Geometry Settings { get; } = P("M4 5.5H13M4.5 5.5A2 2 0 1 1 8.5 5.5A2 2 0 1 1 4.5 5.5M4 11.5H13M8.5 11.5A2 2 0 1 1 12.5 11.5A2 2 0 1 1 8.5 11.5");

    /// <summary>Вкладки.</summary>
    public static Geometry Tabs { get; } = P("M3.5 6.5H13.5V13.5H3.5ZM3.5 6.5L3.5 3.5L8.5 3.5L8.5 6.5");

    /// <summary>Шаблоны: рамка с точкой.</summary>
    public static Geometry Template { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6.5 7.5A2 2 0 1 1 10.5 7.5A2 2 0 1 1 6.5 7.5");

    /// <summary>Предупреждение: круг с восклицательным знаком.</summary>
    public static Geometry Warning { get; } = P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M8.5 5V9M8 11.5H9");

    /// <summary>Окно приложения.</summary>
    public static Geometry Window { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM4 6.5H13");


    // Действия
    /// <summary>Два листа: копировать.</summary>
    public static Geometry Copy { get; } = P("M6.5 6.5H13.5V13.5H6.5ZM11 3.5L3.5 3.5L3.5 11");

    /// <summary>Карандаш: переименовать, изменить.</summary>
    public static Geometry Edit { get; } = P("M11.5 3.5L13.5 5.5L6.5 12.5L4.5 12.5L4.5 10.5ZM9.5 5.5L11.5 7.5");

    /// <summary>Минус: убрать, уменьшить.</summary>
    public static Geometry Minus { get; } = P("M4 8.5H13");

    /// <summary>Вернуть отменённую правку.</summary>
    public static Geometry Redo { get; } = P("M10.5 3.5L13.5 6.5L10.5 9.5M13 6.5H6.5A3 3 0 0 0 6.5 12.5H9");

    /// <summary>Обновить, перечитать.</summary>
    public static Geometry Refresh { get; } = P("M13.5 8.5A5 5 0 1 1 12.5 5.5L13.5 6.5M13.5 3L13.5 6.5L10 6.5");

    /// <summary>Корзина: удалить.</summary>
    public static Geometry Trash { get; } = P("M4 4.5H13M6.5 4L6.5 3.5L10.5 3.5L10.5 4M4.5 5L4.5 13.5L12.5 13.5L12.5 5M7.5 7V12M9.5 7V12");

    /// <summary>Отменить последнюю правку.</summary>
    public static Geometry Undo { get; } = P("M6.5 3.5L3.5 6.5L6.5 9.5M4 6.5H10.5A3 3 0 0 1 10.5 12.5H8");


    // Дизайнер форм
    /// <summary>Выровнять по левому краю.</summary>
    public static Geometry AlignLeft { get; } = P("M3.5 3V14M5 6.5H12M5 8.5H14M5 10.5H10");

    /// <summary>Звенья: привязка данных.</summary>
    public static Geometry Binding { get; } = P("M9.5 6.5L10.5 5.5A2 2 0 0 1 13.5 8.5L12.5 9.5M7.5 10.5L6.5 11.5A2 2 0 0 1 3.5 8.5L4.5 7.5M6.5 10.5L10.5 6.5");

    /// <summary>Ромб-компонент.</summary>
    public static Geometry Component { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6.5 6.5H10.5V10.5H6.5Z");

    /// <summary>Вписать в экран.</summary>
    public static Geometry FitToScreen { get; } = P("M3.5 7L3.5 3.5L7 3.5M13.5 7L13.5 3.5L10 3.5M3.5 10L3.5 13.5L7 13.5M13.5 10L13.5 13.5L10 13.5");

    /// <summary>Линии сетки холста с привязкой.</summary>
    public static Geometry GridSnap { get; } = P("M3 6.5H14M3 10.5H14M6.5 3V14M10.5 3V14");

    /// <summary>Слои.</summary>
    public static Geometry Layers { get; } = P("M8.5 3.5L13.5 6.5L8.5 9.5L3.5 6.5ZM3.5 9.5L8.5 12.5L13.5 9.5");

    /// <summary>Закрытый замок: слой заблокирован.</summary>
    public static Geometry Lock { get; } = P("M4.5 7.5H12.5V12.5H4.5ZM6.5 8V6.5A2 2 0 0 1 10.5 6.5V8M8.5 9V11");

    /// <summary>Стрелки во все стороны: перемещение.</summary>
    public static Geometry Move { get; } = P("M8.5 3V14M3 8.5H14M6.5 5.5L8.5 3.5L10.5 5.5M6.5 11.5L8.5 13.5L10.5 11.5M5.5 6.5L3.5 8.5L5.5 10.5M11.5 6.5L13.5 8.5L11.5 10.5");

    /// <summary>Курсор-указатель: инструмент выбора.</summary>
    public static Geometry Pointer { get; } = P("M4.5 3.5L4.5 12.5L6.5 10.5L8.5 13.5L10.5 12.5L8.5 9.5L11.5 9.5Z");

    /// <summary>Глаз: предпросмотр.</summary>
    public static Geometry Preview { get; } = P("M3.5 8.5C5.5 5.5 7 4.5 8.5 4.5C10 4.5 11.5 5.5 13.5 8.5C11.5 11.5 10 12.5 8.5 12.5C7 12.5 5.5 11.5 3.5 8.5ZM6.5 8.5A2 2 0 1 1 10.5 8.5A2 2 0 1 1 6.5 8.5");

    /// <summary>Перечёркнутый глаз: предпросмотр выключен.</summary>
    public static Geometry PreviewOff { get; } = P("M3.5 8.5C5.5 5.5 7 4.5 8.5 4.5C10 4.5 11.5 5.5 13.5 8.5C11.5 11.5 10 12.5 8.5 12.5C7 12.5 5.5 11.5 3.5 8.5ZM3.5 13.5L13.5 3.5");

    /// <summary>Разделённый вид: дизайн и разметка рядом.</summary>
    public static Geometry Split { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM8.5 4V13");

    /// <summary>Литера T: текст.</summary>
    public static Geometry Text { get; } = P("M4 4.5H13M8.5 5V12M6 12.5H11");

    /// <summary>Открытый замок.</summary>
    public static Geometry Unlock { get; } = P("M4.5 7.5H12.5V12.5H4.5ZM6.5 8V5.5A2 2 0 0 1 10.5 5.5V6M8.5 9V11");


    // Запуск и отладка
    /// <summary>Жук: отладка.</summary>
    public static Geometry Debug { get; } = P("M6.5 7.5H10.5V10.5A2 2 0 0 1 8.5 12.5A2 2 0 0 1 6.5 10.5ZM6.5 4.5L7.5 5.5M3 8.5H6M5.5 11.5L4.5 12.5M10.5 7.5H6.5V10.5A2 2 0 0 0 8.5 12.5A2 2 0 0 0 10.5 10.5ZM10.5 4.5L9.5 5.5M14 8.5H11M11.5 11.5L12.5 12.5");

    /// <summary>Молния по кругу: горячая перезагрузка.</summary>
    public static Geometry HotReload { get; } = P("M9.5 3.5L5.5 8.5L8.5 8.5L7.5 13.5L11.5 8.5L8.5 8.5Z");

    /// <summary>Пауза. IsFilled.</summary>
    public static Geometry Pause { get; } = P("M5 4H7V12H5ZM10 4H12V12H10Z");

    /// <summary>Квадрат: остановить. IsFilled.</summary>
    public static Geometry Stop { get; } = P("M5 5H12V12H5Z");

    /// <summary>Приглашение консоли.</summary>
    public static Geometry Terminal { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM9 11.5H12M5.5 6.5L7.5 8.5L5.5 10.5");


    // Окно, тема, прочее
    /// <summary>Стрелка из рамки: внешняя ссылка.</summary>
    public static Geometry ExternalLink { get; } = P("M7 3.5L3.5 3.5L3.5 13.5L13.5 13.5L13.5 10M10 3.5L13.5 3.5L13.5 7M13.5 3.5L8.5 8.5");

    /// <summary>Шестерёнка: вход в настройки (ползунки остаются за Settings).</summary>
    public static Geometry Gear { get; } = P("M8.5 3.5L9.5 3.5L9.5 5.5L10.5 5.5L11.5 4.5L12.5 5.5L11.5 6.5L11.5 7.5L13.5 7.5L13.5 8.5M8.5 13.5L9.5 13.5L9.5 11.5L10.5 11.5L11.5 12.5L12.5 11.5L11.5 10.5L11.5 9.5L13.5 9.5L13.5 8.5M8.5 3.5L7.5 3.5L7.5 5.5L6.5 5.5L5.5 4.5L4.5 5.5L5.5 6.5L5.5 7.5L3.5 7.5L3.5 8.5M8.5 13.5L7.5 13.5L7.5 11.5L6.5 11.5L5.5 12.5L4.5 11.5L5.5 10.5L5.5 9.5L3.5 9.5L3.5 8.5M7.5 8.5A1 1 0 1 1 9.5 8.5A1 1 0 1 1 7.5 8.5");

    /// <summary>Клавиатура: шорткаты.</summary>
    public static Geometry Keyboard { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM5 7.5H6M7 7.5H8M9 7.5H10M11 7.5H12M6 9.5H11");

    /// <summary>Кнопка-булавка: закрепить.</summary>
    public static Geometry Pin { get; } = P("M6 3.5H11M6.5 4L6.5 7.5L4.5 9.5L12.5 9.5L10.5 7.5L10.5 4M8.5 10V14");

    /// <summary>Звезда: избранное.</summary>
    public static Geometry Star { get; } = P("M8.5 2.8L9.85 6.54L13.83 6.67L10.69 9.11L11.79 12.93L8.5 10.7L5.21 12.93L6.31 9.11L3.17 6.67L7.15 6.54Z");

    /// <summary>Месяц: тёмная тема.</summary>
    public static Geometry ThemeDark { get; } = P("M8.5 3.5A4 4 0 0 0 13.5 8.5A5 5 0 1 1 8.5 3.5Z");

    /// <summary>Солнце: светлая тема.</summary>
    public static Geometry ThemeLight { get; } = P("M4.5 4.5L5.5 5.5M12.5 4.5L11.5 5.5M4.5 12.5L5.5 11.5M12.5 12.5L11.5 11.5M8.5 3V5M8.5 12V14M3 8.5H5M12 8.5H14M6.5 8.5A2 2 0 1 1 10.5 8.5A2 2 0 1 1 6.5 8.5");

    /// <summary>Пользователь.</summary>
    public static Geometry User { get; } = P("M6.5 6.5A2 2 0 1 1 10.5 6.5A2 2 0 1 1 6.5 6.5M4.5 14V13.5A4 4 0 0 1 12.5 13.5V14");

    /// <summary>Развернуть окно.</summary>
    public static Geometry WindowMaximize { get; } = P("M4.5 4.5H12.5V12.5H4.5Z");

    /// <summary>Свернуть окно.</summary>
    public static Geometry WindowMinimize { get; } = P("M4 8.5H13");

    /// <summary>Восстановить окно из развёрнутого.</summary>
    public static Geometry WindowRestore { get; } = P("M4.5 6.5H10.5V12.5H4.5ZM6.5 6.5L6.5 4.5L12.5 4.5L12.5 10.5L10.5 10.5");


    // Поиск, списки, таблицы
    /// <summary>Воронка: фильтр списка.</summary>
    public static Geometry Filter { get; } = P("M3.5 4.5L13.5 4.5L9.5 8.5L9.5 13.5L7.5 13.5L7.5 8.5Z");

    /// <summary>Три точки горизонтально: свёрнутое меню. IsFilled.</summary>
    public static Geometry MoreHorizontal { get; } = P("M3 8.5A1.5 1.5 0 1 1 6 8.5A1.5 1.5 0 1 1 3 8.5M7 8.5A1.5 1.5 0 1 1 10 8.5A1.5 1.5 0 1 1 7 8.5M11 8.5A1.5 1.5 0 1 1 14 8.5A1.5 1.5 0 1 1 11 8.5");

    /// <summary>Три точки вертикально: меню строки. IsFilled.</summary>
    public static Geometry MoreVertical { get; } = P("M7 4.5A1.5 1.5 0 1 1 10 4.5A1.5 1.5 0 1 1 7 4.5M7 8.5A1.5 1.5 0 1 1 10 8.5A1.5 1.5 0 1 1 7 8.5M7 12.5A1.5 1.5 0 1 1 10 12.5A1.5 1.5 0 1 1 7 12.5");

    /// <summary>Стрелки вверх-вниз: сортировка.</summary>
    public static Geometry Sort { get; } = P("M5.5 5V13M11.5 4V12M3.5 6.5L5.5 4.5L7.5 6.5M9.5 10.5L11.5 12.5L13.5 10.5");

    /// <summary>Лупа с плюсом: приблизить.</summary>
    public static Geometry ZoomIn { get; } = P("M3.5 7.5A4 4 0 1 1 11.5 7.5A4 4 0 1 1 3.5 7.5M10.5 10.5L13.5 13.5M7.5 5V10M5 7.5H10");

    /// <summary>Лупа с минусом: отдалить.</summary>
    public static Geometry ZoomOut { get; } = P("M3.5 7.5A4 4 0 1 1 11.5 7.5A4 4 0 1 1 3.5 7.5M10.5 10.5L13.5 13.5M5 7.5H10");


    // Проект и файлы
    /// <summary>Книга: документация.</summary>
    public static Geometry Docs { get; } = P("M8.5 4.5C7.5 3.5 6 3.5 3.5 3.5V12.5C6 12.5 7.5 13 8.5 13.5M8.5 4.5C9.5 3.5 11 3.5 13.5 3.5V12.5C11 12.5 9.5 13 8.5 13.5M8.5 5V13");

    /// <summary>Лист с угловыми скобками: файл разметки или кода.</summary>
    public static Geometry DocumentCode { get; } = P("M4.5 3.5L9.5 3.5L12.5 6.5L12.5 13.5L4.5 13.5ZM9.5 3.5L9.5 6.5L12.5 6.5M7.5 9.5L6.5 10.5L7.5 11.5M9.5 9.5L10.5 10.5L9.5 11.5");

    /// <summary>Раскрытая папка.</summary>
    public static Geometry FolderOpen { get; } = P("M13.5 8L13.5 6.5L9.5 6.5L7.5 4.5L3.5 4.5L3.5 12.5L12 12.5M3.5 12.5L5.5 8.5L13.5 8.5L11.5 12.5");

    /// <summary>Вилка-разъём: плагин.</summary>
    public static Geometry Plugin { get; } = P("M6.5 3V6M10.5 3V6M4.5 5.5H12.5V8.5A2 2 0 0 1 10.5 10.5H6.5A2 2 0 0 1 4.5 8.5ZM8.5 11V14");

    /// <summary>Структура: иерархия элементов документа.</summary>
    public static Geometry Structure { get; } = P("M3 4.5H14M6 8.5H14M9 12.5H14");


    // Раскрытие
    /// <summary>Мелкий шеврон вниз для тесной строки (класс small).</summary>
    public static Geometry ChevronDownSmall { get; } = P("M5.5 6.5L8.5 9.5L11.5 6.5");

    /// <summary>Шеврон влево: назад, свернуть панель.</summary>
    public static Geometry ChevronLeft { get; } = P("M10.5 4.5L6.5 8.5L10.5 12.5");

    /// <summary>Шеврон вверх: свернуть раскрытое.</summary>
    public static Geometry ChevronUp { get; } = P("M4.5 10.5L8.5 6.5L12.5 10.5");

    /// <summary>Свернуть всё дерево.</summary>
    public static Geometry CollapseAll { get; } = P("M4 3.5H13M4 13.5H13M6.5 5.5L8.5 7.5L10.5 5.5M6.5 11.5L8.5 9.5L10.5 11.5");

    /// <summary>Развернуть всё дерево.</summary>
    public static Geometry ExpandAll { get; } = P("M4 3.5H13M4 13.5H13M6.5 7.5L8.5 5.5L10.5 7.5M6.5 9.5L8.5 11.5L10.5 9.5");


    // Статусы
    /// <summary>Круг с крестом: ошибка.</summary>
    public static Geometry Error { get; } = P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M6.5 6.5L10.5 10.5M10.5 6.5L6.5 10.5");

    /// <summary>Круг с «i»: сведения.</summary>
    public static Geometry Info { get; } = P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M8 5.5H9M8.5 7V12");

    /// <summary>Круг с вопросом: справка.</summary>
    public static Geometry Question { get; } = P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M6.5 6.5A2 2 0 1 1 8.5 8.5V10M8 11.5H9");

    /// <summary>Круг с галочкой: успех.</summary>
    public static Geometry Success { get; } = P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M5.5 8.5L7.5 10.5L11.5 6.5");

    /// <summary>Треугольник с восклицательным знаком: предупреждение в баннере и списке проблем.</summary>
    public static Geometry WarningTriangle { get; } = P("M8.5 3.5L13.5 13.5L3.5 13.5ZM8.5 7V10M8 11.5H9");
    // </иконки:actions>

    /// <summary>
    /// Семейство 2 — глифы контролов для палитры дизайнера форм.
    /// Имя глифа = имя контрола Avalonia: <c>Toolbox.Button</c> ставится и для
    /// Button, и для AxButton. List, Tabs, DataGrid, Grid, Image, Window и
    /// Bubble живут в основном наборе и здесь не повторяются.
    /// </summary>
    public static class Toolbox
    {
        // <иконки:toolbox> — собрано tools/build.py из icons/toolbox, руками не править
        // Ввод текста
        /// <summary>Глиф контрола AutoCompleteBox для палитры дизайнера.</summary>
        public static Geometry AutoCompleteBox { get; } = P("M3.5 4.5H13.5V7.5H3.5ZM5.5 5V7M3.5 9.5H13.5V12.5H3.5Z");

        /// <summary>Глиф контрола Label для палитры дизайнера.</summary>
        public static Geometry Label { get; } = P("M4 8.5H10M12 8.5H13");

        /// <summary>Глиф контрола MaskedTextBox для палитры дизайнера.</summary>
        public static Geometry MaskedTextBox { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM5 8.5H6M7 8.5H8M9 8.5H10M11 8.5H13");

        /// <summary>Глиф контрола SearchField для палитры дизайнера.</summary>
        public static Geometry SearchField { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM4.5 8.5A2 2 0 1 1 8.5 8.5A2 2 0 1 1 4.5 8.5M8.5 10.5L9.5 11.5M11 8.5H13");

        /// <summary>Глиф контрола SelectableTextBlock для палитры дизайнера.</summary>
        public static Geometry SelectableTextBlock { get; } = P("M4 5.5H13M3.5 7.5H10.5V10.5H3.5ZM4 12.5H10");

        /// <summary>Глиф контрола TextArea для палитры дизайнера.</summary>
        public static Geometry TextArea { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM5 6.5H12M5 8.5H12M5 10.5H9");

        /// <summary>Глиф контрола TextBlock для палитры дизайнера.</summary>
        public static Geometry TextBlock { get; } = P("M4 5.5H13M4 8.5H13M4 11.5H10");

        /// <summary>Глиф контрола TextBox для палитры дизайнера.</summary>
        public static Geometry TextBox { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM5 8.5H9M10.5 7V10");


        // Вкладки
        /// <summary>Глиф контрола BreadcrumbBar для палитры дизайнера.</summary>
        public static Geometry BreadcrumbBar { get; } = P("M3 8.5H6M9 8.5H12M6.5 7.5L7.5 8.5L6.5 9.5M12.5 7.5L13.5 8.5L12.5 9.5");

        /// <summary>Глиф контрола TabItem для палитры дизайнера.</summary>
        public static Geometry TabItem { get; } = P("M4.5 7.5L4.5 4.5L10.5 4.5L10.5 7.5M3 7.5H14");

        /// <summary>Глиф контрола TabStrip для палитры дизайнера.</summary>
        public static Geometry TabStrip { get; } = P("M3.5 5.5H13.5V9.5H3.5ZM6.5 6V9M10.5 6V9M4 11.5H7");

        /// <summary>Глиф контрола TabStripItem для палитры дизайнера.</summary>
        public static Geometry TabStripItem { get; } = P("M4.5 5.5H12.5V9.5H4.5ZM4 11.5H13");


        // Выбор
        /// <summary>Глиф контрола CheckBox для палитры дизайнера.</summary>
        public static Geometry CheckBox { get; } = P("M3.5 5.5H8.5V11.5H3.5ZM4.5 8.5L5.5 9.5L7.5 7.5M10 8.5H14");

        /// <summary>Глиф контрола ComboBox для палитры дизайнера.</summary>
        public static Geometry ComboBox { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM5 8.5H9M10.5 7.5L11.5 8.5L12.5 7.5");

        /// <summary>Глиф контрола ComboBoxItem для палитры дизайнера.</summary>
        public static Geometry ComboBoxItem { get; } = P("M3.5 6.5H13.5V10.5H3.5ZM4.5 8.5L5.5 9.5L7.5 7.5M9 8.5H12");

        /// <summary>Глиф контрола RadioButton для палитры дизайнера.</summary>
        public static Geometry RadioButton { get; } = P("M3.5 8.5A3 3 0 1 1 9.5 8.5A3 3 0 1 1 3.5 8.5M5.5 8.5A1 1 0 1 1 7.5 8.5A1 1 0 1 1 5.5 8.5M11 8.5H14");

        /// <summary>Глиф контрола SegmentedControl для палитры дизайнера.</summary>
        public static Geometry SegmentedControl { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM6.5 6V11M10.5 6V11");

        /// <summary>Глиф контрола Slider для палитры дизайнера.</summary>
        public static Geometry Slider { get; } = P("M3 8.5H14M8.5 8.5A2 2 0 1 1 12.5 8.5A2 2 0 1 1 8.5 8.5");

        /// <summary>Глиф контрола ToggleSwitch для палитры дизайнера.</summary>
        public static Geometry ToggleSwitch { get; } = P("M6.5 5.5H10.5A3 3 0 0 1 10.5 11.5H6.5A3 3 0 0 1 6.5 5.5ZM9.5 8.5A1 1 0 1 1 11.5 8.5A1 1 0 1 1 9.5 8.5");


        // Дата и время
        /// <summary>Глиф контрола Calendar для палитры дизайнера.</summary>
        public static Geometry Calendar { get; } = P("M3.5 4.5H13.5V13.5H3.5ZM4 7.5H13M6.5 3V5M10.5 3V5M5 9.5H6M8 9.5H9M11 9.5H12M5 11.5H6M8 11.5H9");

        /// <summary>Глиф контрола CalendarDatePicker для палитры дизайнера.</summary>
        public static Geometry CalendarDatePicker { get; } = P("M3.5 5.5H9.5V11.5H3.5ZM5 8.5H8M10.5 5.5H13.5V9.5H10.5ZM11 7.5H13");

        /// <summary>Глиф контрола DatePicker для палитры дизайнера.</summary>
        public static Geometry DatePicker { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM7.5 6V11M10.5 6V11M5 8.5H7M8 8.5H10M11 8.5H13");

        /// <summary>Глиф контрола TimePicker для палитры дизайнера.</summary>
        public static Geometry TimePicker { get; } = P("M3.5 8.5A5 5 0 1 1 13.5 8.5A5 5 0 1 1 3.5 8.5M8.5 6V9M9 8.5H11");


        // Кнопки
        /// <summary>Глиф контрола Button для палитры дизайнера.</summary>
        public static Geometry Button { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM6 8.5H11");

        /// <summary>Глиф контрола ButtonSpinner для палитры дизайнера.</summary>
        public static Geometry ButtonSpinner { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM9.5 6V11M5 8.5H8M10.5 8.5L11.5 7.5L12.5 8.5M10.5 8.5L11.5 9.5L12.5 8.5");

        /// <summary>Глиф контрола DropDownButton для палитры дизайнера.</summary>
        public static Geometry DropDownButton { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM5 8.5H9M10.5 7.5L11.5 8.5L12.5 7.5");

        /// <summary>Глиф контрола HyperlinkButton для палитры дизайнера.</summary>
        public static Geometry HyperlinkButton { get; } = P("M4 6.5H13M4 8.5H10M4 10.5H10");

        /// <summary>Глиф контрола NumericUpDown для палитры дизайнера.</summary>
        public static Geometry NumericUpDown { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM9.5 6V11M5 8.5H7M10.5 8.5L11.5 7.5L12.5 8.5M10.5 8.5L11.5 9.5L12.5 8.5");

        /// <summary>Глиф контрола RepeatButton для палитры дизайнера.</summary>
        public static Geometry RepeatButton { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM5 8.5H9M9.5 7.5L10.5 8.5L9.5 9.5M11.5 7.5L12.5 8.5L11.5 9.5");

        /// <summary>Глиф контрола SplitButton для палитры дизайнера.</summary>
        public static Geometry SplitButton { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM9.5 6V11M5 8.5H8M10.5 7.5L11.5 8.5L12.5 7.5");

        /// <summary>Глиф контрола ToggleButton для палитры дизайнера.</summary>
        public static Geometry ToggleButton { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM8.5 6V11");


        // Контейнеры и компоновка
        /// <summary>Глиф контрола Border для палитры дизайнера.</summary>
        public static Geometry Border { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM5.5 5.5H11.5V11.5H5.5Z");

        /// <summary>Глиф контрола Canvas для палитры дизайнера.</summary>
        public static Geometry Canvas { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6.5 6.5H11.5V10.5H6.5Z");

        /// <summary>Глиф контрола ContentControl для палитры дизайнера.</summary>
        public static Geometry ContentControl { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6.5 8.5A2 2 0 1 1 10.5 8.5A2 2 0 1 1 6.5 8.5");

        /// <summary>Глиф контрола DockPanel для палитры дизайнера.</summary>
        public static Geometry DockPanel { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM4 6.5H13M5.5 7V13M11.5 7V13");

        /// <summary>Глиф контрола Expander для палитры дизайнера.</summary>
        public static Geometry Expander { get; } = P("M3.5 3.5H13.5V6.5H3.5ZM3.5 9.5H13.5V13.5H3.5ZM4.5 4.5L5.5 5.5L6.5 4.5");

        /// <summary>Глиф контрола GroupBox для палитры дизайнера.</summary>
        public static Geometry GroupBox { get; } = P("M3.5 5.5H13.5V13.5H3.5ZM5.5 5.5L5.5 3.5L9.5 3.5L9.5 5.5");

        /// <summary>Глиф контрола Panel для палитры дизайнера.</summary>
        public static Geometry Panel { get; } = P("M3.5 3.5H13.5V13.5H3.5Z");

        /// <summary>Глиф контрола RelativePanel для палитры дизайнера.</summary>
        public static Geometry RelativePanel { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM8.5 8.5H12.5V11.5H8.5ZM4 8.5H8M10.5 4V8");

        /// <summary>Глиф контрола SplitView для палитры дизайнера.</summary>
        public static Geometry SplitView { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6.5 4V13");

        /// <summary>Глиф контрола StackPanel для палитры дизайнера.</summary>
        public static Geometry StackPanel { get; } = P("M3.5 3.5H13.5V6.5H3.5ZM3.5 10.5H13.5V13.5H3.5Z");

        /// <summary>Глиф контрола UniformGrid для палитры дизайнера.</summary>
        public static Geometry UniformGrid { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM4 6.5H13M4 10.5H13M8.5 4V13");

        /// <summary>Глиф контрола Viewbox для палитры дизайнера.</summary>
        public static Geometry Viewbox { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM5.5 5.5L10.5 10.5M5.5 8L5.5 5.5L8 5.5M10.5 8L10.5 10.5L8 10.5");

        /// <summary>Глиф контрола WrapPanel для палитры дизайнера.</summary>
        public static Geometry WrapPanel { get; } = P("M3.5 3.5H6.5V6.5H3.5ZM13.5 3.5H10.5V6.5H13.5ZM3.5 13.5H6.5V10.5H3.5ZM13.5 13.5H10.5V10.5H13.5Z");


        // Контролы студии
        /// <summary>Глиф контрола Avatar для палитры дизайнера.</summary>
        public static Geometry Avatar { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6.5 7.5A2 2 0 1 1 10.5 7.5A2 2 0 1 1 6.5 7.5M5.5 13V12.5A3 3 0 0 1 11.5 12.5V13");

        /// <summary>Глиф контрола Badge для палитры дизайнера.</summary>
        public static Geometry Badge { get; } = P("M3.5 7.5H9.5V13.5H3.5ZM9.5 5.5A2 2 0 1 1 13.5 5.5A2 2 0 1 1 9.5 5.5");

        /// <summary>Глиф контрола Banner для палитры дизайнера.</summary>
        public static Geometry Banner { get; } = P("M3.5 5.5H13.5V11.5H3.5ZM4.5 8.5A1 1 0 1 1 6.5 8.5A1 1 0 1 1 4.5 8.5M8 8.5H13");

        /// <summary>Глиф контрола Card для палитры дизайнера.</summary>
        public static Geometry Card { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM4 7.5H13M5 10.5H12");

        /// <summary>Глиф контрола Chip для палитры дизайнера.</summary>
        public static Geometry Chip { get; } = P("M6.5 5.5H10.5A3 3 0 0 1 10.5 11.5H6.5A3 3 0 0 1 6.5 5.5ZM7 8.5H10");

        /// <summary>Глиф контрола CodeBlock для палитры дизайнера.</summary>
        public static Geometry CodeBlock { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6.5 6.5L4.5 8.5L6.5 10.5M10.5 6.5L12.5 8.5L10.5 10.5");

        /// <summary>Глиф контрола Dialog для палитры дизайнера.</summary>
        public static Geometry Dialog { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM4 6.5H13M7 10.5H9M10 10.5H12");

        /// <summary>Глиф контрола GroupHeader для палитры дизайнера.</summary>
        public static Geometry GroupHeader { get; } = P("M3.5 6.5H7.5V10.5H3.5ZM9 8.5H14");

        /// <summary>Глиф контрола TitleBar для палитры дизайнера.</summary>
        public static Geometry TitleBar { get; } = P("M3.5 4.5H13.5V12.5H3.5ZM4 7.5H13M10 5.5H11M12 5.5H13");

        /// <summary>Глиф контрола ToolBar для палитры дизайнера.</summary>
        public static Geometry ToolBar { get; } = P("M3 4.5H14M3.5 7.5H6.5V10.5H3.5ZM10.5 7.5H13.5V10.5H10.5Z");

        /// <summary>Глиф контрола ToolWindow для палитры дизайнера.</summary>
        public static Geometry ToolWindow { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM6.5 4V13M7 6.5H13");

        /// <summary>Глиф контрола WindowControls для палитры дизайнера.</summary>
        public static Geometry WindowControls { get; } = P("M3 8.5H5M6.5 6.5H10.5V10.5H6.5ZM11.5 7.5L13.5 9.5M13.5 7.5L11.5 9.5");


        // Меню и всплывающие
        /// <summary>Глиф контрола ContextMenu для палитры дизайнера.</summary>
        public static Geometry ContextMenu { get; } = P("M4.5 3.5H12.5V13.5H4.5ZM6 5.5H11M5 7.5H12M6 9.5H11M6 11.5H11");

        /// <summary>Глиф контрола Menu для палитры дизайнера.</summary>
        public static Geometry Menu { get; } = P("M3.5 4.5H13.5V7.5H3.5ZM6.5 5V7M9.5 5V7M3.5 9.5H9.5V13.5H3.5Z");

        /// <summary>Глиф контрола MenuItem для палитры дизайнера.</summary>
        public static Geometry MenuItem { get; } = P("M3.5 6.5H13.5V10.5H3.5ZM5 8.5H10M11.5 7.5L12.5 8.5L11.5 9.5");

        /// <summary>Глиф контрола Popup для палитры дизайнера.</summary>
        public static Geometry Popup { get; } = P("M3.5 3.5H10.5V10.5H3.5ZM6.5 6.5H13.5V13.5H6.5Z");

        /// <summary>Глиф контрола QuickSearch для палитры дизайнера.</summary>
        public static Geometry QuickSearch { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM4 6.5H13M5.5 4V6M5 9.5H12M5 11.5H10");

        /// <summary>Глиф контрола TeachingTip для палитры дизайнера.</summary>
        public static Geometry TeachingTip { get; } = P("M3.5 3.5L13.5 3.5L13.5 10.5L8.5 10.5L5.5 13.5L5.5 10.5L3.5 10.5ZM8.5 5V8M8 9.5H9");

        /// <summary>Глиф контрола ToolTip для палитры дизайнера.</summary>
        public static Geometry ToolTip { get; } = P("M4.5 5.5H12.5V9.5H4.5ZM6.5 9.5L8.5 11.5L10.5 9.5");


        // Прогресс, медиа, оверлеи
        /// <summary>Глиф контрола NotificationCard для палитры дизайнера.</summary>
        public static Geometry NotificationCard { get; } = P("M3.5 4.5H13.5V12.5H3.5ZM5.5 5V12M7 7.5H12M7 9.5H11");

        /// <summary>Глиф контрола PathIcon для палитры дизайнера.</summary>
        public static Geometry PathIcon { get; } = P("M3.5 11.5C5.5 5.5 8.5 11.5 13.5 4.5");

        /// <summary>Глиф контрола ProgressBar для палитры дизайнера.</summary>
        public static Geometry ProgressBar { get; } = P("M3.5 6.5H13.5V10.5H3.5ZM8.5 7V10");

        /// <summary>Глиф контрола RefreshContainer для палитры дизайнера.</summary>
        public static Geometry RefreshContainer { get; } = P("M3.5 6.5H13.5V13.5H3.5ZM8.5 3V5M7.5 4.5L8.5 5.5L9.5 4.5");

        /// <summary>Глиф контрола Spinner для палитры дизайнера.</summary>
        public static Geometry Spinner { get; } = P("M8.5 3.5A5 5 0 1 1 3.5 8.5");

        /// <summary>Глиф контрола WindowNotificationManager для палитры дизайнера.</summary>
        public static Geometry WindowNotificationManager { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM8.5 5.5H12.5V8.5H8.5Z");


        // Прокрутка и разделители
        /// <summary>Глиф контрола GridSplitter для палитры дизайнера.</summary>
        public static Geometry GridSplitter { get; } = P("M3.5 3.5H7.5V13.5H3.5ZM9.5 3.5H13.5V13.5H9.5ZM8.5 7V10");

        /// <summary>Глиф контрола ScrollBar для палитры дизайнера.</summary>
        public static Geometry ScrollBar { get; } = P("M6.5 3.5H10.5V13.5H6.5ZM7 5.5H10M7 11.5H10");

        /// <summary>Глиф контрола ScrollViewer для палитры дизайнера.</summary>
        public static Geometry ScrollViewer { get; } = P("M3.5 3.5H11.5V13.5H3.5ZM13.5 5V9");

        /// <summary>Глиф контрола Separator для палитры дизайнера.</summary>
        public static Geometry Separator { get; } = P("M3 8.5H7M8 8.5H9M10 8.5H14");


        // Списки и данные
        /// <summary>Глиф контрола Carousel для палитры дизайнера.</summary>
        public static Geometry Carousel { get; } = P("M5.5 4.5H11.5V12.5H5.5ZM4.5 7.5L3.5 8.5L4.5 9.5M12.5 7.5L13.5 8.5L12.5 9.5");

        /// <summary>Глиф контрола ItemsControl для палитры дизайнера.</summary>
        public static Geometry ItemsControl { get; } = P("M6 5.5H14M6 8.5H14M6 11.5H14M4 5.5H5M4 8.5H5M4 11.5H5");

        /// <summary>Глиф контрола ListBoxItem для палитры дизайнера.</summary>
        public static Geometry ListBoxItem { get; } = P("M3.5 6.5H13.5V10.5H3.5ZM5 8.5H10");

        /// <summary>Глиф контрола PipsPager для палитры дизайнера.</summary>
        public static Geometry PipsPager { get; } = P("M3.5 8.5A1 1 0 1 1 5.5 8.5A1 1 0 1 1 3.5 8.5M8 8.5H9M12 8.5H13");

        /// <summary>Глиф контрола TreeView для палитры дизайнера.</summary>
        public static Geometry TreeView { get; } = P("M3.5 3.5H13.5V13.5H3.5ZM5 6.5H12M5.5 7V12M7 9.5H12M7 11.5H12");

        /// <summary>Глиф контрола TreeViewItem для палитры дизайнера.</summary>
        public static Geometry TreeViewItem { get; } = P("M4.5 6.5L6.5 8.5L4.5 10.5M8 8.5H14");
        // </иконки:toolbox>
    }
}
