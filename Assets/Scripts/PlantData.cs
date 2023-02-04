using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlantData")]
public class PlantData : ScriptableObject
{
    public Sprite sprite;
    public Sprite growthPattenSprite;
    public int growthNumber;
    public Root[] roots;
    public int requiredRed;
    public int requiredBlue;
    public bool cantHaveRed;
    public bool cantHaveBlue;
    public bool isInfinite;
    public bool isRock;
    public int scoreComplete;
    public int scoreDied;

    [System.Serializable]
    public class Root 
    {
        public Direction[] growthPattern;
    }
}
