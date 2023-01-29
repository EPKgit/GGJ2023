using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoSingleton<PlantManager>
{
    public PlantData[] possiblePlants;
    private List<Plant> plants = new List<Plant>();

    public void AddPlant(Plant plant, Vector2 gridPosition)
    {
        plants.Add(plant);
        GridManager.instance.SetPlant(plant, gridPosition);
        plant.transform.position = GridManager.instance.GetPositionOnGrid(gridPosition);
        plant.growthState = GrowthState.GROWING;
        TurnManager.instance.ActionTaken();
    }

    public void RemovePlant(Plant plant)
    {
        plants.Remove(plant);
    }

    public void Step()
    {
        for(int x = plants.Count - 1; x >= 0; --x)
        {
            Plant p = plants[x];
            var result = p.TryGrow();
            switch (result)
            {
                case GrowthState.DEAD:
                {
                    p.Kill();
                    plants.Remove(p);
                }
                break;
                case GrowthState.COMPLETED:
                {
                    //score out
                    plants.Remove(p);
                }
                break;
            }
        }
    }
}
