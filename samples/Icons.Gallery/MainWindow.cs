using ArxisStudio.Icons;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace Icons.Gallery;

/// <summary>
/// Витрина набора: слева весь набор, справа выбранная иконка вблизи.
/// </summary>
/// <remarks>
/// Окно собрано кодом, а не разметкой, и это осознанно: витрина строит сто
/// шестьдесят однотипных плиток и столько же строк отчёта — в разметке это
/// вышло бы шаблоном к шаблону, а править надо не оформление, а меру.
/// <para>
/// Всё, что витрина посчитала, лежит в подписях с именами: <c>Solid</c>,
/// <c>Faint</c>, <c>Worst</c>. Инструменты читают их так же, как человек, —
/// поэтому разбор набора не требует ни снимков экрана, ни глаза.
/// </para>
/// </remarks>
public sealed class MainWindow : Window
{
    private static readonly double[] Sizes = [12, 16, 24, 32, 64];
    private static readonly double[] Strokes = [1.0, 1.2, 1.5];

    private readonly IReadOnlyList<Icon> _icons = Catalogue.Load();
    private readonly StackPanel _detail = new() { Spacing = 12, Margin = new Thickness(20) };
    private readonly WrapPanel _tiles = new();
    private readonly TextBlock _worst = new() { Name = "Worst", FontFamily = new FontFamily("Consolas") };

    // Та же обводка, что у студии: витрина обязана показывать отгружаемое,
    // а не удобное. Соседние значения рядом — чтобы было с чем сравнить.
    private double _stroke = 1.2;
    private Icon? _chosen;

    public MainWindow()
    {
        Title = "ArxisStudio.Icons — витрина";
        Width = 1280;
        Height = 860;
        Background = new SolidColorBrush(Color.Parse("#1E1F22"));

        Fill();

        Content = new DockPanel
        {
            Children =
            {
                Top(),
                new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("2*,3*"),
                    Children =
                    {
                        Left(),
                        Right(),
                    },
                },
            },
        };

