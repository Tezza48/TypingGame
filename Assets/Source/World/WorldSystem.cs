using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldSystem : MonoBehaviour, IKeyboardTarget
{
    static readonly RectangleGenerator RECTANGLE_GENERATOR = new RectangleGenerator();
    static readonly DrunkenWalkGenerator WALK_GENERATOR = new DrunkenWalkGenerator();

    public delegate void CombatStartEvent(GridEntity enemy);
    public event CombatStartEvent OnCombatStarted;

    public LevelDefenition[] levelDefs;

    public TilePair[] tilePrefabsEditor;
    public GridEntity enemyPrefab;
    new public Camera camera;
    public float maxCameraSize = 10.0f;

    // TODO WT: Out of this class.
    public GridEntity player;
    public List<GridEntity> entities;
    // -    -   -   -   -   -   -


    private LevelData level;

    private Dictionary<Tile, SpriteRenderer> tilePrefabs;

    private List<GameObject> spawnedTiles;

    private Transform tilesContainer;

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
        // Keep level in viewport if possible
        // otherwise scroll view to target player with camera clamped to level bounds.

        var targetSize = Mathf.Max(level.size.x / camera.aspect, level.size.y) / 2.0f;
        var targetPos = (Vector2)player.transform.position;

        camera.orthographicSize = Mathf.Min(targetSize, maxCameraSize);

        var camHalfHeight = camera.orthographicSize;
        var camHalfWidth = camera.aspect * camera.orthographicSize;

        Vector2 camPos;

        // TODO WT: Handle this per axis to stop the camera jumping left and right on thin levels.
        if (targetSize > maxCameraSize)
        {
            camPos = (Vector2)player.transform.position + new Vector2(0.5f, 0.5f);

            camPos.x = Mathf.Clamp(camPos.x, camHalfWidth - 0.5f, level.size.x - camHalfWidth - 0.5f);
            camPos.y = Mathf.Clamp(camPos.y, camHalfHeight - 0.5f, level.size.y - camHalfHeight - 0.5f);
        }
        else
        {
            camPos = ((Vector2)level.size / 2.0f) - new Vector2(0.5f, 0.5f);
        }

        camera.transform.position = new Vector3(camPos.x, camPos.y, -10);
    }

    public void TickEntities()
    {
        bool hasStartedCombat = false;

        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
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

        newPos.x = Mathf.Clamp(newPos.x, 0, level.size.x - 1);
        newPos.y = Mathf.Clamp(newPos.y, 0, level.size.y - 1);

        // Check against level geometry
        var tile = level.tiles[newPos.x, newPos.y];
        if (tile == Tile.Wall)
        {
            return;
        }

        // Check for entities too.

        for (int i = 0; i < entities.Count; i++)
        {
            var e = entities[i];
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
        
        if (level.tiles[player.position.x, player.position.y] == Tile.Exit)
        {
            NewLevel();
            return;
        }

        TickEntities();
    }

    private void NewLevel()
    {
        ClearLevel();

        GenerateGrid();
        InstantiateGrid();
        SpawnEntities();

        camera.backgroundColor = level.style.background;
        camera.transform.position = new Vector3(level.size.x / 2, level.size.y / 2, -10);
        camera.orthographicSize = (level.size.y + 0.5f) / 2;
        
        player.transform.position = new Vector2(level.entrance.x, level.entrance.y);
    }

    private void ClearLevel()
    {
        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            Destroy(spawnedTiles[i]);
        }

        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i] != player)
            {
                Destroy(entities[i].gameObject);
            }
        }

        entities.Clear();
        entities.Add(player);

        spawnedTiles.Clear();
    }

    private void GenerateGrid()
    {
        BaseGenerator generator;

        var def = levelDefs[Random.Range(0, levelDefs.Length)];

        switch (def.type)
        {
            case GeneratorType.Rectangle:
                generator = RECTANGLE_GENERATOR;
                break;
            case GeneratorType.DrunkenWalk:
                generator = WALK_GENERATOR;
                break;
            default:
                generator = RECTANGLE_GENERATOR; // Default to Rectangle.
                break;
        }

        level = generator.Generate(def);
    }

    private void InstantiateGrid()
    {
        for (int y = 0; y < level.size.y; y++)
        {
            for (int x = 0; x < level.size.x; x++)
            {
                var tile = level.tiles[x, y];

                var instance = Instantiate(tilePrefabs[tile], new Vector2(x, y), Quaternion.identity, tilesContainer);
                instance.color = level.style.Get(tile);

                spawnedTiles.Add(instance.gameObject);
            }
        }
    }

    private void SpawnEntities()
    {
        foreach (var spawn in level.mobSpawns)
        {
            var instance = Instantiate(enemyPrefab, (Vector2)spawn, Quaternion.identity, transform);
            entities.Add(instance);
        }
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

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            NewLevel();
        }
    }
#endif
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