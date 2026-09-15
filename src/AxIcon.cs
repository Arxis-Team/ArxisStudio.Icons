using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ArxisStudio.Icons;

/// <summary>Размер иконки.</summary>
public enum AxIconSize
{
    /// <summary>Клетка набора, 16: иконка кнопки, строки, вкладки.</summary>
    Normal,

    /// <summary>Мелкая: шеврон в тесной строке, крестик внутри поля.</summary>
    Small,
}

/// <summary>
/// Контурная иконка студии: SVG-путь в системе координат 16×16, обведённый
/// <see cref="TemplatedControl.Foreground"/>. Пути лежат в наборе студии; плагин
/// может передать свой.
/// </summary>
/// <remarks>
/// Набор нарисован под пиксель сетки 16: оси штрихов стоят на полуклетках, концы —
/// на целых. Когда клетка сетки занимает целое число пикселей экрана — 32 точки на
/// обычном экране, 16 при масштабе 200%, — обводка рисуется целым числом пикселей
/// той же чётности, что клетка, и контур ложится в пиксели без полупрозрачной
/// каймы: при чётной клетке оси стоят на границах пикселей и резкая обводка чётная,
/// при нечётной — в центрах, и резкая нечётная. При 200% это ровно одна клетка
/// сетки вместо заданных 1.2. Замер витрины по всему набору: доля пикселей,
/// закрашенных целиком, при 200% — 0.48 у обводки 1.2 и 0.77 у обводки в клетку,
/// при 300% — 0.59 и 0.87.
/// <para>
/// Одна клетка на пиксель — масштаб, под который набор нарисован, — и там обводка
/// остаётся заданной: кромка в десятую пикселя у 1.2 и есть задуманный вес.
/// Дробные масштабы — 125, 150, 175% — правило не трогает: полуклетки ложатся там
/// на четверти пикселя, и никакая толщина их не соберёт, а округлённая только
/// меняет вес. Резкость там даст лишь набор, нарисованный под свою сетку.
/// </para>
/// <para>
/// Силуэт или контур решает путь, а не автор: иконку набора, нарисованную силуэтом,
/// контрол заливает сам — тема видит это псевдоклассом <c>:filled</c>. Свой путь плагин
/// заливает свойством <see cref="IsFilled"/>. Прежде заливку помнил автор, и пять силуэтов
/// набора стояли в студии обведёнными.
/// </para>
/// </remarks>
[TemplatePart("PART_Path", typeof(Shape))]
[PseudoClasses(":filled", ":small")]
public class AxIcon : TemplatedControl
{
    /// <summary>Геометрия пути в координатах 16×16.</summary>
    public static readonly StyledProperty<Geometry?> DataProperty =
        AvaloniaProperty.Register<AxIcon, Geometry?>(nameof(Data));

    /// <summary>Толщина обводки в клетках сетки; по умолчанию 1.2 — вес, под который нарисован набор.</summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<AxIcon, double>(nameof(StrokeThickness), 1.2);

    /// <summary>Заливать свой путь вместо обводки; силуэты набора заливаются и без этого.</summary>
    public static readonly StyledProperty<bool> IsFilledProperty =
        AvaloniaProperty.Register<AxIcon, bool>(nameof(IsFilled));

    /// <summary>Размер иконки.</summary>
    public static readonly StyledProperty<AxIconSize> SizeProperty =
        AvaloniaProperty.Register<AxIcon, AxIconSize>(nameof(Size));

    /// <summary>Сторона клетки, в которой нарисованы пути.</summary>
    private const double Grid = 16;

    private Shape? _path;
    private TopLevel? _window;

    static AxIcon()
    {
        TemplateAppliedEvent.AddClassHandler<AxIcon>((icon, e) =>
        {
            icon._path = e.NameScope.Find<Shape>("PART_Path");
            icon.Snap();
        });

        StrokeThicknessProperty.Changed.AddClassHandler<AxIcon>((icon, _) => icon.Snap());
        BoundsProperty.Changed.AddClassHandler<AxIcon>((icon, _) => icon.Snap());

        DataProperty.Changed.AddClassHandler<AxIcon>((icon, _) => icon.MarkFilled());
        IsFilledProperty.Changed.AddClassHandler<AxIcon>((icon, _) => icon.MarkFilled());
        SizeProperty.Changed.AddClassHandler<AxIcon>((icon, change) =>
            icon.PseudoClasses.Set(":small", change.GetNewValue<AxIconSize>() == AxIconSize.Small));
    }

    /// <summary>Заводит иконку без пути: его дают свойством <see cref="Data"/>.</summary>
    public AxIcon()
    {
        // Масштаб экрана меняется, когда окно переезжает на другой монитор, и
        // толщина, сведённая под прежний, размазалась бы на новом.
        AttachedToVisualTree += (_, _) =>
        {
            _window = TopLevel.GetTopLevel(this);

            if (_window is not null)
                _window.ScalingChanged += OnScalingChanged;

            Snap();
        };

        DetachedFromVisualTree += (_, _) =>
        {
            if (_window is not null)
                _window.ScalingChanged -= OnScalingChanged;

            _window = null;
        };
    }

    /// <inheritdoc cref="DataProperty"/>
    public Geometry? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <inheritdoc cref="StrokeThicknessProperty"/>
    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    /// <inheritdoc cref="IsFilledProperty"/>
    public bool IsFilled
    {
        get => GetValue(IsFilledProperty);
        set => SetValue(IsFilledProperty, value);
    }

    /// <inheritdoc cref="SizeProperty"/>
    public AxIconSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Толщина обводки в клетках сетки, сведённая к пикселям экрана там, где это делает контур резким.
    /// </summary>
    /// <param name="stroke">Заданная толщина в клетках.</param>
    /// <param name="pixels">Сколько пикселей экрана занимает одна клетка.</param>
    internal static double Rendered(double stroke, double pixels)
    {
        var whole = Math.Round(pixels);

        if (whole < 2 || Math.Abs(pixels - whole) > 0.001)
            return stroke;

        // Ось на полуклетке стоит на границе пикселя, когда клетка чётная, и в его
        // центре, когда нечётная: резкая обводка той же чётности. Из двух таких
        // берётся ближняя к заданной, при равенстве — тоньше.
        var parity = (int)whole % 2;
        var below = Math.Floor(stroke * whole);

        if ((int)below % 2 != parity)
            below--;

        var above = below + 2;
        var device = Math.Max(stroke * whole - below <= above - stroke * whole ? below : above, parity == 0 ? 2 : 1);

        return device / whole;
    }

    private void OnScalingChanged(object? sender, EventArgs e) => Snap();

    private void MarkFilled() => PseudoClasses.Set(":filled", IsFilled || AxIcons.IsFilled(Data));

    private void Snap()
    {
        if (_path is null)
            return;

        // Путь вписан в рамку с сохранением пропорций, поэтому клетку задаёт
        // меньшая сторона. Толщина ставится пути, а не свойству: заданная остаётся
        // той, что назвал хозяин, и сводится заново при каждом размере и масштабе.
        var scaling = _window?.RenderScaling ?? TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        var pixels = Math.Min(Bounds.Width, Bounds.Height) / Grid * scaling;

        _path.StrokeThickness = Rendered(StrokeThickness, pixels);
    }
}
