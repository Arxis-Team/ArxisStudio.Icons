using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
#if DEBUG
using AvaDevTools;
#endif

namespace Icons.Gallery;

public class App : Application
{
    /// <summary>Порт конечной точки MCP витрины: у студии 5171, у галереи контролов 5172.</summary>
    private const int McpPort = 5173;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();

            // Инструменты — только к окну. В режиме отчёта окна нет, и порт
            // занимать незачем: мерить набор можно, пока витрина открыта.
            AttachTools();
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Поднимает инструменты отладки интерфейса поверх витрины.
    /// </summary>
    /// <remarks>
    /// Витрина — то место, где иконку смотрят вблизи, и смотреть её глазами по
    /// снимку экрана значит мерить линейкой по фотографии. Инструменты живут в
    /// том же процессе и отвечают о тех же живых объектах: дерево, действующие
    /// значения свойств, снимок отдельного элемента. Замеры резкости витрина
    /// кладёт в подписи — их видно и человеку, и инструменту.
    ///
    /// Конечная точка поднимается сразу, а не по щелчку в окне инструментов:
    /// подключаются к уже запущенной витрине, и ждать, пока кто-то откроет
    /// окно, означало бы не подключиться вовсе.
    /// </remarks>
    private static void AttachTools()
    {
#if DEBUG
        Current!.AttachAvaDevTools(new DevToolsOptions
        {
            McpServer = true,
            McpPort = McpPort,
            McpAllowHold = true,
            McpAllowInput = true,
        });
#endif
    }
}
