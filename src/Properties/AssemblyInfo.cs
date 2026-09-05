using Avalonia.Metadata;

// Тот же словарь разметки, что и у остальных библиотек ArxisStudio: набор
// иконок живёт отдельным репозиторием, но в разметке стоит рядом с контролами,
// и разделять их ещё и адресом незачем — <AxIcon> пишется так же, как
// <AxButton>.
[assembly: XmlnsDefinition("https://github.com/Arxis-Team/ArxisStudio", "ArxisStudio.Icons")]

// Префикс, который предложит инструмент, когда адрес объявляют псевдонимом.
[assembly: XmlnsPrefix("https://github.com/Arxis-Team/ArxisStudio", "ax")]
