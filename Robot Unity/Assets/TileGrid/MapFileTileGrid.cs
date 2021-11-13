using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MapFileTileGrid : MonoBehaviour
{

    private TextAsset mapFile;
    private MapFileParser parser;
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
        set
        {
            this.mapFile = value;
        }
    }

    public MapFileParser Parser
    {
        get => this.parser;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException("Parser must be non-null.");
            }

            this.parser = value;
        }
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
        List<Transform> children = this.transform.Cast<Transform>().ToList();
        foreach(Transform child in children)
        {
            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }

        using (StringReader reader = new StringReader(mapFile.text))
        {
            string firstLine = reader.ReadLine();
            (int rows, int columns) = this.Parser == null ? DefaultParseSize(firstLine) : this.Parser.ParseSize(firstLine);
            this.grid = new GameObject[rows, columns];
            if (rows < 1 || columns < 1)
            {
                throw new ArgumentException($"Could not create grid. Expected rows and columns greater than 0 but received (rows: {rows}, columns: {columns}).");
            }

            this.rows = rows;
            this.columns = columns;
            float offsetX = -((float)(this.rows - 1)) * 0.5f;
            float offsetZ = -((float)(this.columns - 1)) * 0.5f;
            int row = 1;
            string line;

            while ((line = reader.ReadLine()) != null)
            {

                if (row > this.rows)
                {
                    throw new ArgumentException($"Expected {this.rows} rows but found at least {row} rows.");
                }

                List<GameObject> objects = this.Parser != null ? this.Parser.ParseLine(line) : MapFileTileGrid.DefaultParseLine(line);

                if (objects.Count != this.columns)
                {
                    throw new ArgumentException($"Expected {this.columns} columns but found {objects.Count} columns on line {row}.");
                }


                for (int r = row - 1, c = 0; c < objects.Count; c++)
                {
                    if (objects[c] == null)
                    {
                        throw new NullReferenceException($"Found a null GameObject on line {row + 1} at column {c}.");
                    }
                    this.grid[r, c] = objects[c];
                    this.grid[r, c].transform.parent = this.transform;
                    this.grid[r, c].transform.localPosition = new Vector3(r + offsetX, 0, c + offsetZ);
                }

                row++;
            }
            row--;

            if (row != this.rows)
            {
                throw new ArgumentException($"Expected {this.rows} rows but  found {row} rows.");
            }
        }
    }

    private static (int, int) DefaultParseSize(string line)
    {
        string[] str = line.Split('x');
        int rows = Int32.Parse(str[0]);
        int columns = Int32.Parse(str[1]);
        return (rows, columns);
    }

    private static List<GameObject> DefaultParseLine(string line)
    {
        List<GameObject> objects = new List<GameObject>();
        foreach(char c in line)
        {
            GameObject defaultObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            defaultObject.name = "Default Tile";
            defaultObject.transform.localScale = new Vector3(.98f, 0.1f, .98f);
            objects.Add(defaultObject);
        }
        return objects;
    }

}

[CustomEditor(typeof(MapFileTileGrid))]
public class MapFileTileGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapFileTileGrid tileGrid = (MapFileTileGrid)target;

        tileGrid.MapFile = (TextAsset)EditorGUILayout.ObjectField("Map File", tileGrid.MapFile, typeof(TextAsset), false);
        EditorGUILayout.ObjectField("Parser Class", tileGrid.Parser, typeof(MapFileParser), true);


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

public class MapFileParser : MonoBehaviour
{

    public virtual (int, int) ParseSize(string line)
    {
        return (1, 1);
    }

    public virtual List<GameObject> ParseLine(string line)
    {
        List<GameObject> objects = new List<GameObject>();
        GameObject defaultObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        defaultObject.transform.localScale = new Vector3(1, 0.1f, 1);
        objects.Add(defaultObject);
        return objects;
    }

}