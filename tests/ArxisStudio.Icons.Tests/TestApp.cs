using ArxisStudio.Icons.Tests;
using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(TestApp))]

namespace ArxisStudio.Icons.Tests;

/// <summary>
/// Headless-приложение тестов набора.
/// </summary>
/// <remarks>
/// Без тем: набор — это данные, и проверяется здесь форма пути, а не то, как
/// её рисуют. Отрисовка живёт там, где есть чем рисовать, — в репозитории темы
/// (<c>IconRenderTests</c>): контрол <see cref="AxIcon"/> lookless, и его
/// ControlTheme приходит из <c>ArxisStudio.Themes.Arxis</c>. Приложение всё же
/// нужно: разбор геометрии просит поднятой платформы.
/// </remarks>
public class TestApp : Application
{
    /// <summary>Собирает headless-приложение без единой темы.</summary>
    public static AppBuilder BuildAvaloniaApp() => AppBuilder
        .Configure<TestApp>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
