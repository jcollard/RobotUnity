using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Tile : MonoBehaviour
{
    private static readonly Dictionary<TileType, string> tileResourceLookup;
    private static readonly Dictionary<TileType, (string, GameObject)> tilePrefabLookup;

    public TileGrid Parent {
        get 
        {
            TileGrid parent = this.gameObject.GetComponentInParent<TileGrid>();
            if (parent == null)
            {
                throw new NullReferenceException("Tile must be within a parent TileGrid.");
            }
            return parent;
        }
    }

    private static (string, GameObject) GetPrefab(TileType tileType)
    {
        if (!tilePrefabLookup.ContainsKey(tileType))
        {
            string name = tileResourceLookup[tileType];
            tilePrefabLookup[tileType] = (name, Resources.Load<GameObject>($"Tiles/{name}"));
        }

        return tilePrefabLookup[tileType];
    }

    static Tile()
    {
        tilePrefabLookup = new Dictionary<TileType, (string, GameObject)> ();
        tileResourceLookup = new Dictionary<TileType, string> ();
        tileResourceLookup[TileType.Floor] = "Floor Tile";
        tileResourceLookup[TileType.Wall] = "Wall Tile";

        foreach(TileType t in Enum.GetValues(typeof(TileType)))
        {
            if(!tileResourceLookup.ContainsKey(t))
            {
                throw new Exception($"No tile associated with TileType {t}");
            }
        }
    }

    private TileType tileType = TileType.Floor;
    private bool isDirty = true;

    public TileType TileType {
        get => this.tileType;
        set => this.SetType(value);
    } 
    
    public void SetType(TileType tileType)
    {
        if (this.tileType == tileType)
        {
            return;
        }

        this.tileType = tileType;
        this.isDirty = true;
    }

    public void CleanUp()
    {
        if (!this.isDirty)
        {
            return;
        }
        this.Build();
        this.isDirty = false;
    }

    public void Build()
    {
        foreach(Transform child in this.transform.Cast<Transform>().ToList<Transform>())
        {
            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }

        (string name, GameObject prefab) = Tile.GetPrefab(this.tileType);
        GameObject floorTile = UnityEngine.Object.Instantiate<GameObject>(prefab);
        floorTile.name = name;
        floorTile.transform.parent = this.transform;
        floorTile.transform.localPosition = new Vector3();
    }
}

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Tile tile = (Tile)target;
        //tile.TileType = (TileType)EditorGUILayout.EnumFlagsField("Tile Type", tile.TileType);
        tile.TileType = (TileType)EditorGUILayout.EnumPopup("Tile Type", tile.TileType);
        tile.CleanUp();
    }

}

public enum TileType
{
    Floor,
    Wall
}