        Show(_icons[0]);
    }

    /// <summary>Полоса управления: чем набор мерить.</summary>
    private Control Top()
    {
        var strokes = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };

        foreach (var stroke in Strokes)
        {
            var button = new RadioButton
            {
                Name = "Stroke" + stroke.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture).Replace(".", string.Empty, StringComparison.Ordinal),
                Content = stroke.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture),
                IsChecked = Math.Abs(stroke - _stroke) < 0.01,
                GroupName = "stroke",
                Foreground = Brushes.Gainsboro,
            };

            button.IsCheckedChanged += (_, _) =>
            {
                if (button.IsChecked != true)
                    return;

                _stroke = stroke;

                Fill();

                if (_chosen is { } icon)
                    Show(icon);
            };

            strokes.Children.Add(button);
        }

        var bar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 16,
            Margin = new Thickness(20, 14),
            Children =
            {
                new TextBlock { Text = "Обводка", Foreground = Brushes.Gray, VerticalAlignment = VerticalAlignment.Center },
                strokes,
                new TextBlock
                {
                    Name = "Total",
                    Text = $"иконок: {_icons.Count}",
                    Foreground = Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            },
        };

        DockPanel.SetDock(bar, Dock.Top);

        return bar;
    }

    /// <summary>Весь набор плитками — и отчёт под ним.</summary>
    private Control Left()
    {
        var panel = new DockPanel { Margin = new Thickness(20, 0, 10, 20) };

        var report = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                new TextBlock { Text = "Худшие по размазне", Foreground = Brushes.Gray },
                new ScrollViewer { Height = 200, Content = _worst },
            },
        };

        DockPanel.SetDock(report, Dock.Bottom);
        panel.Children.Add(report);
        panel.Children.Add(new ScrollViewer { Content = _tiles });

        Grid.SetColumn(panel, 0);

        return panel;
    }

    /// <summary>Выбранная иконка вблизи.</summary>
    private Control Right()
    {
        var scroll = new ScrollViewer { Content = _detail };

        Grid.SetColumn(scroll, 1);

        return scroll;
    }

    /// <summary>Раскладывает набор плитками и пересчитывает отчёт.</summary>
    private void Fill()
    {
        _tiles.Children.Clear();

        var measured = new List<(Icon Icon, Sharpness Sharpness)>();

        foreach (var icon in _icons)
        {
            var sharpness = Sharpness.Measure(icon.Geometry, _stroke, icon.Filled);

            measured.Add((icon, sharpness));

            var tile = new Button
            {
                Name = "Tile" + icon.Name,
                Width = 76,
                Height = 76,
                Margin = new Thickness(2),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Content = new StackPanel
                {
                    Spacing = 6,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new AxIcon { Data = icon.Geometry, IsFilled = icon.Filled, StrokeThickness = _stroke },
                        new TextBlock
                        {
                            Text = icon.Name,
                            FontSize = 9,
                            Foreground = Brushes.Gray,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            MaxWidth = 70,
                            TextAlignment = TextAlignment.Center,
                        },
                    },
                },
            };

            tile.Click += (_, _) => Show(icon);

            _tiles.Children.Add(tile);
        }

        _worst.Text = string.Join(
            Environment.NewLine,
            measured
                .OrderByDescending(pair => pair.Sharpness.FaintShare)
                .ThenBy(pair => pair.Sharpness.SolidShare)
                .Take(20)
                .Select(pair => $"{pair.Icon.Name,-22} сплошных {pair.Sharpness.SolidShare,6:P0}   еле видных {pair.Sharpness.FaintShare,6:P0}"));
    }

    /// <summary>Показывает иконку вблизи: размеры, пиксели и числа.</summary>
    private void Show(Icon icon)
    {
        _chosen = icon;

        var sharpness = Sharpness.Measure(icon.Geometry, _stroke, icon.Filled);
        var sizes = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 20, VerticalAlignment = VerticalAlignment.Bottom };

        foreach (var size in Sizes)
        {
            sizes.Children.Add(new StackPanel
            {
                Spacing = 6,
                Children =
                {
                    new AxIcon
                    {
                        Data = icon.Geometry,
                        IsFilled = icon.Filled,
                        StrokeThickness = _stroke,
                        Width = size,
                        Height = size,
                        Foreground = Brushes.Gainsboro,
                    },
                    new TextBlock { Text = size.ToString("0", System.Globalization.CultureInfo.InvariantCulture), FontSize = 10, Foreground = Brushes.DimGray },
                },
            });
        }

        _detail.Children.Clear();

        _detail.Children.Add(new TextBlock
        {
            Name = "Chosen",
            Text = icon.Name,
            FontSize = 20,
            Foreground = Brushes.Gainsboro,
        });

        _detail.Children.Add(new TextBlock
        {
            Name = "About",
            Text = $"{icon.Family} · {(icon.Group.Length > 0 ? icon.Group + " · " : string.Empty)}{icon.Title}",
            Foreground = Brushes.Gray,
            TextWrapping = TextWrapping.Wrap,
        });

        _detail.Children.Add(sizes);
        _detail.Children.Add(new PixelView { Source = sharpness, Zoom = 24 });

        _detail.Children.Add(new TextBlock
        {
            Name = "Solid",
            Text = $"сплошных пикселей: {sharpness.Solid} из {sharpness.Covered} ({sharpness.SolidShare:P0})",
            Foreground = Brushes.Gainsboro,
        });

        _detail.Children.Add(new TextBlock
        {
            Name = "Faint",
            Text = $"еле видных (меньше четверти): {sharpness.Faint} ({sharpness.FaintShare:P0})",
            Foreground = Brushes.Gainsboro,
        });

        _detail.Children.Add(new TextBlock
        {
            Name = "Path",
            Text = icon.Geometry.ToString(),
            FontFamily = new FontFamily("Consolas"),
            FontSize = 11,
            Foreground = Brushes.DimGray,
            TextWrapping = TextWrapping.Wrap,
        });
    }
}
