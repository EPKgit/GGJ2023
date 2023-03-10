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
public class Plant : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public GameObject dirtDrippingVFXPrefab;
    public GameObject dirtPlantingVFXPrefab;
    public GameObject harvestableVFXPrefab;
    public GameObject harvestingVFXPrefab;
    public GameObject deadPlantVFXPrefab;
    public Sprite deadPlantSprite;
    public GrowthState growthState = GrowthState.INVALID;
    public PlantData plantData;

    public int reqBlue;
    public int reqRed;
    private List<RootCanal> rootCanals = new List<RootCanal>();

    public Vector2 gridPosition = new Vector2(-1, -1);

    private class RootCanal
    {
        public bool stillGrowing = true;
        public List<Root> roots = new List<Root>();
        public PlantData.Root rootData;
    }

    public GameObject rootPrefab;

    public void SetPlantData(PlantData d)
    {
        plantData = d;
        GetComponent<SpriteRenderer>().sprite = d.sprite;
        growthState = GrowthState.UNPLANTED;
        rootCanals.Clear();
        for (int i = 0; i < plantData.roots.Length; i++)
        {
            rootCanals.Add(new RootCanal(){
                rootData = plantData.roots[i]
            });
        }
        reqBlue = plantData.requiredBlue;
        reqRed = plantData.requiredRed;
    }

    public bool CanGrow()
    {
        if(growthState != GrowthState.GROWING)
        {
            return false;
        }

        if (plantData.isInfinite)
        {
            return true;
        }

        foreach (RootCanal rootCanal in rootCanals)
        {
            if (rootCanal.roots.Count >= rootCanal.rootData.growthPattern.Length)
            {
                return false;
            }
        }

        return true;
    }


    public GrowthState TryGrow()
    {
        if(!CanGrow())
        {
            CheckCompletion();
        }

        bool grew = false;

        for (int i = 0; i < rootCanals.Count; i++)
        {
            grew |= GrowDir(rootCanals[i]);

            if (growthState == GrowthState.DEAD)
            {
                return growthState;
            }
        }
        
        if (grew == false)
        {
            CheckCompletion();
        }

        //Grow();
        return growthState;
    }

    private bool GrowDir(RootCanal rootCanal)
    {
        PlantData.Root rootData = rootCanal.rootData;

        if (rootCanal.stillGrowing == false)
        {
            return false;
        }


        List<Root> roots = rootCanal.roots;

        Direction growDirection = rootData.growthPattern[plantData.isInfinite ? 0 : roots.Count];
        Vector2 growFrom = (roots.Count > 0 ? roots[roots.Count - 1].GetComponent<Root>().gridPosition : gridPosition);
        Vector2 growTo = growFrom + GridManager.DIRECTION_TO_OFFSET[growDirection];

        var Grid = GridManager.instance;
        if(!Grid.IsValid(growTo) || Grid.IsOccupied(growTo))
        {
            rootCanal.stillGrowing = false;
            return false;
        }

        Tile resourceTile = GridManager.instance.GetTile(growTo);
        if (resourceTile.isBlue)
        {
            if (plantData.cantHaveBlue)
            {
                Kill();
                return false;
            }
            reqBlue--;
        }
        if (resourceTile.isRed)
        {
            if (plantData.cantHaveRed)
            {
                Kill();
                return false;
            }
            reqRed--;
        }

        // Create root at tile
        var root = Instantiate(rootPrefab, Grid.GetPositionOnGrid(growTo), Quaternion.identity);
        GridManager.instance.SetOccupied(growTo, true);

        var rootComp = root.GetComponent<Root>();
        rootComp.gridPosition = growTo;

        if(roots.Count > 0)
        {
            roots[roots.Count - 1].GetComponent<Root>().SetConnection(rootData.growthPattern[plantData.isInfinite ? 0 : roots.Count - 1], rootData.growthPattern[plantData.isInfinite ? 0 : roots.Count]);
        }

        Direction nextDirection = growDirection;
        if(rootData.growthPattern.Length > roots.Count + 1)
        {
            nextDirection = rootData.growthPattern[roots.Count + 1];
        }

        rootComp.SetEnding(growDirection, nextDirection);

        roots.Add(rootComp);

        if(roots.Count >= rootData.growthPattern.Length && !plantData.isInfinite)
        {
            rootCanal.stillGrowing = false;
        }

        return true;
    }
    
    /*
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

    */
    void CheckCompletion()
    {
        if(reqBlue <= 0 && reqRed <= 0)
        {
            growthState = GrowthState.COMPLETED;
            harvestableVFXPrefab = Instantiate(harvestableVFXPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Kill();
        }
    }

    public void Kill()
    {
        if(growthState == GrowthState.DEAD)
        {
            return;
        }

        ScoreManager.instance.Score += plantData.scoreDied;
        growthState = GrowthState.DEAD;
        CleanupRoots();
        GetComponent<SpriteRenderer>().sprite = deadPlantSprite;
        deadPlantVFXPrefab = Instantiate(deadPlantVFXPrefab, transform.position, Quaternion.identity);
    }

    void CleanupRoots()
    {
        foreach (RootCanal canal in rootCanals)
        {
            foreach (Root r in canal.roots)
            {
                GridManager.instance.SetOccupied(r.gridPosition, false);
                Destroy(r.gameObject);
            }
        }
    }


#region MOUSE_EVENTS

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (growthState == GrowthState.COMPLETED)
        {
            ScoreManager.instance.Score += plantData.scoreComplete;
            CleanupRoots();
            GridManager.instance.SetOccupied(gridPosition, false);
            harvestableVFXPrefab.GetComponent<VFXController>().StopParticlePlaying();
            Destroy(gameObject);
            TurnManager.instance.ActionTaken();
            Instantiate(harvestingVFXPrefab, transform.position, Quaternion.identity).GetComponent<VFXController>().StopParticlePlaying();
        }
    }
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
                    if(gridPosition.x >= 0 && gridPosition.y >= 0)//if we previously had a position and are moving (we are a rock) free up our old spot
                    {
                        GridManager.instance.SetOccupied(gridPosition, false);
                    }
                    PlantManager.instance.PlantSeed(this, t.gridPosition);
                    Tile resourceTile = GridManager.instance.GetTile(t.gridPosition);
                    if (resourceTile.isBlue)
                    {
                        if (plantData.cantHaveBlue)
                        {
                            Kill();
                        }
                        reqBlue--;
                    }
                    if (resourceTile.isRed)
                    {
                        if (plantData.cantHaveRed)
                        {
                            Kill();
                        }
                        reqRed--;
                    }
                    Instantiate(dirtPlantingVFXPrefab, transform.position, Quaternion.identity, transform);
                    TurnManager.instance.ActionTaken();
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
