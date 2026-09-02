using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace ArxisStudio.Icons.Tests;

/// <summary>
/// Файлы <c>icons/</c> — источник набора, а не его копия.
/// </summary>
/// <remarks>
/// Путь живёт в SVG: его открывает дизайнер, его видно в диффе. AxIcons.cs
/// собирается из файлов генератором, и разойтись они могут молча — сборку
/// собрали, файл поправили, а генератор не позвали.
/// <para>
/// Здесь же закреплено правило сетки, ради которого набор и переставляли.
/// Дизайн-проект записал его словами и сам же за собой не уследил: двадцать
/// четыре пути были написаны до правила и под него не подгонялись. Правило,
/// которое проверяет только внимание автора, — не правило.
/// </para>
/// </remarks>
public class IconSourceTests
{
    private const string Grid = "прямой осевой штрих обязан стоять на полупикселе";

    public static TheoryData<string, string> Files
    {
        get
        {
            var data = new TheoryData<string, string>();

            foreach (var (family, name, _) in Icons())
                data.Add(family, name);

            return data;
        }
    }

    /// <summary>Что нарисовано в файле, то и лежит в сборке.</summary>
    [AvaloniaTheory]
    [MemberData(nameof(Files))]
    public void The_path_in_code_is_the_path_in_the_file(string family, string name)
    {
        var text = File.ReadAllText(System.IO.Path.Combine(Root(), family, name + ".svg"));
        var declared = Geometry.Parse(Data(text));
        var built = Resolve(family, name);

        Assert.Equal(Round(declared.Bounds), Round(built.Bounds));
        Assert.Equal(declared.ToString(), built.ToString());
    }

    /// <summary>Файлы и сборка описывают один набор — без лишних и потерянных.</summary>
    [AvaloniaFact]
    public void The_files_and_the_assembly_hold_the_same_set()
    {
        foreach (var (family, type) in new[] { ("actions", typeof(AxIcons)), ("toolbox", typeof(AxIcons.Toolbox)) })
        {
            var built = type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(property => property.PropertyType == typeof(Geometry))
                .Select(property => property.Name)
                .Order(StringComparer.Ordinal);

            var files = Directory
                .EnumerateFiles(System.IO.Path.Combine(Root(), family), "*.svg")
                .Select(System.IO.Path.GetFileNameWithoutExtension)
                .OfType<string>()
                .Order(StringComparer.Ordinal);

            Assert.Equal(files, built);
        }
    }

    /// <summary>
    /// Прямые осевые штрихи стоят на сетке — контуры на полупикселе, силуэты на целом.
    /// </summary>
    /// <remarks>
    /// Обводка центрирована на пути: чернила лягут в один пиксель, только если
    /// её середина стоит на x.5. Заливка кончается на самом пути, и ей нужна
    /// целая координата, иначе по контуру идёт полупрозрачный ореол.
    /// <para>
    /// Точка, которой путь входит в дугу или кривую, из правила выведена: она
    /// держит касание, и сдвинуть её значит сломать окружность. Таких мест
    /// немного, и каждое — сознательный обмен резкости на форму.
    /// </para>
    /// </remarks>
    [AvaloniaTheory]
    [MemberData(nameof(Files))]
    public void Straight_strokes_stand_on_the_pixel_grid(string family, string name)
    {
        var text = File.ReadAllText(System.IO.Path.Combine(Root(), family, name + ".svg"));
        var filled = !text.Contains("fill=\"none\"", StringComparison.Ordinal);
        var commands = Commands(Data(text));

        Assert.All(Straight(commands), segment =>
        {
            var (from, to, locked) = segment;

            if (locked)
                return;

            var horizontal = Math.Abs(from.Y - to.Y) < 1e-6 && Math.Abs(from.X - to.X) > 1e-6;
            var vertical = Math.Abs(from.X - to.X) < 1e-6 && Math.Abs(from.Y - to.Y) > 1e-6;

            if (!horizontal && !vertical)
                return;

            var value = horizontal ? from.Y : from.X;
            var wanted = filled ? Math.Round(value) : Math.Round(value - 0.5) + 0.5;

            Assert.True(
                Math.Abs(value - wanted) < 1e-6,
                string.Create(CultureInfo.InvariantCulture, $"{name}: {Grid}, а стоит на {value}"));
        });
    }

