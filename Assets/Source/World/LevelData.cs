using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LevelData
{
    public Tile[,] tiles;
    public Vector2Int size;
    public Vector2Int[] mobSpawns;
    public Vector2Int entrance;
    public Vector2Int exit;
    public LevelStyle style;
}
