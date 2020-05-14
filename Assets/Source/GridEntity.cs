using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
    public delegate void HitpointsEvent(uint hitpoints);
    public event HitpointsEvent OnHitpointsChanged;

    public Vector2Int position
    {
        get => Vector2Int.RoundToInt(transform.position);
        set => transform.position = (Vector2)value;
    }

    public uint Hitpoints { get => hitpoints;
        set {
            hitpoints = value;
            OnHitpointsChanged?.Invoke(hitpoints);
        }
    }

    [SerializeField]
    private uint hitpoints;
    public bool invulnerable;

    // Start is called before the first frame update
    void Start()
    {
        OnHitpointsChanged?.Invoke(hitpoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Tick(World grid)
    {
        // Enemy Move
        grid.TryMove(this, new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2)));
    }
}
