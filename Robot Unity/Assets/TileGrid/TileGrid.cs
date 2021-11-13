using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Collard;
using UnityEditor.SceneManagement;

public class TileGrid : MonoBehaviour
{

    [SerializeField]
    private TextAsset mapFile;

    [SerializeField]
    private TileFactory factory;

    [NonSerialized]
    private bool IsDirty = true;
    public bool IsLoaded
    {
        get => this.IsDirty == false && this.ErrorMessage == null;
    }
    public string ErrorMessage { get; private set; }

    public int Rows { get; private set; }
    public int Columns { get; private set; }

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
        try
        {
            char[,] grid = MapFileParser.Instance.Parse(lines);
            this.Rows = grid.GetLength(0);
            this.Columns = grid.GetLength(1);
            float offsetX = -((float)(this.Rows - 1)) * 0.5f;
            float offsetZ = -((float)(this.Columns - 1)) * 0.5f;

            foreach ((int row, int col, char c) in Utils.Get2DEnumerable(grid))
            {
                List<GameObject> newTile;
                if (this.factory == null)
                {
                    newTile = new List<GameObject>();
                    newTile.Add(TileGrid.GetDefaultTile());
                }
                else if (this.factory.IsValidTile(c))
                {
                    newTile = this.factory.GetTile(c);
                    if (newTile == null)
                    {
                        throw new InvalidTileCharacterException($"Attempted to load Tile for {c} but resulted in a null List<GameObject>.");
                    }
                }
                else
                {
                    throw new InvalidTileCharacterException($"An invalid tile character {c} was found at {row}x{col}.");
                }
                foreach(GameObject newObject in newTile)
                {
                    newObject.transform.parent = this.transform;
                    newObject.transform.localPosition = new Vector3(row + offsetX, 0, col + offsetZ);
                }
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
        EditorGUI.BeginChangeCheck();

        TileGrid tileGrid = (TileGrid)target;
        tileGrid.MapFile = (TextAsset)EditorGUILayout.ObjectField("Map File", tileGrid.MapFile, typeof(TextAsset), false);
        tileGrid.Factory = (TileFactory)EditorGUILayout.ObjectField("Tile Factory", tileGrid.Factory, typeof(TileFactory), true);

        GUILayout.Space(20);
        if (tileGrid.IsLoaded)
        {

            EditorGUILayout.LabelField("Tile Grid Loaded", UnityUtils.GetColorLabel(Color.green));
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

        EditorGUILayout.TextArea(tileGrid.MapFile.text, UnityUtils.MonoSpacedTextArea);


        GUILayout.Space(20);
        if (GUILayout.Button("Rebuild Tile Grid"))
        {
            tileGrid.GenerateGrid();
        }

        tileGrid.CleanUp();

        if (EditorGUI.EndChangeCheck())
        {
            // This code will unsave the current scene if there's any change in the editor GUI.
            // Hence user would forcefully need to save the scene before changing scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

public class TileFactory : MonoBehaviour
{
    public void Start()
    {
        this.gameObject.SetActive(false);
    }

    public virtual List<GameObject> GetTile(char ch)
    {
        throw new NotImplementedException("Method not Implemented");
    }

    public virtual bool IsValidTile(char ch)
    {
        throw new NotImplementedException("Method not Implemented");
    }
}