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

    private (GameObject, GameObject)[] spawnedObjects;
    private PlantData[] levelHopperQueue = null;
    private int levelHopperIndex;


    public void Setup(LevelData data)
    {
        if (data != null)
        {
            numHoppers = data.hopperNum;
            levelHopperQueue = data.hopperOrder;
            levelHopperIndex = 0;
        }

        if (isDebug)
        {
            numHoppers = PlantManager.instance.GetAllPlantData().Count;
        }
        spawnedObjects = new (GameObject, GameObject)[numHoppers];

        Transform[] hoppers = new Transform[numHoppers];
        for (int x = 0; x < spawnPoints.Length; ++x)
        {
            spawnPoints[x].gameObject.SetActive(x < numHoppers);
        }
        Step();
    }

    public void TakePlant(Plant p)
    {
        for (int x = spawnedObjects.Length - 1; x >= 0; --x)
        {
            if(p.gameObject == spawnedObjects[x].Item1)
            {
                Destroy(spawnedObjects[x].Item2);
                spawnedObjects[x].Item1 = spawnedObjects[x].Item2 = null;
                return;
            }
        }
    }
    public void Step()
    {
        
        if (isDebug)
        {
            CleanupOldPlants();
            DoDebugSpawning();
        }
        else if (levelHopperQueue == null || levelHopperQueue.Length == 0)
        {
            CleanupOldPlants();
            DoRandomSpawning();
        }
        else
        {
            SpawnFromQueue();
        }
    }

    void CleanupOldPlants()
    {
        for (int x = spawnedObjects.Length - 1; x >= 0; --x)
        {
            Destroy(spawnedObjects[x].Item1);
            Destroy(spawnedObjects[x].Item2);
        }
        spawnedObjects = new (GameObject, GameObject)[numHoppers];
    }
    void DoDebugSpawning()
    {
        var plantData = PlantManager.instance.GetAllPlantData();
        for (int x = 0; x < spawnPoints.Length; ++x)
        {
            Plant p = Instantiate(plantPrefab, spawnPoints[x].transform.position, Quaternion.identity).GetComponent<Plant>();
            p.SetPlantData(plantData[x]);
            PlantRequirementUI ui = Instantiate(plantUIPrefab, spawnPoints[x].transform.position, Quaternion.identity, UICanvas.transform).GetComponent<PlantRequirementUI>();
            ui.Setup(p);
            spawnedObjects[x] = (p.gameObject, ui.gameObject);
        }
    }

    void DoRandomSpawning()
    {
        for (int x = 0; x < spawnPoints.Length && x < numHoppers; ++x)
        {
            Plant p = Instantiate(plantPrefab, spawnPoints[x].transform.position, Quaternion.identity).GetComponent<Plant>();
            p.SetPlantData(PlantManager.instance.GetRandomPlantData());
            PlantRequirementUI ui = Instantiate(plantUIPrefab, spawnPoints[x].transform.position, Quaternion.identity, UICanvas.transform).GetComponent<PlantRequirementUI>();
            ui.Setup(p);
            spawnedObjects[x] = (p.gameObject, ui.gameObject);
        }
    }

    void SpawnFromQueue()
    {
        bool empty = true;
        for (int x = 0; x < spawnedObjects.Length; ++x)
        {
            if(spawnedObjects[x].Item1 == null)
            {
                if (levelHopperIndex < levelHopperQueue.Length)
                {
                    Plant p = Instantiate(plantPrefab, spawnPoints[x].transform.position, Quaternion.identity).GetComponent<Plant>();
                    p.SetPlantData(levelHopperQueue[levelHopperIndex++]);
                    PlantRequirementUI ui = Instantiate(plantUIPrefab, spawnPoints[x].transform.position, Quaternion.identity, UICanvas.transform).GetComponent<PlantRequirementUI>();
                    ui.Setup(p);
                    spawnedObjects[x] = (p.gameObject, ui.gameObject);
                    empty = false;
                }
            }
            else
            {
                empty = false;
            }
        }
        if(empty)
        {
            Debug.Log("LEVEL OVER, NOTHING TO SPAWN, NOTHING LEFT");
        }
    }
}
