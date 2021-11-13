using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Collard;

public class TileGrid : MonoBehaviour
{

    private TextAsset mapFile;
    private TileFactory factory;
    private bool _IsDirty = true;
    private bool IsDirty
    {
        get => ErrorMessage != null || _IsDirty;
        set => _IsDirty = value;
    }
    private GameObject[,] grid;
    public bool IsLoaded
    {
        get => this.IsDirty == false && this.grid != null;
    }
    public string ErrorMessage { get; private set; }

    public int Rows { get => this.grid != null ? this.grid.GetLength(0) : -1; }
    public int Columns { get => this.grid != null ? this.grid.GetLength(1) : -1; }

    public TextAsset MapFile
    {
        get
        {
            if (this.mapFile == null)
            {
                this.mapFile = new TextAsset("1x1\n.");
            }
            return this.mapFile;
        }

        set
        {
            if (this.mapFile == value)
            {
                return;
            }

            this.mapFile = value ?? throw new ArgumentNullException("Cannot specify a null Map File.");
            this.IsDirty = true;
        }
    }

    public TileFactory Factory
    {
        get => this.factory;
        set
        {
            if (this.factory == value)
            {
                return;
            }

            this.factory = value ?? throw new ArgumentNullException("Factory must be non-null.");
            this.IsDirty = true;
        }
    }

    public void CleanUp()
    {
        if (!this.IsDirty)
        {
            return;
        }

        this.IsDirty = false;
        this.GenerateGrid();
    }

    public void GenerateGrid()
    {
        UnityUtils.DestroyImmediateChildren(this.transform);
        List<String> lines = Utils.GetStringIterable(mapFile.text).ToList();
        this.grid = null;
        try
        {
            char[,] grid = MapFileParser.Instance.Parse(lines);
            this.grid = new GameObject[grid.GetLength(0), grid.GetLength(1)];
            float offsetX = -((float)(this.Rows - 1)) * 0.5f;
            float offsetZ = -((float)(this.Columns - 1)) * 0.5f;

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
                newTile.transform.localPosition = new Vector3(row + offsetX, 0, col + offsetZ);
                this.grid[row, col] = newTile;
            }

            this.ErrorMessage = null;
            this.IsDirty = false;
        }
        catch (Exception e)
        {
            this.ErrorMessage = e.Message;
            throw e;
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
        if (tileGrid.IsLoaded)
        {

            EditorGUILayout.LabelField("Tile Grid Loaded", UnityUtils.GetColorLabel(Color.green));
            EditorGUILayout.LabelField($"Dimensions: {tileGrid.Rows}x{tileGrid.Columns}");
        }
        else
        {
            EditorGUILayout.LabelField("Tile Grid Not Loaded", UnityUtils.GetColorLabel(Color.red));
        }

        if (!tileGrid.IsLoaded && tileGrid.ErrorMessage != null)
        {
            GUILayout.Space(20);
            EditorGUILayout.HelpBox(tileGrid.ErrorMessage, MessageType.Error);
        }


        GUILayout.Space(20);
        if (GUILayout.Button("Rebuild Tile Grid"))
        {
            tileGrid.GenerateGrid();
        }

        tileGrid.CleanUp();
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