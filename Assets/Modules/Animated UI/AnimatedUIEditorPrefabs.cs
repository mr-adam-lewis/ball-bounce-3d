using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Custom Menu Items to create the prefabs directly from the Unity menu
/// </summary>
public static class AnimatedUIEditorPrefabs
{
    private const string dir = "GameObject/Animated UI/";
	private const int priority = 10;
	
	[MenuItem(dir + "Button",false,priority)]
	static void CreateButton(MenuCommand menuCommand)
	{
		InstantiateGameObject(menuCommand, "Button");
	}
	
	[MenuItem(dir + "Icon Button",false,priority)]
	static void CreateIconButton(MenuCommand menuCommand)
	{
		InstantiateGameObject(menuCommand, "Icon Button");
	}
	
	[MenuItem(dir + "Slider",false,priority)]
	static void CreateSlider(MenuCommand menuCommand)
	{
		InstantiateGameObject(menuCommand, "Slider");
	}
	
	[MenuItem(dir + "Switch",false,priority)]
	static void CreateSwitch(MenuCommand menuCommand)
	{
		InstantiateGameObject(menuCommand, "Switch");
	}
	
	[MenuItem(dir + "Selector",false,priority)]
	static void CreateSelector(MenuCommand menuCommand)
	{
		InstantiateGameObject(menuCommand, "Selector");
	}

    static void InstantiateGameObject(MenuCommand menuCommand, string name)
    {
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Modules/Animated UI/Prefabs/" + name + ".prefab", typeof(GameObject));
        if (!prefab)
        {
			Debug.LogError("Prefab '" + name + ".prefab' missing from '" + "Assets/Modules/Animated UI/Prefabs/', make sure you have placed the prefab where it should be at.");
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
