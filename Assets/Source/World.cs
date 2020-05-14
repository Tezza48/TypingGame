﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour, IKeyboardTarget
{
    public delegate void CombatStartEvent(GridEntity enemy);
    public event CombatStartEvent OnCombatStarted;

    public Vector2Int maxSize;
    public Vector2Int minSize;

    public TilePair[] tilePrefabsEditor;
    public GridEntity enemyPrefab;
    public LevelStyle style;
    new public Camera camera;

    // TODO WT: Out of this class.
    public GridEntity player;
    public List<GridEntity> entities;
    // -    -   -   -   -   -   -

    private Tile[,] grid;
    private Vector2Int size;
    private Dictionary<Tile, SpriteRenderer> tilePrefabs;
    private Vector2Int entrance;

    private List<GameObject> spawnedTiles;

    private Transform tilesContainer;

    // TODO WT: queue entities to be removed, destroy them in the next update.
    //private List<GridEntity> destroyQueue;

    // Start is called before the first frame update
    void Start()
    {
        tilePrefabs = tilePrefabsEditor.ToDictionary(element => element.tile, element => element.renderer);

        tilesContainer = new GameObject("Tiles").transform;
        tilesContainer.SetParent(transform);

        spawnedTiles = new List<GameObject>();

        NewLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TickEntities()
    {
        bool hasStartedCombat = false;

        foreach(var entity in entities)
        {
            if (entity == player) continue;

            entity.Tick(this);

            // TODO WT: Support multiple attackers.
            if (!hasStartedCombat && Vector2Int.Distance(player.position, entity.position) == 1.0f)
            {
                OnCombatStarted?.Invoke(entity);
                hasStartedCombat = true;
            }
        }
    }

    public void TryMove(GridEntity entity, Vector2Int delta)
    {
        var newPos = entity.position + delta;

        newPos.x = Mathf.Clamp(newPos.x, 0, size.x - 1);
        newPos.y = Mathf.Clamp(newPos.y, 0, size.y - 1);

        // Check against level geometry
        var tile = grid[newPos.x, newPos.y];
        switch (tile)
        {
            case Tile.Wall:
                // Cant walk on wall tiles.
                return;
            case Tile.Floor:
                // Free to move on this
                break;
            case Tile.Entrance:
                // Free to move on this
                break;
            case Tile.Exit:
                // Should have a little delay here so we can see the player stand on the exit.
                NewLevel();
                // Move to next room
                return;
            default:
                break;
        }

        // Check for entities too.

        foreach (var e in entities)
        {
            if (e != entity)
            {
                if (newPos == e.position)
                {
                    return;
                }
            }
        }

        entity.position = newPos;
    }

    public void MovePlayer(Vector2Int delta)
    {
        TryMove(player, delta);

        TickEntities();
    }

    private void NewLevel(bool clear = true)
    {
        ClearLevel();

        GenerateGrid();
        InstantiateGrid();
        SpawnEntities();

        camera.backgroundColor = style.background;
        camera.transform.position = new Vector3(size.x / 2, size.y / 2, -10);
        camera.orthographicSize = size.magnitude / 2;
        
        player.transform.position = new Vector2(entrance.x, entrance.y);
    }

    private void ClearLevel()
    {
        foreach(var tile in spawnedTiles)
        {
            Destroy(tile);
        }

        foreach(var entity in entities)
        {
            if (entity != player)
            {
                Destroy(entity.gameObject);
            }
        }

        entities.Clear();
        entities.Add(player);

        spawnedTiles.Clear();
    }

    private void GenerateGrid()
    {
        size.x = Random.Range(minSize.x, maxSize.x);
        size.y = Random.Range(minSize.y, maxSize.y);

        grid = new Tile[size.x, size.y];

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (y > 0 && y < size.y - 1 && x > 0 && x < size.x - 1)
                {
                    grid[x, y] = Tile.Floor;
                } else
                {
                    grid[x, y] = Tile.Wall;
                }
            }
        }

        entrance = new Vector2Int(size.x / 2, 0);
        grid[entrance.x, entrance.y] = Tile.Entrance;

        grid[size.x / 2, size.y - 1] = Tile.Exit;
    }

    private void InstantiateGrid()
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var tile = grid[x, y];

                var instance = Instantiate(tilePrefabs[tile], new Vector2(x, y), Quaternion.identity, tilesContainer);
                instance.color = style.Get(tile);

                spawnedTiles.Add(instance.gameObject);
            }
        }
    }

    private void SpawnEntities()
    {
        var enemy = Instantiate(enemyPrefab, (Vector2)(size / 2), Quaternion.identity, transform);
        entities.Add(enemy);
    }

    public void KeyPressed(char key)
    {
        // Handle Moving
        switch (key)
        {
            case 'w':
                MovePlayer(Vector2Int.up);
                break;
            case 'a':
                MovePlayer(Vector2Int.left);
                break;
            case 's':
                MovePlayer(Vector2Int.down);
                break;
            case 'd':
                MovePlayer(Vector2Int.right);
                break;
            case ' ':
                TickEntities();
                break;
            default:
                break;
        }
    }

    public void StringChanged(string value)
    {

    }

    public void StringSubmitted(string value)
    {

    }

    public void Backspace()
    {

    }
}

public enum Tile
{
    Wall,
    Floor,
    Entrance,
    Exit,
}

[System.Serializable]
public struct TilePair
{
    public Tile tile;
    public SpriteRenderer renderer;
}