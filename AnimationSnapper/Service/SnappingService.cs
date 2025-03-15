using System.Collections.Generic;
using AnimationSnapper.Config;
using AnimationSnapper.Model;

namespace AnimationSnapper.Service;

public class SnappingService
{
    public Snapping Selected;
    private readonly Configuration configuration;

    public SnappingService(Configuration configuration)
    {
        this.configuration = configuration;
        Selected = this.configuration.Snappings[0];
    }

    public List<Snapping> GetSnappings()
    {
        return configuration.Snappings;
    }

    public void Select(Snapping snapping)
    {
        Selected = snapping;
    }
    
    public void AddSnapping(Snapping snapping)
    {
        configuration.Snappings.Add(snapping);
        configuration.Save();
    }

    public void RemoveSnapping(Snapping snapping)
    {
        configuration.Snappings.Remove(snapping);
        configuration.Save();
    }
}
