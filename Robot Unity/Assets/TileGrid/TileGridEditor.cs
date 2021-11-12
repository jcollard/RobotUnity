using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        TileGrid tileGrid = (TileGrid)target;

        tileGrid.rows = EditorGUILayout.IntField("Rows", tileGrid.rows);
        tileGrid.columns = EditorGUILayout.IntField("Columns", tileGrid.columns);
        
        Debug.Log($"Rows: {tileGrid.rows}");
    }

}