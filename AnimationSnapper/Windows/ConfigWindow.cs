using System;
using System.Numerics;
using AnimationSnapper.Config;
using AnimationSnapper.Model;
using AnimationSnapper.Service;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AnimationSnapper.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private readonly HousingService housingService;
    private readonly SnappingService snappingService;
    private string newSnappingName = "";
    private Vector3 closestItemPosition;

    public ConfigWindow(Plugin plugin, HousingService housingService, SnappingService snappingService) : base("Configuration###ConfigWindow")
    {
        Flags = ImGuiWindowFlags.AlwaysAutoResize;

        configuration = plugin.Configuration;
        this.housingService = housingService;
        this.snappingService = snappingService;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
    }

    public override void Draw()
    {
        closestItemPosition = housingService.GetClosestItemDistance(197799u);

        ImGui.Text("Snapping List:");

        if (ImGui.BeginTable("##SnappingTable", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Offset X");
            ImGui.TableSetupColumn("Offset Y");
            ImGui.TableSetupColumn("Actions");
            ImGui.TableHeadersRow();

            for (var i = 0; i < configuration.Snappings.Count; i++)
            {
                var snapping = configuration.Snappings[i];
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text(snapping.Name);

                ImGui.TableNextColumn();
                ImGui.Text(snapping.OffsetX.ToString("F2"));

                ImGui.TableNextColumn();
                ImGui.Text(snapping.OffsetY.ToString("F2"));

                ImGui.TableNextColumn();
                if (ImGui.Button($"Delete##{i}"))
                {
                    Plugin.Log.Information($"Deleting snapping: {snapping.Name}");
                    snappingService.RemoveSnapping(snapping);
                }
            }
            ImGui.EndTable();
        }

        ImGui.Separator();
        ImGui.Text("Add New Snapping");
        ImGui.InputText("Name", ref newSnappingName, 100);

        ImGui.Text($"Offset X: {closestItemPosition.X:F2}");
        ImGui.Text($"Offset Z: {closestItemPosition.Z:F2}");

        if (ImGui.Button("Add Snapping"))
        {
            if (!string.IsNullOrWhiteSpace(newSnappingName))
            {
                Plugin.Log.Information($"Adding new snapping: {newSnappingName} with X={closestItemPosition.X}, Z={closestItemPosition.Z}");
                snappingService.AddSnapping(new Snapping { Name = newSnappingName, OffsetX = closestItemPosition.X, OffsetY = closestItemPosition.Z });
                configuration.Save();
                newSnappingName = "";
            }
        }
    }
}
