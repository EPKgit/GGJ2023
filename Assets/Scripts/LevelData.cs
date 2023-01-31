using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public Texture2D mapImage;
    public PlantData[] hopperOrder;
    public int hopperNum;
}
