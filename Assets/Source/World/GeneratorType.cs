using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public enum GeneratorType
{
    Rectangle,
    DrunkenWalk,
    Rooms,
}

public class BaseGenerator
{
    protected const float MOB_DENSITY = 0.05f;
    protected const float MOB_DENSITY_DEVIANCE = 0.01f;

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

    protected Vector2Int[] GenerateSpawns(List<Vector2Int> clearTiles)
    {
        float populationPercent = MOB_DENSITY + Random.Range(-MOB_DENSITY_DEVIANCE, MOB_DENSITY_DEVIANCE);
        int mobCount = (int)(populationPercent * clearTiles.Count);

        ShuffleCollection(clearTiles);

        return clearTiles.GetRange(0, mobCount).ToArray();
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

public class RectangleGenerator: BaseGenerator
{
    public override LevelData Generate(LevelDefenition def)
    {
        LevelData data = base.Generate(def);

        var clearTiles = new List<Vector2Int>();
        
        for (int y = 0; y < data.size.y; y++)
        {
            for (int x = 0; x < data.size.x; x++)
            {
                if (y > 0 && y < data.size.y - 1 && x > 0 && x < data.size.x - 1)
                {
                    data.tiles[x, y] = Tile.Floor;
                    clearTiles.Add(new Vector2Int(x, y));
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

        clearTiles.Remove(data.entrance);
        clearTiles.Remove(data.exit);

        data.mobSpawns = GenerateSpawns(clearTiles);

        return data;
    }
}

public class DrunkenWalkGenerator : BaseGenerator
{
    const int MAX_WALK = 10000;
    const float MIN_CLEAR = 0.25f;
    const float MAX_CLEAR = 0.5f;

    public override LevelData Generate(LevelDefenition def)
    {
        var data = base.Generate(def);

        int walks = 0;
        int cleared = 0;
        int tilesToClear = (int)(Random.Range(MIN_CLEAR, MAX_CLEAR) * data.size.x * data.size.y);

        Vector2Int pos = data.size / 2;

        data.entrance = pos;

        // Fill with Walls;
        for (int y = 0; y < data.size.y; y++)
        {
            Parallel.For(0, data.size.x, (x) =>
            {
                data.tiles[x, y] = Tile.Wall;
            });
        }

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
        
        data.mobSpawns = GenerateSpawns(clearTiles);

        return data;
    }
}