    /// <summary>Разбирает путь в точки: файлы набора записаны абсолютными командами.</summary>
    private static List<(char Letter, double X, double Y)> Commands(string path)
    {
        var found = new List<(char, double, double)>();
        var tokens = Regex.Matches(path, @"[A-Za-z]|-?\d*\.?\d+");
        var numbers = new List<double>();
        var letter = '\0';
        double x = 0, y = 0;

        void Flush()
        {
            if (letter == '\0')
                return;

            Assert.True(char.IsUpper(letter), $"{letter}: путь набора обязан быть в абсолютных командах");

            var size = letter switch { 'H' or 'V' => 1, 'C' => 6, 'S' or 'Q' => 4, 'A' => 7, 'Z' => 0, _ => 2 };

            if (size == 0)
            {
                found.Add(('Z', x, y));
                return;
            }

            for (var at = 0; at + size <= numbers.Count; at += size)
            {
                var current = letter == 'M' && at > 0 ? 'L' : letter;

                if (current == 'H')
                    x = numbers[at];
                else if (current == 'V')
                    y = numbers[at];
                else
                {
                    x = numbers[at + size - 2];
                    y = numbers[at + size - 1];
                }

                found.Add((current, x, y));
            }
        }

        foreach (Match token in tokens)
        {
            if (char.IsLetter(token.Value[0]))
            {
                Flush();
                letter = token.Value[0];
                numbers.Clear();
            }
            else
            {
                numbers.Add(double.Parse(token.Value, CultureInfo.InvariantCulture));
            }
        }

        Flush();

        return found;
    }

    /// <summary>Прямые отрезки парами точек; locked — отрезок держит дуга или кривая.</summary>
    private static IEnumerable<((double X, double Y) From, (double X, double Y) To, bool Locked)> Straight(
        List<(char Letter, double X, double Y)> commands)
    {
        for (var at = 1; at < commands.Count; at++)
        {
            var (letter, x, y) = commands[at];

            if (letter is not ('L' or 'H' or 'V'))
                continue;

            var previous = commands[at - 1];
            var next = at + 1 < commands.Count ? commands[at + 1].Letter : 'Z';
            var locked = previous.Letter is 'A' or 'C' or 'S' or 'Q' or 'T'
                || next is 'A' or 'C' or 'S' or 'Q' or 'T';

            yield return ((previous.X, previous.Y), (x, y), locked);
        }
    }

    private static string Data(string svg) =>
        Regex.Match(svg, @"<path d=""([^""]+)""").Groups[1].Value;

    private static Geometry Resolve(string family, string name)
    {
        var type = family == "actions" ? typeof(AxIcons) : typeof(AxIcons.Toolbox);

        return (Geometry)type.GetProperty(name, BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
    }

    private static Avalonia.Rect Round(Avalonia.Rect rect) => new(
        Math.Round(rect.X, 3),
        Math.Round(rect.Y, 3),
        Math.Round(rect.Width, 3),
        Math.Round(rect.Height, 3));

    private static IEnumerable<(string Family, string Name, string File)> Icons()
    {
        foreach (var family in new[] { "actions", "toolbox" })
        {
            foreach (var file in Directory.EnumerateFiles(System.IO.Path.Combine(Root(), family), "*.svg").Order(StringComparer.Ordinal))
                yield return (family, System.IO.Path.GetFileNameWithoutExtension(file), file);
        }
    }

    /// <summary>Папка набора: над ней лежит решение.</summary>
    private static string Root()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = System.IO.Path.Combine(directory.FullName, "icons");

            if (Directory.Exists(System.IO.Path.Combine(candidate, "actions")))
                return candidate;
        }

        throw new InvalidOperationException("Не найдена папка icons/ — тест запущен вне репозитория");
    }
}
