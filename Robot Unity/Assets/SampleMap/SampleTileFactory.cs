using UnityEngine;
public class SampleTileFactory : TileFactory
{
    public GameObject FloorTile;
    public GameObject RobotTile;
    public GameObject ExitTile;
    public GameObject WallTile;

    public override GameObject GetTile(char ch)
    {
        if (ch == '.')
        {
            return UnityEngine.Object.Instantiate(this.FloorTile);
        }

        if (ch == '>')
        {
            return UnityEngine.Object.Instantiate(this.RobotTile);
        }

        if (ch == 'X')
        {
            return UnityEngine.Object.Instantiate(this.ExitTile);
        }

        if (ch == '#')
        {
            return UnityEngine.Object.Instantiate(this.WallTile);
        }

        throw new InvalidTileCharacterException($"Could not create Tile with character '{ch}'.");
    }

    public override bool IsValidTile(char ch)
    {
        return ".>X#".Contains($"{ch}");
    }
}