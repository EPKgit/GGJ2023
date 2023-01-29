using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}

public class GridManager : MonoSingleton<GridManager>
{
    const int width = 20;
    const int height = 10;

    public GameObject tilePrefab;
    public GameObject plantPrefab;

    public Tile[,] tiles;
    public Plant[,] plants;

    protected override void OnCreation()
    {
        plants = new Plant[width, height];
        tiles = new Tile[width, height];
        Generate();
    }

    public void Generate()
    {
        GameObject g = new GameObject("TileParent");
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                GameObject temp = Instantiate(tilePrefab, GetPositionOnGrid(x, y), Quaternion.identity, g.transform);
                Tile t = temp.GetComponent<Tile>();
                t.gridPosition = new Vector2(x, y);
                tiles[x, y] = t;
                var rand = Random.Range(0, 10);
                if (rand == 8)
                {
                    t.isBlue = true;
                    t.GetComponent<SpriteRenderer>().color = Color.blue;
                }
                if (rand == 9)
                {
                    t.isRed = true;
                    t.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }

    public Vector3 GetPositionOnGrid(int x, int y)
    {
        return new Vector3(x, y, 0);
    }

    public Vector3 GetPositionOnGrid(Vector2 v)
    {
        return GetPositionOnGrid((int)v.x, (int)v.y);
    }

    public bool IsOccupied(int x, int y)
    {
        if(x >= 0 && x < width && y >= 0 && y < height)
        {
            return plants[x,y] != null;
        }
        throw new System.Exception();
    }

    public bool IsOccupied(Vector2 v)
    {
        return IsOccupied((int)v.x, (int)v.y);
    }

    public void SetPlant(Plant p, int x, int y)
    {
        plants[x, y] = p;
    }

    public void SetPlant(Plant p, Vector2 v)
    {
        SetPlant(p, (int)v.x, (int)v.y);
    }
}
