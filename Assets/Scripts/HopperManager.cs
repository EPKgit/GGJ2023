using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperManager : MonoSingleton<HopperManager>
{
    public GameObject plantPrefab;
    public GameObject plantUIPrefab;

    public Canvas UICanvas;

    public Transform[] spawnPoints;

    private List<(GameObject, GameObject)> spawnedObjects;

    protected override void OnCreation()
    {
        spawnedObjects = new List<(GameObject, GameObject)>();
        Step();
    }

    public void TakePlant(Plant p)
    {
        for (int x = spawnedObjects.Count - 1; x >= 0; --x)
        {
            if(p.gameObject == spawnedObjects[x].Item1)
            {
                Destroy(spawnedObjects[x].Item2);
                spawnedObjects.RemoveAt(x);
                return;
            }
        }
    }
    public void Step()
    {
        for (int x = spawnedObjects.Count - 1; x >= 0; --x)
        {
            Destroy(spawnedObjects[x].Item1);
            Destroy(spawnedObjects[x].Item2);
        }
        spawnedObjects.Clear();
        foreach (Transform t in spawnPoints)
        {
            Plant p = Instantiate(plantPrefab, t.transform.position, Quaternion.identity).GetComponent<Plant>();
            p.SetPlantData(PlantManager.instance.GetRandomPlantData());
            PlantRequirementUI ui = Instantiate(plantUIPrefab, t.transform.position, Quaternion.identity, UICanvas.transform).GetComponent<PlantRequirementUI>();
            ui.Setup(p);
            spawnedObjects.Add((p.gameObject, ui.gameObject));
        }
    }
}
