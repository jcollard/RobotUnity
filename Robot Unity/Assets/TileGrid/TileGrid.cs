using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileGrid : MonoBehaviour
{

    private int rows = 1;
    private int columns = 1;
    private readonly Dictionary<(int, int), TileType> previousTileTypes = new Dictionary<(int, int), TileType>();
    private bool isDirty = true;

    public Transform TileContainer
    {
        get 
        {
            Transform container = this.transform.Find("Container")?.gameObject.transform;
            if (container == null)
            {
                GameObject containerObject = new GameObject("Container");
                containerObject.transform.parent = this.transform;
                containerObject.transform.localPosition = new Vector3();
                container = containerObject.transform;
            }

            return container;
        }
    }

    public int Rows
    {
        get => this.rows;
        set => this.SetRows(value);
    }

    public int Columns
    {
        get => this.columns;
        set => this.SetColumns(value);
    }

    public TileGrid SetRows(int rows)
    {
        if (rows < 1)
        {
            throw new ArgumentException($"Rows must be a positive number. Invalid input: {rows}");
        }

        if (this.rows != rows)
        {
            this.rows = rows;
            this.isDirty = true;
        }

        return this;
    }

    public TileGrid SetColumns(int columns)
    {
        if (columns < 1)
        {
            throw new ArgumentException($"Columns must be a positive number. Invalid input: {columns}");
        }

        if (this.columns != columns)
        {
            this.columns = columns;
            this.isDirty = true;
        }

        return this;
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
        for (int row = 0; row < this.rows; row++)
        {
            for (int col = 0; col < this.columns; col++)
            {
                Transform child = this.TileContainer.Find($"(row: {row}, col: {col})");
                if (child == null)
                {
                    continue;
                }

                Tile t = child.gameObject.GetComponent<Tile>();
                if (t == null)
                {
                    continue;
                }

                previousTileTypes[(row, col)] = t.TileType;
            }
        }


        List<Transform> children = this.TileContainer.Cast<Transform>().ToList();
        foreach(Transform child in children)
        {
            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }

        float offsetX = -((float)(this.rows - 1)) * 0.5f;
        float offsetZ = -((float)(this.columns - 1)) * 0.5f;
        for (int row = 0; row < this.rows; row++)
        {
            for (int col = 0; col < this.columns; col++)
            {
                GameObject newTile = new GameObject();
                newTile.name = $"(row: {row}, col: {col})";
                newTile.transform.parent = this.TileContainer;
                Tile t = newTile.AddComponent<Tile>();
                if (previousTileTypes.ContainsKey((row, col)))
                {
                    t.TileType = previousTileTypes[(row, col)];
                }
                newTile.transform.position = new Vector3(row + offsetX, 0, col + offsetZ);
                t.Build();
                

            }
        }
    }

}
