using System.Text;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Common.Math;

namespace AnimationSnapper.Service;

public class HousingService
{
    public unsafe Vector3 GetClosestItemDistance(uint itemBaseId) // 197799u
    {
        var allFurniture = HousingManager.Instance()->IndoorTerritory->HousingObjectManager.Objects;
    
        var currentCharacter = (BattleChara*)(Plugin.ClientState.LocalPlayer?.Address ?? 0);
        if (currentCharacter == null)
        {
            Plugin.Log.Error("Could not get local player");
            return new Vector3();
        }

        var minDistance = new Vector3();
        foreach (var housingFurniturePtr in allFurniture)
        {
            var housingFurniture = housingFurniturePtr.Value;

            if (housingFurniture != null && housingFurniture->BaseId == itemBaseId)
            {
                var dist = (currentCharacter->Position - housingFurniture->Position);

                if (dist.Magnitude < minDistance.Magnitude)
                {
                    minDistance = dist;
                }
            }
        }

        return minDistance;
    }

}
