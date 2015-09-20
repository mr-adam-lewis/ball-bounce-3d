using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Custom Menu Items to create the prefabs directly from the Unity menu
/// </summary>
public static class PopupsEditorPrefabs
{
    private const string dir = "GameObject/Popups/";
	private const int priority = 10;
	
	[MenuItem(dir + "Popup Canvas",false,priority)]
	static void CreatePopupCanvas(MenuCommand menuCommand)
	{
		InstantiateGameObject(menuCommand, "Popup Canvas");
	}
	
	[MenuItem(dir + "Popup",false,priority)]
	static void CreatePopup(MenuCommand menuCommand)
	{
		InstantiateGameObject(menuCommand, "Popup");
	}

    static void InstantiateGameObject(MenuCommand menuCommand, string name)
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Modules/Popups/" + name + ".prefab", typeof(GameObject));
        if (!prefab)
        {
			Debug.LogError("Prefab '" + name + ".prefab' missing from '" + "Assets/Modules/Popups/', make sure you have placed the prefab where it should be at.");
            return;
        }
        GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(prefab);


        if (menuCommand.context)
        {
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        }
        else if (Selection.activeObject is GameObject) {
            GameObjectUtility.SetParentAndAlign(go, Selection.activeObject as GameObject);
        }

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
