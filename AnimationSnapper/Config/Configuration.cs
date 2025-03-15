using System;
using System.Collections.Generic;
using AnimationSnapper.Model;
using Dalamud.Configuration;

namespace AnimationSnapper.Config;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public List<Snapping> Snappings { get; set; } = [];

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
