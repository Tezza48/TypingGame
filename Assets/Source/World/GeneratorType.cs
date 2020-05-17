using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeneratorType
{
    Rectangle,
    //DrunkenWalk,
    // Rooms,
}

public class BaseGenerator
{
    public virtual LevelData Generate(LevelDefenition def)
    {
        var sizeX = Random.Range(def.minSize.x, def.maxSize.x);
        var sizeY = Random.Range(def.minSize.y, def.maxSize.y);

       return new LevelData
        {
            size = new Vector2Int(sizeX, sizeY),
            style = def.styles[Random.Range(0, def.styles.Length)],
            tiles = new Tile[sizeX, sizeY],
        };
    }
}

public class RectangleGenerator: BaseGenerator
{
    public override LevelData Generate(LevelDefenition def)
    {
        LevelData data = base.Generate(def);

        Debug.Log(data.style.name);
        
        for (int y = 0; y < data.size.y; y++)
        {
            for (int x = 0; x < data.size.x; x++)
            {
                if (y > 0 && y < data.size.y - 1 && x > 0 && x < data.size.x - 1)
                {
                    data.tiles[x, y] = Tile.Floor;
                }
                else
                {
                    data.tiles[x, y] = Tile.Wall;
                }
            }
        }

        data.entrance = new Vector2Int(data.size.x / 2, 0);
        data.tiles[data.entrance.x, data.entrance.y] = Tile.Entrance;

        data.tiles[data.size.x / 2, data.size.y - 1] = Tile.Exit;

        data.mobSpawns = new Vector2Int[] { data.size / 2 };
        //data.mobSpawns = new Vector2Int[] { };

        return data;
    }
}