using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperManager : MonoSingleton<HopperManager>
{
    public GameObject plantPrefab;

    public Transform[] spawnPoints;

    private List<GameObject> spawnedObjects;

    protected override void OnCreation()
    {
        spawnedObjects = new List<GameObject>();
        Step();
    }

    public void TakePlant(Plant p)
    {
        for (int x = spawnedObjects.Count - 1; x >= 0; --x)
        {
            if(p.gameObject == spawnedObjects[x])
            {
                spawnedObjects.Remove(p.gameObject);
                return;
            }
        }
    }
    public void Step()
    {
        for (int x = spawnedObjects.Count - 1; x >= 0; --x)
        {
            Destroy(spawnedObjects[x]);
        }
        spawnedObjects.Clear();
        foreach (Transform t in spawnPoints)
        {
            Plant p = Instantiate(plantPrefab, t.transform.position, Quaternion.identity).GetComponent<Plant>();
            p.SetPlantData(PlantManager.instance.GetRandomPlantData());
            spawnedObjects.Add(p.gameObject);
        }
    }
}
