using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileGrid : MonoBehaviour
{

    private int rows = 1;
    private int columns = 1;
    public Transform TileContainer;
    private bool isDirty = true;

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

        if (this.TileContainer == null)
        {
            this.TileContainer = this.transform.Find("Container")?.gameObject.transform;
            if (this.TileContainer == null)
            {
                throw new MissingComponentException("Could not locate Tile Container.");
            }
        }
        this.GenerateGrid();

        this.isDirty = false;
    }

    public void GenerateGrid()
    {
        this.TileContainer = this.transform.Find("Container")?.gameObject.transform;
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
                newTile.AddComponent<Tile>().Build();
                newTile.transform.position = new Vector3(row + offsetX, 0, col + offsetZ);
                

            }
        }
    }

}
