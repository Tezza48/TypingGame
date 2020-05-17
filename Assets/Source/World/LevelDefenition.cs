using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/LevelDefinition")]
public class LevelDefenition : ScriptableObject
{
    public GeneratorType type;
    public Vector2Int minSize;
    public Vector2Int maxSize;
    public LevelStyle[] styles;
}
