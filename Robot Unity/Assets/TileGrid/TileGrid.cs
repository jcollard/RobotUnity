using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using Collard;

public class TileGrid : MonoBehaviour
{

    private TextAsset mapFile;
    private TileFactory factory;
    private bool isDirty = true;
    private GameObject[,] grid;

    private int rows = 1;
    private int columns = 1;

    public TextAsset MapFile
    {
        get
        {
            if (this.mapFile == null)
            {
                this.mapFile = new TextAsset("1x1\n#");
            }
            return this.mapFile;
        }
        set => this.mapFile = value;
    }

    public TileFactory Factory
    {
        get => this.factory;
        set => this.factory = value ?? throw new ArgumentNullException("Factory must be non-null.");
    }

    public void CleanUp()
    {
        if (!this.isDirty)
        {
            return;
        }

        this.GenerateGrid();
        this.isDirty = false;
    }

    public void GenerateGrid()
    {
        UnityUtils.DeleteChildren(this.transform);
        List<String> lines = Utils.GetStringIterable(mapFile.text).ToList();
        char[,] grid = MapFileParser.Instance.Parse(lines);
        this.rows = grid.GetLength(0);
        this.columns = grid.GetLength(1);
        this.grid = new GameObject[rows, columns];

        float offsetX = -((float)(this.rows - 1)) * 0.5f;
        float offsetZ = -((float)(this.columns - 1)) * 0.5f;

        foreach ((int row, int col, char c) in Utils.Get2DEnumerable(grid))
        {
            GameObject newTile;
            if (this.factory == null)
            {
                newTile = TileGrid.GetDefaultTile();
            }
            else if (!this.factory.IsValidTile(c))
            {
                newTile = this.factory.GetTile(c);
            }
            else
            {
                throw new InvalidTileCharacterException($"An invalid tile character {c} was found at {row}x{col}.");
            }
            newTile.transform.parent = this.transform;
            this.transform.localPosition = new Vector3(row + offsetX, 0, col + offsetZ);
        }
    }

    private static GameObject GetDefaultTile()
    {
        GameObject defaultObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        defaultObject.name = "Default Tile";
        defaultObject.transform.localScale = new Vector3(.98f, 0.1f, .98f);
        return defaultObject;
    }
}

[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TileGrid tileGrid = (TileGrid)target;

        tileGrid.MapFile = (TextAsset)EditorGUILayout.ObjectField("Map File", tileGrid.MapFile, typeof(TextAsset), false);
        EditorGUILayout.ObjectField("Tile Factory", tileGrid.Factory, typeof(MapFileParser), true);


        GUILayout.Space(20);
        if (GUILayout.Button("Rebuild Tile Grid"))
        {
            //tileGrid.CleanUp();
            tileGrid.GenerateGrid();
        }


        //EditorGUILayout.HelpBox("The rows and columns must be >0.", MessageType.Info);

        //tileGrid.CleanUp();

    }
}

public class TileFactory : MonoBehaviour
{
    public virtual GameObject GetTile(char ch)
    {
        throw new NotImplementedException("Method not Implemented");
    }

    public bool IsValidTile(char ch)
    {
        throw new NotImplementedException("Method not Implemented");
    }
}