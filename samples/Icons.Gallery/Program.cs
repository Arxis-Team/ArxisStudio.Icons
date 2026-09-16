using System.Globalization;
using ArxisStudio.Icons;
using Avalonia;
using Avalonia.Media;

namespace Icons.Gallery;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Contains("--report", StringComparer.Ordinal))
        {
            Report(args);
            return;
        }

        if (Array.IndexOf(args, "--dump") is var at and >= 0 && at + 1 < args.Length)
        {
            Dump(args, args[at + 1]);
            return;
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder
        .Configure<App>()
        .UsePlatformDetect();

    /// <summary>
    /// Печатает одну иконку попиксельно: чем плотнее клетка, тем гуще знак.
    /// </summary>
    /// <remarks>
    /// Доли в отчёте говорят, что иконка мутная, но не говорят почему. Здесь
    /// видно причину: где штрих лёг в пиксель целиком, а где разошёлся на две
    /// половины.
    /// </remarks>
    private static void Dump(string[] args, string name)
    {
        BuildAvaloniaApp().SetupWithoutStarting();

        var icon = Catalogue.Load().Single(candidate => candidate.Name == name);
        var (geometry, stroke, pixels, cell) = Chosen(args, icon, Stroke(args));
        var sharpness = Sharpness.Measure(geometry, stroke, icon.Filled, pixels, cell);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"{icon.Name}, клетка {cell}, {pixels} пикселей, обводка {stroke}"));

        for (var y = 0; y < sharpness.Size; y++)
        {
            var line = new char[sharpness.Size];

            for (var x = 0; x < sharpness.Size; x++)
            {
                var alpha = sharpness.Alpha[(y * sharpness.Size) + x];

                line[x] = alpha switch
                {
                    0 => '.',
                    >= 250 => '#',
                    >= 128 => '+',
                    >= 64 => '-',
                    _ => ':',
                };
            }

            Console.WriteLine(new string(line));
        }

        Console.WriteLine(string.Create(
            CultureInfo.InvariantCulture,
            $"задето {sharpness.Covered}, сплошных {sharpness.Solid}, еле видных {sharpness.Faint}"));
    }

    private static double Stroke(string[] args)
    {
        var at = Array.IndexOf(args, "--stroke");

        return at >= 0 && at + 1 < args.Length
            ? double.Parse(args[at + 1], CultureInfo.InvariantCulture)
            : 1.2;
    }

    /// <summary>Значение числового ключа командной строки.</summary>
    private static double Number(string[] args, string key, double fallback)
    {
        var at = Array.IndexOf(args, key);

        return at >= 0 && at + 1 < args.Length
            ? double.Parse(args[at + 1], CultureInfo.InvariantCulture)
            : fallback;
    }

    /// <summary>
    /// Что и в какой клетке мерить: размер значка в точках и масштаб экрана дают число пикселей.
    /// </summary>
    /// <param name="args">Командная строка: <c>--size</c> и <c>--scale</c>.</param>
    /// <param name="icon">Иконка набора.</param>
    /// <param name="stroke">Обводка, названная в командной строке.</param>
    /// <remarks>
    /// Набор нарисован в восьми клетках, и значок берёт ту, что равна числу занятых им пикселей.
    /// Витрина обязана мерить тот же путь, что возьмёт экран, — иначе мера говорит о рисунке,
    /// которого никто не увидит. В своей клетке единица равна пикселю, и штрих там — ровно один.
    /// </remarks>
    private static (Geometry Geometry, double Stroke, int Pixels, double Cell) Chosen(
        string[] args, Icon icon, double stroke)
    {
        var size = Number(args, "--size", 16);
        var scale = Number(args, "--scale", 1);
        var pixels = (int)Math.Round(size * scale);
        var fitted = Math.Abs(size * scale - pixels) < 0.01 ? AxIcons.Fitted(icon.Geometry, pixels) : null;

        return fitted is null
            ? (icon.Geometry, stroke, pixels, 16)
            : (fitted, 1, pixels, pixels);
    }

    /// <summary>
    /// Печатает меру резкости по всему набору и выходит.
    /// </summary>
    /// <remarks>
    /// Окно показывает двадцать худших, а править набор надо по всему списку —
    /// и сравнивать до и после правки. Глазами по витрине это делать негде:
    /// сто шестьдесят одна иконка, и разница в один пиксель.
    /// <para>
    /// <c>--size</c> и <c>--scale</c> говорят, каким значок будет на экране: размер в точках и
    /// масштаб экрана. Из них выходит число пикселей, а из него — клетка, в которой набор для
    /// этого случая нарисован: <c>--size 12 --scale 1.25</c> — пятнадцать пикселей и клетка 15.
    /// </para>
    /// <para>
    /// Приложение поднимается без окна: растеризатору нужна платформа, а
    /// человеку в этом режиме — нет.
    /// </para>
    /// </remarks>
    private static void Report(string[] args)
    {
        BuildAvaloniaApp().SetupWithoutStarting();

        var given = Stroke(args);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("имя\tсемейство\tклетка\tзадето\tсплошных\tеле видных\tдоля сплошных\tдоля еле видных");

        foreach (var icon in Catalogue.Load())
        {
            var (geometry, stroke, pixels, cell) = Chosen(args, icon, given);
            var sharpness = Sharpness.Measure(geometry, stroke, icon.Filled, pixels, cell);

            Console.WriteLine(string.Create(
                CultureInfo.InvariantCulture,
                $"{icon.Name}\t{icon.Family}\t{cell}\t{sharpness.Covered}\t{sharpness.Solid}\t{sharpness.Faint}\t{sharpness.SolidShare:F3}\t{sharpness.FaintShare:F3}"));
        }
    }
}
