using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Window/Find Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FindMissingScripts));
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in Selected Objects"))
        {
            FindInSelected();
        }
    }

    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        List<GameObject> missing = new List<GameObject>();
        foreach (GameObject g in go)
        {
            Component[] components = g.GetComponents<Component>();
            foreach (Component c in components)
            {
                if (c == null)
                {
                    missing.Add(g);
                    Debug.Log("Missing script found in: " + FullObjectPath(g), g);
                }
            }
        }
        if (missing.Count == 0)
        {
            Debug.Log("No missing scripts found in selected objects.");
        }
    }

    private static string FullObjectPath(GameObject go)
    {
        return go.transform.parent == null ? go.name : FullObjectPath(go.transform.parent.gameObject) + "/" + go.name;
    }
}
