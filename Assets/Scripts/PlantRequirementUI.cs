using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantRequirementUI : MonoBehaviour
{
    public GameObject dotPrefab;
    
    public void Setup(Plant plant)
    {
        int totalReqs = plant.plantData.requiredBlue + plant.plantData.requiredRed;
        for (int i = 0; i < totalReqs; ++i)
        {
            GameObject g = Instantiate(dotPrefab, Vector3.zero, Quaternion.identity, transform);
            g.GetComponent<Image>().color = i >= plant.plantData.requiredBlue ? GridManager.instance.redTileColor : GridManager.instance.blueTileColor;
        }

        transform.position = plant.transform.position + Vector3.left * 0.7f;
    }
}
