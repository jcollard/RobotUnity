using UnityEngine;
using System.Collections.Generic;
public class SampleTileFactory : TileFactory
{
    public GameObject FloorTile;
    public GameObject RobotTile;
    public GameObject ExitTile;
    public GameObject WallTile;

    public override List<GameObject> GetTile(char ch)
    {
        List<GameObject> objects = new List<GameObject>();
        objects.Add(UnityEngine.Object.Instantiate(this.FloorTile));

        if (ch == '>')
        {
            objects.Add(UnityEngine.Object.Instantiate(this.RobotTile));
        }

        if (ch == 'v')
        {
            GameObject clone = UnityEngine.Object.Instantiate(this.RobotTile);
            clone.transform.Rotate(0, 90, 0);
            objects.Add(clone);
        }

        if (ch == 'X')
        {
            objects.Add(UnityEngine.Object.Instantiate(this.ExitTile));
        }

        if (ch == '#')
        {
            objects.Add(UnityEngine.Object.Instantiate(this.WallTile));
        }

        return objects;
    }

    public override bool IsValidTile(char ch)
    {
        return ".>vX#".Contains($"{ch}");
    }
}