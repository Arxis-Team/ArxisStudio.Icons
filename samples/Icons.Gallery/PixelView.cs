using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Icons.Gallery;

/// <summary>
/// Иконка, увеличенная до пикселей: каждый пиксель — квадрат своей плотности.
/// </summary>
/// <remarks>
/// Это главный прибор витрины. На 16 px глаз видит «мутновато», а здесь видно
/// причину: сплошное ядро штриха или две полупрозрачные половинки по соседним
/// пикселям. Сетка нарисована по границам пикселей, а не по координатам пути,
/// — расхождение между ними и есть то, что размывает контур.
/// </remarks>
public sealed class PixelView : Control
{
    /// <summary>Снимок, который показываем.</summary>
    public static readonly StyledProperty<Sharpness?> SourceProperty =
        AvaloniaProperty.Register<PixelView, Sharpness?>(nameof(Source));

    /// <summary>Во сколько раз увеличен пиксель.</summary>
    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<PixelView, double>(nameof(Zoom), 20d);

    static PixelView()
    {
        AffectsRender<PixelView>(SourceProperty, ZoomProperty);
        AffectsMeasure<PixelView>(SourceProperty, ZoomProperty);
    }

    /// <inheritdoc cref="SourceProperty"/>
    public Sharpness? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <inheritdoc cref="ZoomProperty"/>
    public double Zoom
    {
        get => GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize) =>
        Source is { } source ? new Size(source.Size * Zoom, source.Size * Zoom) : default;

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        if (Source is not { } source)
            return;

        var side = source.Size * Zoom;

        context.FillRectangle(Brushes.Black, new Rect(0, 0, side, side));

        for (var y = 0; y < source.Size; y++)
        {
            for (var x = 0; x < source.Size; x++)
            {
                var alpha = source.Alpha[(y * source.Size) + x];

                if (alpha == 0)
                    continue;

                context.FillRectangle(
                    new SolidColorBrush(Color.FromRgb(alpha, alpha, alpha)),
                    new Rect(x * Zoom, y * Zoom, Zoom, Zoom));
            }
        }

        // Сетка по границам пикселей: каждая четвёртая линия ярче, чтобы
        // считать клетки, не тыкая пальцем в экран.
        for (var at = 0; at <= source.Size; at++)
        {
            var pen = new Pen(at % 4 == 0 ? Brushes.DimGray : Brushes.DarkSlateGray, 1);
            var offset = at * Zoom;

            context.DrawLine(pen, new Point(offset, 0), new Point(offset, side));
            context.DrawLine(pen, new Point(0, offset), new Point(side, offset));
        }
    }
}
