using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnerFiller: EditorWindow
{
    [MenuItem("Window/Update Prefab List")]
    public static void ShowWindow()
    {
        GetWindow<SpawnerFiller>();
    }

    private GameObject Spawner;

    private SceneIndex Scene;

    void OnGUI()
    {
        GUILayout.Label("Populate Prefab List", EditorStyles.boldLabel);

        // Add a field to select the target object from the scene
        Spawner = EditorGUILayout.ObjectField("Spawner", Spawner, typeof(GameObject), true) as GameObject;
        Scene = (SceneIndex)EditorGUILayout.EnumPopup("Spawner Type", Scene);

        if (GUILayout.Button("Populate List"))
        {
            PopulateList();
        }
    }

    private void PopulateList()
    {
        if (Spawner != null) 
        {
            LayoutManager realSpawner = Spawner.GetComponent<LayoutManager>();
            if (realSpawner != null) 
            {

                realSpawner.LayoutList.Clear();

                string[] prefabs = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/Prefabs/Layouts/" + Scene });

                foreach (string path in prefabs)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(path);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

                    if (prefab != null)
                    {
                        realSpawner.LayoutList.Add(prefab);
                        Debug.Log(prefab.name);
                    }
                }
            }
        }

    }

}
