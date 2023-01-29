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
    public GameObject dirtDrippingVFXPrefab;
    public GrowthState growthState = GrowthState.INVALID;
    public bool isRock;
    public PlantData plantData;
    public List<GameObject> roots = new List<GameObject>();
    public Vector2 gridPosition;

    public GameObject rootPrefab;

    public void SetPlantData(PlantData d)
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

        if(roots.Count >= plantData.growthPattern.Length)
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

        Direction growDirection = plantData.growthPattern[roots.Count];
        Vector2 growFrom = (roots.Count > 0 ? roots[roots.Count - 1].GetComponent<Root>().gridPosition : gridPosition);
        Vector2 growTo = growFrom + GridManager.DIRECTION_TO_OFFSET[growDirection];

        var Grid = GridManager.instance;
        if(!Grid.IsValid(growTo) || Grid.IsOccupied(growTo))
        {
            Debug.Log("Can't grow, ded");
            Kill();
            return;
        }

        // Create root at tile
        var root = Instantiate(rootPrefab, Grid.GetPositionOnGrid(growTo), Quaternion.Euler(0,0,0));
        var rootComp = root.GetComponent<Root>();
        rootComp.gridPosition = growTo;

        if(roots.Count > 0)
        {
            roots[roots.Count - 1].GetComponent<Root>().SetConnection(plantData.growthPattern[roots.Count - 1], plantData.growthPattern[roots.Count]);
        }

        Direction nextDirection = growDirection;
        if(plantData.growthPattern.Length > roots.Count + 1)
        {
            nextDirection = plantData.growthPattern[roots.Count + 1];
        }

        rootComp.SetEnding(growDirection, nextDirection);

        roots.Add(root);

        if(roots.Count >= plantData.growthPattern.Length)
        {
            Debug.Log("Done growing");
            growthState = GrowthState.COMPLETED;
        }
    }

    public void Kill()
    {
        growthState = GrowthState.DEAD;
        foreach(GameObject g in roots)
        {
            Destroy(g);
        }
    }


#region DRAGGING
    public static Plant currentDraggingPlant = null;
    private Vector3 startPosition;

    public void OnBeginDrag(PointerEventData data)
    {
        if (growthState == GrowthState.UNPLANTED || growthState == GrowthState.INVALID) //we can only move unplanted plants
        {
            startPosition = this.transform.position;
            Instantiate(dirtDrippingVFXPrefab, transform.position, Quaternion.identity, transform);
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
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ParticleSystem>())
            {
                Destroy(child.gameObject);
                break;
            }
        }
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
                    PlantManager.instance.PlantSeed(this, t.gridPosition);
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
