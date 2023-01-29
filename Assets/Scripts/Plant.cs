using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GrowthState
{
    INVALID,
    UNPLANTED,
    GROWING,
    DEAD,
    COMPLETED,
}
public class Plant : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GrowthState growthState = GrowthState.INVALID;
    public bool isRock;
    public PlantData plantData;
    public List<GameObject> roots = new List<GameObject>();

    private void SetPlantData(PlantData d)
    {
        plantData = d;
        GetComponent<SpriteRenderer>().sprite = d.sprite;
        growthState = GrowthState.UNPLANTED;
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
        Destroy(g);
    }


#region DRAGGING
    public static Plant currentDraggingPlant = null;
    private Vector3 startPosition;

    public void OnBeginDrag(PointerEventData data)
    {
        if (growthState == GrowthState.UNPLANTED || growthState == GrowthState.INVALID) //we can only move unplanted plants
        {
            startPosition = this.transform.position;
            return;
        }
        data.pointerDrag = null;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = new Vector3(pos.x, pos.y, 0);
    }

    public void OnEndDrag(PointerEventData data)
    {
        Collider2D[] hits;
        hits = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), ~LayerMask.NameToLayer("Tiles"));
        bool found = false;
        foreach(Collider2D hit in hits)
        {
            Tile t = hit.transform.GetComponent<Tile>();
            if(t != null)
            {
                if (!GridManager.instance.IsOccupied(t.gridPosition))
                {
                    found = true;
                    PlantManager.instance.AddPlant(this, t.gridPosition);
                }
                break;
            }
        }
        if (!found)
        {
            this.transform.position = startPosition;
        }
    }
#endregion
}
