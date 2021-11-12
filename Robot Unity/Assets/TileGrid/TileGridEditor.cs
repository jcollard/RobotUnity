using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        TileGrid tileGrid = (TileGrid)target;

        
        EditorGUILayout.LabelField("Dimensions");
        tileGrid.Rows = EditorGUILayout.IntField("Rows", tileGrid.Rows);
        tileGrid.Columns = EditorGUILayout.IntField("Columns", tileGrid.Columns);

        GUILayout.Space(20);
        if (GUILayout.Button("Rebuild Tile Grid"))
        {
            tileGrid.CleanUp();
            tileGrid.GenerateGrid();
        }

        
        //EditorGUILayout.HelpBox("The rows and columns must be >0.", MessageType.Info);
        
        tileGrid.CleanUp();

        if (tileGrid.TileContainer == null)
        {
            EditorGUILayout.HelpBox("Missing a Container Reference!", MessageType.Error);
        }

    }  
}