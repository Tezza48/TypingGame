using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : MonoBehaviour
{
    public Vector2Int position
    {
        get => Vector2Int.RoundToInt(transform.position);
        set => transform.position = (Vector2)value;
    }

    public uint hitpoints;
    public bool invulnerable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
