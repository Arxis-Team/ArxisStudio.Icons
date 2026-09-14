using Xunit;

namespace ArxisStudio.Icons.Tests;

/// <summary>
/// Толщина обводки на крупном масштабе экрана: целое число пикселей той же чётности, что клетка.
/// </summary>
/// <remarks>
/// Набор нарисован под пиксель сетки 16: оси штрихов на полуклетках. Когда клетка занимает целое
/// число пикселей, ось стоит на границе пикселя при чётной клетке и в его центре при нечётной — и
/// контур без каймы даёт только обводка той же чётности. Здесь правило проверяется числом; что
/// контур от этого действительно резче, мерит отрисовка в репозитории студии.
/// </remarks>
public class StrokeCellTests
{
    /// <summary>Клетка в пиксель — масштаб, под который набор нарисован: обводка та, что задана.</summary>
    [Fact]
    public void At_one_pixel_a_cell_the_stroke_is_the_one_given() =>
        Assert.Equal(1.2, AxIcon.Rendered(1.2, 1));

    /// <summary>
    /// На дробном масштабе обводка не трогается: полуклетки ложатся на четверти пикселя, и толщиной
    /// их не собрать.
    /// </summary>
    [Theory]
    [InlineData(0.75)]
    [InlineData(1.25)]
    [InlineData(1.5)]
    [InlineData(1.75)]
    [InlineData(2.5)]
    public void On_a_fractional_scale_the_stroke_is_left_alone(double pixels) =>
        Assert.Equal(1.2, AxIcon.Rendered(1.2, pixels));

    /// <summary>Заданные 1.2 на целом масштабе от двух — ровно одна клетка.</summary>
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void The_stroke_of_the_set_becomes_one_cell_on_a_whole_scale(double pixels) =>
        Assert.Equal(1.0, AxIcon.Rendered(1.2, pixels), 6);

    /// <summary>
    /// На любом целом масштабе от двух обводка — целое число пикселей чётности клетки, ближайшее к
    /// заданному, и не тоньше одного пикселя.
    /// </summary>
    [Fact]
    public void A_whole_scale_gives_the_nearest_whole_pixels_of_the_cell_parity()
    {
        for (var pixels = 2; pixels <= 8; pixels++)
        {
            for (var stroke = 0.5; stroke <= 3.0; stroke += 0.1)
            {
                var device = AxIcon.Rendered(stroke, pixels) * pixels;
                var rounded = Math.Round(device);

                Assert.True(Math.Abs(device - rounded) < 1e-9, $"клетка {pixels}, обводка {stroke:F1}: {device} пикселя — не целое");
                Assert.True((int)rounded % 2 == pixels % 2, $"клетка {pixels}, обводка {stroke:F1}: {rounded} пикселей не той чётности");
                Assert.True(rounded >= 1, $"клетка {pixels}, обводка {stroke:F1}: обводка пропала");
                Assert.True(
                    Math.Abs(rounded - stroke * pixels) <= 1 + 1e-9 || rounded <= 2,
                    $"клетка {pixels}, обводка {stroke:F1}: {rounded} пикселей вместо ближайших к {stroke * pixels:F2}");
            }
        }
    }
}
