using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Icons.Gallery;

/// <summary>
/// Во что превращается путь, когда его кладут на пиксели.
/// </summary>
/// <remarks>
/// Резкость контура — не мнение, а число: обводка, попавшая между пикселями,
/// размазывается по двум-трём полупрозрачными долями, а попавшая в пиксель
/// целиком даёт сплошное ядро. Глазом на 16 px разницу видно плохо, поэтому
/// витрина её считает, а не показывает.
/// <para>
/// Мера снимается с настоящей отрисовки, а не из геометрии: считать доли по
/// координатам значило бы повторять в столбик то, что растеризатор делает
/// иначе — со скруглёнными концами, стыками и сглаживанием.
/// </para>
/// </remarks>
public sealed class Sharpness
{
    private Sharpness(byte[] alpha, int size)
    {
        Alpha = alpha;
        Size = size;

        foreach (var value in alpha)
        {
            if (value == 0)
                continue;

            Covered++;

            if (value >= 250)
                Solid++;
            else if (value < 64)
                Faint++;
        }
    }

    /// <summary>Прозрачность каждого пикселя, строка за строкой.</summary>
    public byte[] Alpha { get; }

    /// <summary>Сторона снимка в пикселях.</summary>
    public int Size { get; }

    /// <summary>Сколько пикселей чернила задели вообще.</summary>
    public int Covered { get; }

    /// <summary>Сколько закрашено целиком: это и есть ядро штриха.</summary>
    public int Solid { get; }

    /// <summary>Сколько задето еле-еле — меньше четверти. Это и есть размазня.</summary>
    public int Faint { get; }

    /// <summary>Доля сплошных пикселей среди задетых: чем выше, тем резче.</summary>
    public double SolidShare => Covered == 0 ? 0 : (double)Solid / Covered;

    /// <summary>Доля еле задетых среди задетых: чем выше, тем грязнее контур.</summary>
    public double FaintShare => Covered == 0 ? 0 : (double)Faint / Covered;

    /// <summary>
    /// Рисует путь в клетку заданного размера и снимает прозрачность попиксельно.
    /// </summary>
    /// <param name="geometry">Путь в координатах 16×16.</param>
    /// <param name="stroke">Толщина обводки в тех же координатах.</param>
    /// <param name="filled">Силуэт вместо контура.</param>
    /// <param name="size">Сторона снимка; 16 — размер, которым иконку показывают.</param>
    public static Sharpness Measure(Geometry geometry, double stroke, bool filled, int size = 16)
    {
        ArgumentNullException.ThrowIfNull(geometry);

        using var target = new RenderTargetBitmap(new PixelSize(size, size), new Vector(96, 96));

        using (var context = target.CreateDrawingContext())
        {
            // Клетка пути всегда 16×16; снимок бывает крупнее, и тогда путь
            // растягивается ровно так же, как его растягивает Viewbox витрины.
            using var _ = context.PushTransform(Matrix.CreateScale(size / 16d, size / 16d));

            if (filled)
            {
                context.DrawGeometry(Brushes.White, null, geometry);
            }
            else
            {
                context.DrawGeometry(null, new Pen(Brushes.White, stroke)
                {
                    LineCap = PenLineCap.Round,
                    LineJoin = PenLineJoin.Round,
                }, geometry);
            }
        }

        return new Sharpness(Read(target, size), size);
    }

    private static byte[] Read(RenderTargetBitmap target, int size)
    {
        var stride = size * 4;
        var bytes = new byte[stride * size];
        var buffer = Marshal.AllocHGlobal(bytes.Length);

        try
        {
            target.CopyPixels(new PixelRect(0, 0, size, size), buffer, bytes.Length, stride);
            Marshal.Copy(buffer, bytes, 0, bytes.Length);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

        // Формат BGRA: прозрачность — четвёртый байт каждого пикселя.
        var alpha = new byte[size * size];

        for (var at = 0; at < alpha.Length; at++)
            alpha[at] = bytes[(at * 4) + 3];

        return alpha;
    }
}
