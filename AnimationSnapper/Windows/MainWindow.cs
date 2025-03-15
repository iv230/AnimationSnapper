using System;
using System.Linq;
using System.Numerics;
using AnimationSnapper.Config;
using AnimationSnapper.Service;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace AnimationSnapper.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private readonly Configuration configuration;
    private readonly HousingService housingService;
    private readonly SnappingService snappingService;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, HousingService housingService, SnappingService snappingService) 
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.plugin = plugin;
        configuration = plugin.Configuration;
        this.housingService = housingService;
        this.snappingService = snappingService;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text("Select a Snapping:");

        if (plugin.Configuration.Snappings.Count > 0)
        {
            // Liste des snappings avec sélection via SnappingService
            var snappingNames = plugin.Configuration.Snappings.Select(s => s.Name).ToArray();
            var selectedIndex = plugin.Configuration.Snappings.IndexOf(snappingService.Selected);

            if (ImGui.ListBox("##SnappingList", ref selectedIndex, snappingNames, snappingNames.Length))
            {
                snappingService.Select(configuration.Snappings[selectedIndex]);
            }

            var selectedSnapping = snappingService.Selected;
            if (selectedSnapping != null)
            {
                // Obtenir la position réelle du joueur
                Vector3 playerPosition = housingService.GetClosestItemDistance(197799u);
                
                // Différences en X et Z
                var offsetX = playerPosition.X - selectedSnapping.OffsetX;
                var offsetZ = playerPosition.Z - selectedSnapping.OffsetY;

                ImGui.Separator();
                ImGui.Text($"Selected Snapping: {selectedSnapping.Name}");
                ImGui.Text($"Offset Difference: X={offsetX:F2}, Z={offsetZ:F2}");
            }
        }
        else
        {
            ImGui.Text("No snappings available.");
        }
    }
}
