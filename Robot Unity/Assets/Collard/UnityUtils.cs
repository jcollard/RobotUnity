namespace Collard
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    public class UnityUtils
    {
        public static void DeleteChildren(Transform t)
        {
            List<Transform> children = t.Cast<Transform>().ToList();
            foreach (Transform child in children)
            {
                UnityEngine.Object.DestroyImmediate(child.gameObject);
            }
        }
    }
}