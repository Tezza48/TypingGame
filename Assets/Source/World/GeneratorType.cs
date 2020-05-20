using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public enum GeneratorType
{
    Rectangle,
    DrunkenWalk,
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

public class DrunkenWalkGenerator : BaseGenerator
{
    const int MAX_WALK = 10000;
    const float MIN_CLEAR = 0.25f;
    const float MAX_CLEAR = 0.5f;

    const float MOB_DENSITY = 0.05f;
    const float MOB_DENSITY_DEVIANCE = 0.01f;

    public override LevelData Generate(LevelDefenition def)
    {
        var data = base.Generate(def);

        int walks = 0;
        int cleared = 0;
        int tilesToClear = (int)(Random.Range(MIN_CLEAR, MAX_CLEAR) * data.size.x * data.size.y);

        Vector2Int pos = data.size / 2;

        data.entrance = pos;

        // Fill with Walls;
        Parallel.For(0, data.size.y, (y) =>
        {
            for (int x = 0; x < data.size.x; x++)
            {
                data.tiles[x, y] = Tile.Wall;
            }
        });

        var clearTiles = new List<Vector2Int>();

        // Walk the grid
        while (walks < MAX_WALK && cleared < tilesToClear)
        {
            bool alreadyCleared = data.tiles[pos.x, pos.y] == Tile.Floor;

            data.tiles[pos.x, pos.y] = Tile.Floor;

            if (!alreadyCleared)
            {
                cleared++;
                clearTiles.Add(pos);
            }

            // Walk in random direction.
            bool horizontal = Random.value > 0.5f;
            var direction = new Vector2Int(horizontal ? 1 : 0, horizontal ? 0 : 1);
            if (Random.value > 0.5f)
            {
                direction *= -1;
            }

            var newPos = pos + direction;

            // Bounds check
            if (newPos.x > 0 && 
                newPos.x < data.size.x - 1 && 
                newPos.y > 0 && 
                newPos.y < data.size.y - 1)
            {
                pos = newPos;
            }

            walks++;
        }

        data.exit = pos;

        data.tiles[data.entrance.x, data.entrance.y] = Tile.Entrance;
        data.tiles[data.exit.x, data.exit.y] = Tile.Exit;
        
        clearTiles.Remove(data.exit);
        clearTiles.Remove(data.entrance);

        // TODO WT: Populate with mobs.
        var numMobs = (int)(cleared * (MOB_DENSITY + Random.Range(-MOB_DENSITY_DEVIANCE, MOB_DENSITY_DEVIANCE)));

        data.mobSpawns = new Vector2Int[numMobs];
        // build list of available places
        // shuffle
        ShuffleCollection(clearTiles);

        // use first N elements in array as mob spawns.
        for (int i = 0; i < numMobs; i++)
        {
            data.mobSpawns[i] = clearTiles[i];
        }

        Debug.Log(string.Format("Total num tiles: {0}, NumMobs: {1}, cleared: {2}",
            data.size.x * data.size.y, numMobs, cleared));

        return data;
    }

    private void ShuffleCollection<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}