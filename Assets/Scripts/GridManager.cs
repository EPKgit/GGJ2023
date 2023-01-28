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
    public Tile[][] tiles;
    public Plant[][] plants;

    public void Generate()
    {

    }

    public bool IsOccupied(int x, int y)
    {
        if(x >= 0 && x < plants.Length && y >= 0 && y < plants[x].Length)
        {
            return plants[x][y] != null;
        }
        throw new System.Exception();
    }
}
