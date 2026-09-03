using System.Reflection;
using System.Text.Json;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace ArxisStudio.Icons.Tests;

/// <summary>
/// Состав набора против листа иконок дизайн-проекта.
/// </summary>
/// <remarks>
/// Дизайн-проект отвечает за состав набора, а не за геометрию: набор
/// перерисован под пиксельную сетку целиком, и сверять форму с листом, который
/// эту сетку не держал, значило бы сверять с тем, от чего ушли. Форму проверяют
/// <c>IconSourceTests</c> — совпадение с файлом и правило сетки.
/// <para>
/// Состав же сверяется именно с листом: пропавшая иконка не падает ни в одной
/// сборке, а лишняя не мешает никому — и обе видны только рядом со списком
/// того, что задумано.
/// </para>
/// </remarks>
public class IconGeometryTests
{
    /// <summary>Набор не растерял иконок и не завёл лишних.</summary>
    [AvaloniaFact]
    public void Set_holds_exactly_the_icons_of_the_design_project()
    {
        var declared = Sources()
            .SelectMany(type => type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            .Where(property => property.PropertyType == typeof(Geometry))
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(Load().Keys.OrderBy(name => name, StringComparer.Ordinal),
            declared.OrderBy(name => name, StringComparer.Ordinal));
    }

    /// <summary>Действия лежат в AxIcons, глифы палитры — во вложенном Toolbox.</summary>
    private static IEnumerable<Type> Sources()
    {
        yield return typeof(AxIcons);

        foreach (var nested in typeof(AxIcons).GetNestedTypes(BindingFlags.Public))
            yield return nested;
    }

    private static Dictionary<string, string> Load()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var name = assembly.GetManifestResourceNames()
            .Single(candidate => candidate.EndsWith("design-icons.json", StringComparison.Ordinal));

        using var stream = assembly.GetManifestResourceStream(name)!;

        return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!["icons"];
    }
}
