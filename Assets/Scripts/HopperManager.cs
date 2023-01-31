using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperManager : MonoSingleton<HopperManager>
{
    public bool isDebug = false;
    public int numHoppers = 3;
    public GameObject plantPrefab;
    public GameObject plantUIPrefab;

    public Canvas UICanvas;

    public Transform[] spawnPoints;

    private List<(GameObject, GameObject)> spawnedObjects;

    protected override void OnCreation()
    {
        spawnedObjects = new List<(GameObject, GameObject)>();
        if (!isDebug)
        {
            Transform[] hoppers = new Transform[numHoppers];
            for (int x = numHoppers; x < spawnPoints.Length; ++x)
            {
                spawnPoints[x].gameObject.SetActive(false);
            }
        }
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
        if(isDebug)
        {
            var plantData = PlantManager.instance.GetAllPlantData();
            for (int x = 0; x < spawnPoints.Length; ++x)
            {
                Plant p = Instantiate(plantPrefab, spawnPoints[x].transform.position, Quaternion.identity).GetComponent<Plant>();
                p.SetPlantData(plantData[x]);
                PlantRequirementUI ui = Instantiate(plantUIPrefab, spawnPoints[x].transform.position, Quaternion.identity, UICanvas.transform).GetComponent<PlantRequirementUI>();
                ui.Setup(p);
                spawnedObjects.Add((p.gameObject, ui.gameObject));
            }
            return;
        }
        for (int x = 0; x < spawnPoints.Length && x < numHoppers; ++x)
        {
            Plant p = Instantiate(plantPrefab, spawnPoints[x].transform.position, Quaternion.identity).GetComponent<Plant>();
            p.SetPlantData(PlantManager.instance.GetRandomPlantData());
            PlantRequirementUI ui = Instantiate(plantUIPrefab, spawnPoints[x].transform.position, Quaternion.identity, UICanvas.transform).GetComponent<PlantRequirementUI>();
            ui.Setup(p);
            spawnedObjects.Add((p.gameObject, ui.gameObject));
        }
    }
}
