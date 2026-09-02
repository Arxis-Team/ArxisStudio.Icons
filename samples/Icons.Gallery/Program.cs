using System.Globalization;
using Avalonia;

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

        var stroke = Stroke(args);
        var icon = Catalogue.Load().Single(candidate => candidate.Name == name);
        var sharpness = Sharpness.Measure(icon.Geometry, stroke, icon.Filled);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"{icon.Name}, обводка {stroke}"));

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
            : 1.0;
    }

    /// <summary>
    /// Печатает меру резкости по всему набору и выходит.
    /// </summary>
    /// <remarks>
    /// Окно показывает двадцать худших, а править набор надо по всему списку —
    /// и сравнивать до и после правки. Глазами по витрине это делать негде:
    /// сто шестьдесят одна иконка, и разница в один пиксель.
    /// <para>
    /// Приложение поднимается без окна: растеризатору нужна платформа, а
    /// человеку в этом режиме — нет.
    /// </para>
    /// </remarks>
    private static void Report(string[] args)
    {
        BuildAvaloniaApp().SetupWithoutStarting();

        var stroke = Stroke(args);

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("имя\tсемейство\tзадето\tсплошных\tеле видных\tдоля сплошных\tдоля еле видных");

        foreach (var icon in Catalogue.Load())
        {
            var sharpness = Sharpness.Measure(icon.Geometry, stroke, icon.Filled);

            Console.WriteLine(string.Create(
                CultureInfo.InvariantCulture,
                $"{icon.Name}\t{icon.Family}\t{sharpness.Covered}\t{sharpness.Solid}\t{sharpness.Faint}\t{sharpness.SolidShare:F3}\t{sharpness.FaintShare:F3}"));
        }
    }
}
