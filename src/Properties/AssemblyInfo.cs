using System.Runtime.CompilerServices;
using Avalonia.Metadata;

// Правило толщины обводки — чистая функция, и проверяется оно без отрисовки.
[assembly: InternalsVisibleTo("ArxisStudio.Icons.Tests")]

// Витрина — рабочий стол набора: она меряет резкость того самого пути, который возьмёт экран,
// а выбор пути под клетку — внутреннее дело набора.
[assembly: InternalsVisibleTo("Icons.Gallery")]

// Тот же словарь разметки, что и у остальных библиотек ArxisStudio: набор
// иконок живёт отдельным репозиторием, но в разметке стоит рядом с контролами,
// и разделять их ещё и адресом незачем — <AxIcon> пишется так же, как
// <AxButton>.
[assembly: XmlnsDefinition("https://github.com/Arxis-Team/ArxisStudio", "ArxisStudio.Icons")]

// Префикс, который предложит инструмент, когда адрес объявляют псевдонимом.
[assembly: XmlnsPrefix("https://github.com/Arxis-Team/ArxisStudio", "ax")]
