namespace Collard
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class UnityUtils
    {
        private static readonly Dictionary<Color, GUIStyle> ColorLabels = new Dictionary<Color, GUIStyle>();

        public static GUIStyle GetColorLabel(Color color)
        {
            if (!ColorLabels.TryGetValue(color, out GUIStyle style))
            {
                style = new GUIStyle(EditorStyles.label);
                style.normal.textColor = color;
            }

            return style;
        }

        public static void DestroyImmediateChildren(Transform t)
        {
            List<Transform> children = t.Cast<Transform>().ToList();
            foreach (Transform child in children)
            {
                UnityEngine.Object.DestroyImmediate(child.gameObject);
            }
        }
    }
}