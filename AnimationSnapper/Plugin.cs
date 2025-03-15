using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using AnimationSnapper.Config;
using AnimationSnapper.Service;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using AnimationSnapper.Windows;

namespace AnimationSnapper;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    private const string MainCommandName = "/snapper";
    private const string ConfigCommandName = "/snapperconf";
    private const string DebugCommandName = "/snapperdebug";

    public Configuration Configuration { get; init; }
    
    public HousingService HousingService { get; init; }

    public SnappingService SnappingService { get; init; }

    public readonly WindowSystem WindowSystem = new("AnimationSnapper");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        HousingService = new HousingService();
        SnappingService = new SnappingService(Configuration);

        ConfigWindow = new ConfigWindow(this, HousingService, SnappingService);
        MainWindow = new MainWindow(this, HousingService, SnappingService);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(MainCommandName, new CommandInfo(OnMainCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });
        CommandManager.AddHandler(ConfigCommandName, new CommandInfo(OnConfCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });
        CommandManager.AddHandler(DebugCommandName, new CommandInfo(OnDebugCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [AnimationSnapper] ===A cool log message from Sample Plugin===
        Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(MainCommandName);
    }

    private void OnMainCommand(string command, string args)
    {
        ToggleMainUI();
    }
    
    private void OnConfCommand(string command, string args)
    {
        ToggleConfigUI();
    }

    private void OnDebugCommand(string command, string args)
    {
        var vector = HousingService.GetClosestItemDistance(197799u);
        Log.Debug(vector.ToString());
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
