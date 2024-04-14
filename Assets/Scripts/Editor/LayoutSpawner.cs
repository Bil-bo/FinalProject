using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LayoutSpawner : EditorWindow
{
    [MenuItem("Window/Update Prefab List")]
    public static void ShowWindow()
    {
        GetWindow<LayoutSpawner>();
    }

    private GameObject Spawner;

    void OnGUI()
    {
        GUILayout.Label("Populate Prefab List", EditorStyles.boldLabel);

        // Add a field to select the target object from the scene
        Spawner = EditorGUILayout.ObjectField("Spawner", Spawner, typeof(GameObject), true) as GameObject;

        if (GUILayout.Button("Populate List"))
        {
            PopulateList();
        }
    }

    private void PopulateList()
    {
        if (Spawner != null) 
        {
            GroundSpawner realSpawner = Spawner.GetComponent<GroundSpawner>();
            Debug.Log("Here");
            if (realSpawner != null) 
            {
                Debug.Log("NOw Here");
                realSpawner.LayoutList.Clear();

                string[] prefabs = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/Prefabs/Layouts" });

                foreach (string path in prefabs)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(path);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

                    if (prefab != null)
                    {
                        Debug.Log("Finally Here");
                        realSpawner.LayoutList.Add(prefab);
                        Debug.Log(prefab.name);
                    }
                }
            }
        }

    }

}
