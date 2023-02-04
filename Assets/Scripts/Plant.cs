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
    public List<Root> roots = new List<Root>();
    public Vector2 gridPosition = new Vector2(-1, -1);

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
            growthState = GrowthState.COMPLETED;
            harvestableVFXPrefab = Instantiate(harvestableVFXPrefab, transform.position, Quaternion.identity);
            AudioManager.instance.growSuccessAudio.Play();
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
        AudioManager.instance.dieAudio.Play();
    }

    void CleanupRoots()
    {
        foreach (Root r in roots)
        {
            GridManager.instance.SetOccupied(r.gridPosition, false);
            Destroy(r.gameObject);
        }
    }


#region MOUSE_EVENTS

    private void OnMouseEnter() {
        if(startPosition==Vector3.zero||this.transform.position == startPosition)
        {
            AudioManager.instance.hoverAudio.Play();
        }
    }

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
            AudioManager.instance.harvestAudio.Play();
        }
    }
    private Vector3 startPosition;

    public void OnBeginDrag(PointerEventData data)
    {
        if (growthState == GrowthState.UNPLANTED || growthState == GrowthState.INVALID) //we can only move unplanted plants
        {
            AudioManager.instance.pickupAudio.Play();
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
                        plantData.requiredBlue--;
                    }
                    if (resourceTile.isRed)
                    {
                        if (plantData.cantHaveRed)
                        {
                            Kill();
                        }
                        plantData.requiredRed--;
                    }
                    Instantiate(dirtPlantingVFXPrefab, transform.position, Quaternion.identity, transform);
                    TurnManager.instance.ActionTaken();
                    AudioManager.instance.dropAudio.Play();
                } else {
                    AudioManager.instance.cannotAudio.Play();
                }
                break;
            }
        }
        if (!found)
        {
            AudioManager.instance.cannotAudio.Play();
            this.transform.position = startPosition;
        }
    }
#endregion
}
