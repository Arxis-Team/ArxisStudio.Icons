using System.Reflection;
using System.Text.RegularExpressions;
using ArxisStudio.Icons;
using Avalonia.Media;

namespace Icons.Gallery;

/// <summary>Одна иконка набора со всем, что о ней известно.</summary>
/// <param name="Name">Имя свойства: <c>AxIcons.Folder</c>.</param>
/// <param name="Family">Семейство: действия или глифы палитры.</param>
/// <param name="Group">Группа внутри семейства; пусто — вне групп.</param>
/// <param name="Title">Подпись из файла иконки.</param>
/// <param name="Geometry">Путь в координатах 16×16.</param>
/// <param name="Filled">Силуэт вместо контура.</param>
public sealed record Icon(
    string Name,
    string Family,
    string Group,
    string Title,
    Geometry Geometry,
    bool Filled);

/// <summary>
/// Набор целиком: пути из сборки, подписи из файлов.
/// </summary>
/// <remarks>
/// Пути берутся отражением, а не чтением SVG: витрина обязана показывать то,
/// что достанется студии, — то есть содержимое сборки. Подписи и группы
/// читаются из <c>icons/</c>, потому что там они и живут; заодно это
/// проверяет, что файлы и сборка описывают один набор.
/// </remarks>
public static class Catalogue
{
    private static readonly Regex Title = new(@"<title>(?<value>.*?)</title>", RegexOptions.Singleline);
    private static readonly Regex Desc = new(@"<desc>(?<value>.*?)</desc>", RegexOptions.Singleline);
    private static readonly Regex Fill = new(@"fill=""(?<value>[^""]+)""");

    /// <summary>Собирает набор: сначала действия, потом глифы палитры.</summary>
    public static IReadOnlyList<Icon> Load()
    {
        var root = Icons();
        var found = new List<Icon>();

        found.AddRange(Family(typeof(AxIcons), "actions", "Действия и объекты", root));
        found.AddRange(Family(typeof(AxIcons.Toolbox), "toolbox", "Глифы палитры", root));

        return found;
    }

    private static IEnumerable<Icon> Family(Type type, string folder, string family, string? root)
    {
        foreach (var property in type
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(Geometry))
            .OrderBy(property => property.Name, StringComparer.Ordinal))
        {
            var file = root is null ? null : Path.Combine(root, folder, property.Name + ".svg");
            var text = file is not null && File.Exists(file) ? File.ReadAllText(file) : null;

            yield return new Icon(
                property.Name,
                family,
                Read(Desc, text) ?? string.Empty,
                Read(Title, text) ?? property.Name,
                (Geometry)property.GetValue(null)!,
                Read(Fill, text) is { } fill && fill != "none");
        }
    }

    private static string? Read(Regex pattern, string? text) =>
        text is null || pattern.Match(text) is not { Success: true } match
            ? null
            : match.Groups["value"].Value.Trim()
                .Replace("&lt;", "<", StringComparison.Ordinal)
                .Replace("&gt;", ">", StringComparison.Ordinal)
                .Replace("&amp;", "&", StringComparison.Ordinal);

    /// <summary>Папка с файлами набора; null — витрину запустили вне репозитория.</summary>
    private static string? Icons()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, "icons");

            if (Directory.Exists(Path.Combine(candidate, "actions")))
                return candidate;
        }

        return null;
    }
}
