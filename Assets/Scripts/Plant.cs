using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrowthState
{
    DEAD,
    GROWING,
    COMPLETED,
}
public class Plant : MonoBehaviour
{
    public GrowthState growthState;
    public bool isRock;
    public PlantData plantData;
    public List<GameObject> roots = new List<GameObject>();

    private void Awake()
    {
        growthState = GrowthState.GROWING;
    }

    public bool CanGrow()
    {
        if(growthState != GrowthState.GROWING)
        {
            return false;
        }
        // do some adjacency check for the roots
        return true;
    }


    public GrowthState TryGrow()
    {
        if(!CanGrow())
        {
            return GrowthState.DEAD;
        }
        Grow();
        return growthState;
    }
    private void Grow()
    {
        //spawn a root based on our current plant data growth list
        //check if we're blocked
        //check if we've picked up a requirement and mark that in the growth data
        //check if we're out of growth states
        growthState = GrowthState.COMPLETED;
    }

    public void Kill()
    {
        growthState = GrowthState.DEAD;
        foreach(GameObject g in roots)
        {
            Destroy(g);
        }
    }
}
