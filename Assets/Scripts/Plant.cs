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
    public GameObject dirtPlantingVFXPrefab;
    public Sprite deadPlantSprite;
    public GrowthState growthState = GrowthState.INVALID;
    public PlantData plantData;
    public List<Root> roots = new List<Root>();
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

        if(roots.Count >= plantData.growthPattern.Length && !plantData.isInfinite)
        {
            return false;
        }

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
        Direction growDirection = plantData.growthPattern[plantData.isInfinite ? 0 : roots.Count];
        Vector2 growFrom = (roots.Count > 0 ? roots[roots.Count - 1].GetComponent<Root>().gridPosition : gridPosition);
        Vector2 growTo = growFrom + GridManager.DIRECTION_TO_OFFSET[growDirection];

        var Grid = GridManager.instance;
        if(!Grid.IsValid(growTo) || Grid.IsOccupied(growTo))
        {
            CheckCompletion();
            return;
        }

        Tile resourceTile = GridManager.instance.GetTile(growTo);
        if (resourceTile.isBlue)
        {
            if (plantData.cantHaveBlue)
            {
                Kill();
                return;
            }
            plantData.requiredBlue--;
        }
        if (resourceTile.isRed)
        {
            if (plantData.cantHaveRed)
            {
                Kill();
                return;
            }
            plantData.requiredRed--;
        }

        // Create root at tile
        var root = Instantiate(rootPrefab, Grid.GetPositionOnGrid(growTo), Quaternion.identity);
        GridManager.instance.SetOccupied(growTo, true);

        var rootComp = root.GetComponent<Root>();
        rootComp.gridPosition = growTo;

        if(roots.Count > 0)
        {
            roots[roots.Count - 1].GetComponent<Root>().SetConnection(plantData.growthPattern[plantData.isInfinite ? 0 : roots.Count - 1], plantData.growthPattern[plantData.isInfinite ? 0 : roots.Count]);
        }

        Direction nextDirection = growDirection;
        if(plantData.growthPattern.Length > roots.Count + 1)
        {
            nextDirection = plantData.growthPattern[roots.Count + 1];
        }

        rootComp.SetEnding(growDirection, nextDirection);

        roots.Add(rootComp);

        if(roots.Count >= plantData.growthPattern.Length && !plantData.isInfinite)
        {
            CheckCompletion();
        }
    }

    void CheckCompletion()
    {
        if(plantData.requiredBlue <= 0 && plantData.requiredRed <= 0)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
            ScoreManager.instance.Score += plantData.scoreComplete;
            growthState = GrowthState.COMPLETED;
        }
        else
        {
            Kill();
        }
    }

    public void Kill()
    {
        if(growthState != GrowthState.DEAD)
        {
            ScoreManager.instance.Score += plantData.scoreDied;
        }

        growthState = GrowthState.DEAD;
        foreach(Root r in roots)
        {
            GridManager.instance.SetOccupied(r.gridPosition, false);
            Destroy(r.gameObject);
        }
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<SpriteRenderer>().sprite = deadPlantSprite;
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
            child.GetComponent<VFXController>()?.StopParticlePlaying();
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
                    Tile resourceTile = GridManager.instance.GetTile(t.gridPosition);
                    if (resourceTile.isBlue)
                    {
                        if (plantData.cantHaveBlue)
                        {
                            Kill();
                            return;
                        }
                        plantData.requiredBlue--;
                    }
                    if (resourceTile.isRed)
                    {
                        if (plantData.cantHaveRed)
                        {
                            Kill();
                            return;
                        }
                        plantData.requiredRed--;
                    }
                    Instantiate(dirtPlantingVFXPrefab, transform.position, Quaternion.identity, transform);
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
