using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    UP = 0,
    RIGHT = 1,
    DOWN = 2,
    LEFT = 3,
}

public class GridManager : MonoSingleton<GridManager>
{
    public static Dictionary<Direction, Vector2> DIRECTION_TO_OFFSET = new Dictionary<Direction, Vector2>()
    {
        { Direction.UP,     new Vector2(0, 1) },
        { Direction.RIGHT,  new Vector2(1, 0) },
        { Direction.DOWN,   new Vector2(0, -1) },
        { Direction.LEFT,   new Vector2(-1, 0) },
    };
    int width = 15;
    int height = 10;

    public LevelData levelData;

    public GameObject tilePrefab;

    public Color redTileColor;
    public Color blueTileColor;
    public Color defaultTileColor;

    public Tile[,] tiles;

    protected override void OnCreation()
    {
        tiles = new Tile[width, height];
        Generate();
    }

    void Generate()
    {
        if(levelData == null)
        {
            GenerateRandom();
        }
        else
        {
            GenerateFromImage();
        }
        HopperManager.instance.Setup(levelData);
    }

    void GenerateRandom()
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
                    t.GetComponent<SpriteRenderer>().color = blueTileColor;
                }
                else if (rand == 9)
                {
                    t.isRed = true;
                    t.GetComponent<SpriteRenderer>().color = redTileColor;
                }
                else
                {
                    t.GetComponent<SpriteRenderer>().color = defaultTileColor;
                }
            }
        }
    }

    void GenerateFromImage()
    {
        Texture2D mapImage = levelData.mapImage;
        GameObject g = new GameObject("TileParent");
        width = mapImage.width;
        height = mapImage.height;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                GameObject temp = Instantiate(tilePrefab, GetPositionOnGrid(x, y), Quaternion.identity, g.transform);
                Tile t = temp.GetComponent<Tile>();
                t.gridPosition = new Vector2(x, y);
                tiles[x, y] = t;
                var pixel = mapImage.GetPixel(x, y);
                if (pixel.a == 1.0)
                {
                    if (pixel.r == 1.0)
                    {
                        t.isRed = true;
                        t.GetComponent<SpriteRenderer>().color = redTileColor;
                    }
                    else if (pixel.b == 1.0)
                    {
                        t.isBlue = true;
                        t.GetComponent<SpriteRenderer>().color = blueTileColor;
                    }
                    else
                    {
                        t.GetComponent<SpriteRenderer>().color = defaultTileColor;
                    }
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
        if(IsValid(x, y))
        {
            return tiles[x,y].isOccupied;
        }
        throw new System.Exception();
    }

    public bool IsOccupied(Vector2 v)
    {
        return IsOccupied((int)v.x, (int)v.y);
    }

    public void SetOccupied(int x, int y, bool occupied)
    {
        if (IsValid(x, y))
        {
            tiles[x, y].isOccupied = occupied;
            return;
        }
        throw new System.Exception();
    }

    public void SetOccupied(Vector2 v, bool occupied)
    {
        SetOccupied((int)v.x, (int)v.y, occupied);
    }

    public bool IsValid(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    public bool IsValid(Vector2 v)
    {
        return IsValid((int)v.x, (int)v.y);
    }

    public Tile GetTile(int x, int y)
    {
        if(IsValid(x, y))
        {
            return tiles[x, y];
        }
        throw new System.Exception();
    }

    public Tile GetTile(Vector2 v)
    {
        return GetTile((int)v.x, (int)v.y);
    }
}
