using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlantData")]
public class PlantData : ScriptableObject
{
    public Sprite sprite;
    public Direction[] growthPattern;
    public int requiredRed;
    public int requiredBlue;
}
