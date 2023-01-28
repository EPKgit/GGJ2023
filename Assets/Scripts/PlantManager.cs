using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoSingleton<PlantManager>
{
    private List<Plant> plants = new List<Plant>();

    public void AddPlant(Plant plant)
    {
        plants.Add(plant);
    }

    public void RemovePlant(Plant plant)
    {
        plants.Remove(plant);
    }

    public void Step()
    {
        for(int x = plants.Count; x >= 0; --x)
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
