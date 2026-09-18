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
/// Здесь же закреплено правило сетки, ради которого набор перерисовали.
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

    /// <summary>Иконка и клетка, в которую её пересчитали.</summary>
    public static TheoryData<string, string, int> Fitted
    {
        get
        {
            var data = new TheoryData<string, string, int>();

            foreach (var (family, name, _) in Icons())
            {
                foreach (var cell in AxIcons.Sizing)
                    data.Add(family, name, cell);
            }

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
        var built = typeof(AxIcons)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(Geometry))
            .Select(property => property.Name)
            .Order(StringComparer.Ordinal);

        var files = Directory
            .EnumerateFiles(System.IO.Path.Combine(Root(), "actions"), "*.svg")
            .Select(System.IO.Path.GetFileNameWithoutExtension)
            .OfType<string>()
            .Order(StringComparer.Ordinal);

        Assert.Equal(files, built);
    }

    /// <summary>
    /// Путь набора разбирается при первом обращении к иконке, а не при загрузке набора.
    /// </summary>
    /// <remarks>
    /// Экрану нужны единицы иконок, и разбор всего набора до первого кадра — лишняя работа на
    /// старте. Спрашивается собранный код, а не живой тип: какие иконки уже тронули соседние тесты,
    /// в одном процессе не узнать, а то, как объявлен геттер, видно всегда.
    /// </remarks>
    [Fact]
    public void Every_icon_is_parsed_on_first_use()
    {
        var code = File.ReadAllText(System.IO.Path.Combine(Root(), "..", "src", "AxIcons.cs"));
        var getters = Regex.Matches(code, @"public static Geometry (\w+) (.*)$", RegexOptions.Multiline);

        Assert.NotEmpty(getters);
        Assert.All(getters, getter => Assert.Matches(@"^\{ get => field \?\?= (Filled\()?P\(""", getter.Groups[2].Value));
    }

    /// <summary>
    /// Иконка, нарисованная в файле силуэтом, заливается сама — без слова автора.
    /// </summary>
    /// <remarks>
    /// Прежде заливку помнил тот, кто ставил иконку, и пять силуэтов набора стояли в студии
    /// обведёнными: треугольник запуска и три точки меню читались контуром.
    /// </remarks>
    [AvaloniaTheory]
    [MemberData(nameof(Files))]
    public void An_icon_drawn_as_a_silhouette_is_filled_by_itself(string family, string name)
    {
        var text = File.ReadAllText(System.IO.Path.Combine(Root(), family, name + ".svg"));
        var silhouette = Regex.Match(text, @"<svg[^>]*\sfill=""([^""]+)""").Groups[1].Value != "none";
        var icon = new AxIcon { Data = Resolve(family, name) };

        Assert.Equal(silhouette, icon.Classes.Contains(":filled"));
    }

    /// <summary>
    /// Плитка — силуэт, и каждая его точка стоит на целой единице сетки.
    /// </summary>
    /// <remarks>
    /// Плитку рисуют в 64 и 128 точек — вчетверо и в восемь раз крупнее клетки, — и на любом
    /// масштабе с шагом в четверть единица сетки занимает там целое число пикселей: 4 × 1,25 = 5,
    /// 4 × 1,5 = 6, 4 × 1,75 = 7. Край заливки на целой точке ложится тогда на границу пикселя при
    /// каждом из пяти масштабов, а размытыми остаются только диагонали под 45°. Половинная точка
    /// попала бы на середину пикселя при 125 и 175 %, контур — тоже: его толщина сводится к
    /// пикселям лишь там, где клетка своя.
    /// </remarks>
    [Fact]
    public void A_tile_is_a_silhouette_with_every_point_on_a_whole_unit()
    {
        var tiles = Icons().Where(icon => icon.Name.EndsWith("Tile", StringComparison.Ordinal)).ToList();

        Assert.NotEmpty(tiles);

        Assert.All(tiles, tile =>
        {
            var text = File.ReadAllText(tile.File);

            Assert.True(
                Regex.Match(text, @"<svg[^>]*\sfill=""([^""]+)""").Groups[1].Value != "none",
                $"{tile.Name}: плитка обязана быть силуэтом, а нарисована контуром");

            Assert.All(Regex.Matches(Data(text), @"-?\d*\.?\d+"), number =>
            {
                var value = double.Parse(number.Value, CultureInfo.InvariantCulture);

                Assert.True(
                    Math.Abs(value - Math.Round(value)) < 1e-6 && value is >= 2 and <= 14,
                    string.Create(CultureInfo.InvariantCulture,
                        $"{tile.Name}: точка {value} не на целой единице или вне живой области 2…14"));
            });
        });
    }

    /// <summary>
    /// Силуэт из прямых, растянутый туда, где единица сетки не целая, сажается на пиксели: края — на
    /// целых, и каждый не дальше полупикселя от своего места. Силуэт с дугой не сажается вовсе.
    /// </summary>
    /// <remarks>
    /// Плитку окна проекта растягивают ступенями по 16 точек, и у 48 при 125 % на единицу приходится
    /// 3,75 пикселя: край заливки на целой точке сетки падает внутрь пикселя. Сдвиг вершины к
    /// ближайшему пикселю ставит прямой край на границу пикселя и меняет рисунок меньше чем на
    /// пиксель. Дугу так не посадить — её вершины держат касание, — и путь с дугой остаётся как был.
    /// </remarks>
    [AvaloniaFact]
    public void A_stretched_silhouette_is_snapped_to_the_pixels_nearest_to_its_edges()
    {
        var silhouettes = Icons()
            .Select(icon => (icon.Name, Path: Data(File.ReadAllText(icon.File)), Geometry: Resolve(icon.Family, icon.Name)))
            .Where(icon => new AxIcon { Data = icon.Geometry }.Classes.Contains(":filled"))
            .ToList();

        Assert.Contains(silhouettes, icon => icon.Name == "FolderTile");
        Assert.Contains(silhouettes, icon => icon.Name == "DocumentTile");

        foreach (var (name, path, geometry) in silhouettes)
        {
            var curved = Regex.IsMatch(path, "[AaCcQqSsTt]");

            for (var cell = 20; cell <= 224; cell++)
            {
                var snapped = AxIcons.Snapped(geometry, cell);

                if (curved)
                {
                    Assert.True(snapped is null, $"{name}: путь с дугой посажен на пиксели в клетке {cell}");
                    continue;
                }

                Assert.True(snapped is not null, $"{name}: силуэт из прямых не посажен на пиксели в клетке {cell}");

                var scale = cell / 16.0;
                var ideal = geometry.Bounds;
                var edges = new[]
                {
                    (snapped.Bounds.Left, ideal.Left * scale),
                    (snapped.Bounds.Top, ideal.Top * scale),
                    (snapped.Bounds.Right, ideal.Right * scale),
                    (snapped.Bounds.Bottom, ideal.Bottom * scale),
                };

                foreach (var (edge, place) in edges)
                {
                    Assert.True(
                        Math.Abs(edge - Math.Round(edge)) < 1e-6,
                        string.Create(CultureInfo.InvariantCulture, $"{name} в клетке {cell}: край {edge} не на целом пикселе"));
                    Assert.True(
                        Math.Abs(edge - place) <= 0.5 + 1e-6,
                        string.Create(CultureInfo.InvariantCulture, $"{name} в клетке {cell}: край {edge} дальше полупикселя от {place}"));
                }
            }
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
    /// Срез штриха плоский, поэтому и его концы обязаны стоять на границе
    /// пикселя — на целом вдоль штриха. Иначе последний пиксель закрашен
    /// наполовину: это и есть хвост, который прежде тянул за собой скруглённый
    /// срез. Конец, упирающийся в другой штрих, от правила свободен — его
    /// пиксель закрыт тем штрихом.
    /// </para>
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
        var straight = Straight(commands).ToList();

        Assert.All(straight, segment =>
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

        if (filled)
            return;

        // Свободные концы: вдоль штриха — на целом, то есть на границе пикселя.
        // Полупиксель разрешён только концу, который упирается в другой штрих:
        // его пиксель закрыт тем штрихом, и хвоста не будет.
        var segments = straight.Select(segment => (segment.From, segment.To)).ToList();

        Assert.All(Ends(commands), end =>
        {
            var (point, own, along) = end;
            var onGrid = Math.Abs(along - Math.Round(along)) < 1e-6;

            Assert.True(
                onGrid || Touches(point, own, segments),
                string.Create(CultureInfo.InvariantCulture,
                    $"{name}: конец штриха в ({point.X}, {point.Y}) стоит на полупикселе и ни во что не упирается"));
        });
    }

    /// <summary>
    /// Пересчитанный под клетку путь стоит на её сетке так же, как исходный — на своей.
    /// </summary>
    /// <remarks>
    /// Набор нарисован в клетке 16, и в пиксели он ложится только там, где на единицу приходится
    /// целое их число. Значок в шестнадцать точек занимает при 125 % ровно 20 пикселей, мелкий, в
    /// двенадцать, — пятнадцать: путь, пересчитанный под такую клетку, снова стоит на сетке, и
    /// пересчёт обязан это сохранять. Иначе он лишь переносит рисунок мимо пикселей в другом
    /// месте.
    /// </remarks>
    [AvaloniaTheory]
    [MemberData(nameof(Fitted))]
    public void A_fitted_path_stands_on_the_grid_of_its_cell(string family, string name, int cell)
    {
        var text = File.ReadAllText(System.IO.Path.Combine(Root(), family, name + ".svg"));
        var filled = !text.Contains("fill=\"none\"", StringComparison.Ordinal);
        var commands = Commands(Drawn(family, name, cell));

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
                string.Create(CultureInfo.InvariantCulture, $"{name} в клетке {cell}: {Grid}, а стоит на {value}"));
        });
    }

    /// <summary>
    /// Пересчёт сохраняет наклон: диагональ под 45° остаётся под 45°.
    /// </summary>
    /// <remarks>
    /// Снять концы диагонали порознь — и она перестаёт быть диагональю: у шеврона плечи разной
    /// длины, у галочки один конец выше другого. Проверено это было поломкой: простой пересчёт
    /// координат ломал 34 диагонали из 101.
    /// </remarks>
    [AvaloniaTheory]
    [MemberData(nameof(Fitted))]
    public void A_fitted_diagonal_keeps_its_slope(string family, string name, int cell)
    {
        var straight = Straight(Commands(Data(File.ReadAllText(System.IO.Path.Combine(Root(), family, name + ".svg")))))
            .Select(segment => (segment.From, segment.To))
            .ToList();

        var fitted = Straight(Commands(Drawn(family, name, cell)))
            .Select(segment => (segment.From, segment.To))
            .ToList();

        Assert.Equal(straight.Count, fitted.Count);

        for (var at = 0; at < straight.Count; at++)
        {
            var (from, to) = straight[at];
            var step = (X: Math.Abs(to.X - from.X), Y: Math.Abs(to.Y - from.Y));

            if (step.X < 1e-6 || step.Y < 1e-6 || Math.Abs(step.X - step.Y) > 1e-6)
                continue;

            var (was, now) = (fitted[at].From, fitted[at].To);

            Assert.True(
                Math.Abs(Math.Abs(now.X - was.X) - Math.Abs(now.Y - was.Y)) < 1e-6,
                string.Create(CultureInfo.InvariantCulture,
                    $"{name} в клетке {cell}: диагональ стала {Math.Abs(now.X - was.X)}×{Math.Abs(now.Y - was.Y)}"));
        }
    }

    /// <summary>Путь иконки в этой клетке — строкой, как его собрал генератор.</summary>
    private static string Drawn(string family, string name, int cell)
    {
        var paths = AxIcons.Drawn(Resolve(family, name));
        var at = AxIcons.Sizing.ToList().IndexOf(cell);

        Assert.True(at >= 0 && at < paths.Count, $"{name}: клетки {cell} у набора нет");

        return paths[at];
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

    /// <summary>Концы открытых осевых штрихов и координата вдоль штриха у каждого.</summary>
    /// <remarks>
    /// Подпуть, закрытый <c>Z</c>, концов не имеет: его точка <c>M</c> — такой же
    /// угол, как остальные. Считать её концом значило бы спрашивать с каждой
    /// рамки то, что спрашивают только с открытого штриха.
    /// </remarks>
    private static IEnumerable<((double X, double Y) Point, (double X, double Y) Own, double Along)> Ends(
        List<(char Letter, double X, double Y)> commands)
    {
        var start = 0;

        for (var at = 1; at <= commands.Count; at++)
        {
            var boundary = at == commands.Count || commands[at].Letter is 'M' or 'Z';

            if (!boundary)
                continue;

            // [start, at) — один подпуть; at стоит на следующем M, на Z или за концом.
            var closed = at < commands.Count && commands[at].Letter == 'Z';
            var last = at - 1;

            if (!closed && last > start)
            {
                foreach (var (point, next) in new[] { (start, start + 1), (last, last - 1) })
                {
                    var (letter, x, y) = commands[point];
                    var other = commands[next];

                    if (letter is not ('M' or 'L' or 'H' or 'V') || other.Letter is not ('M' or 'L' or 'H' or 'V'))
                        continue;

                    var horizontal = Math.Abs(other.Y - y) < 1e-6 && Math.Abs(other.X - x) > 1e-6;
                    var vertical = Math.Abs(other.X - x) < 1e-6 && Math.Abs(other.Y - y) > 1e-6;

                    if (horizontal)
                        yield return ((x, y), (other.X, other.Y), x);
                    else if (vertical)
                        yield return ((x, y), (other.X, other.Y), y);
                }
            }

            start = closed ? at + 1 : at;
        }
    }

    /// <summary>Лежит ли точка на каком-нибудь прямом отрезке, кроме своего собственного.</summary>
    /// <remarks>
    /// Не в счёт только собственный отрезок — тот, что и кончается в этой точке.
    /// Чужой отрезок, кончающийся здесь же, закрывает пиксель: угол рамки — такая
    /// же опора для конца штриха, как её сторона.
    /// </remarks>
    private static bool Touches(
        (double X, double Y) point,
        (double X, double Y) own,
        List<((double X, double Y) From, (double X, double Y) To)> segments)
    {
        foreach (var (from, to) in segments)
        {
            if ((Same(from, point) && Same(to, own)) || (Same(to, point) && Same(from, own)))
                continue;

            var cross = ((to.X - from.X) * (point.Y - from.Y)) - ((to.Y - from.Y) * (point.X - from.X));
            var inside = point.X >= Math.Min(from.X, to.X) - 1e-6 && point.X <= Math.Max(from.X, to.X) + 1e-6
                && point.Y >= Math.Min(from.Y, to.Y) - 1e-6 && point.Y <= Math.Max(from.Y, to.Y) + 1e-6;

            if (Math.Abs(cross) < 1e-6 && inside)
                return true;
        }

        return false;
    }

    private static bool Same((double X, double Y) a, (double X, double Y) b) =>
        Math.Abs(a.X - b.X) < 1e-6 && Math.Abs(a.Y - b.Y) < 1e-6;

    private static string Data(string svg) =>
        Regex.Match(svg, @"<path d=""([^""]+)""").Groups[1].Value;

    private static Geometry Resolve(string family, string name) =>
        (Geometry)typeof(AxIcons).GetProperty(name, BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;

    private static Avalonia.Rect Round(Avalonia.Rect rect) => new(
        Math.Round(rect.X, 3),
        Math.Round(rect.Y, 3),
        Math.Round(rect.Width, 3),
        Math.Round(rect.Height, 3));

    private static IEnumerable<(string Family, string Name, string File)> Icons()
    {
        foreach (var file in Directory.EnumerateFiles(System.IO.Path.Combine(Root(), "actions"), "*.svg").Order(StringComparer.Ordinal))
            yield return ("actions", System.IO.Path.GetFileNameWithoutExtension(file), file);
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
