using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/LevelStyle")]
public class LevelStyle : ScriptableObject
{
    public Color wall;
    public Color floor;
    public Color entrance;
    public Color exit;
    public Color background;

    public Color Get(Tile tile)
    {
        switch (tile)
        {
            case Tile.Wall:
                return wall;
            case Tile.Floor:
                return floor;
            case Tile.Entrance:
                return entrance;
            case Tile.Exit:
                return exit;
            default:
                // Return obvious color for eronious color;
                return Color.magenta;
        }
    }
}
