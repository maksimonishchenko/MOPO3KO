using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class Prefabolize : EditorWindow {
	
	static Prefabolize instance;
	public string path="";
		
	List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
	
	List<UnityEngine.GameObject> prefabs = new List<UnityEngine.GameObject>();
	
	[MenuItem("Window/Prefabolize &p")]
	static public void ShowEditor()
	{
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}
		
		instance = (Prefabolize)EditorWindow.GetWindow(typeof(Prefabolize), false, "Prefabolize");
		
		instance.ShowUtility();
	}
	
	void OnGUI()
	{
		
		if (GUI.Button(new Rect(0,110,100,20),"Prefabolize") && objs !=null)
		{
			foreach(GameObject oo in objs)
			{
				PackedSprite[] sprs = oo.GetComponentsInChildren<PackedSprite>();
				
				foreach(PackedSprite sp in sprs)
				{
					bool Found = false;
					
					foreach(UnityEngine.Object o in prefabs)
					{
						if (o is GameObject)
						{
							GameObject l = (GameObject)o;
							PackedSprite p = l.GetComponent<PackedSprite>();
							if (p!=null)
							{
								if (p.staticTexPath == sp.staticTexPath)
								{
									GameObject g = (GameObject)EditorUtility.InstantiatePrefab(l);
									g.transform.position = sp.transform.position;
									g.transform.rotation = sp.transform.rotation;
									g.transform.parent=sp.transform.parent;
									PackedSprite ppp = g.GetComponent<PackedSprite>();
									ppp.SetAnchor(sp.anchor);
									ppp.doMirror = sp.doMirror;
									DestroyImmediate(sp.gameObject);
									Found = true;
								}
							}
						}
					}
					
					if (!Found)
					{
						 createNew(sp.gameObject); 
					}
				}	
			}
			
			objs.Clear();
			prefabs.Clear();
		}
		
		if (GUI.Button(new Rect(0,140,100,20),"Clear objs") && objs!=null)
		{
			objs.Clear();
			prefabs.Clear();
		}
		
		int i =0;
		foreach(UnityEngine.Object o in objs)
		{
			if (o is GameObject)
			{
				GameObject l = (GameObject)o;
				if (l)
				{
					GUI.Label(new Rect(0,160+25*i,100,20),l.name);
					i++;
				}
			}
		}
		
		// Handle drag and drop from an outside source:
		EventType eventType = Event.current.type;
		if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
		{
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

			if (eventType == EventType.DragPerform)
			{
					DragAndDrop.AcceptDrag();
					objs.AddRange(DragAndDrop.objectReferences);
			}

			Event.current.Use();
			
		}

		
	}

	public void createNew(GameObject obj) {
		
		String localPath = "Assets/Prefabs/buff/" + obj.name + ".prefab";	
        UnityEngine.Object prefab = EditorUtility.CreateEmptyPrefab(localPath);
        EditorUtility.ReplacePrefab(obj, prefab);
        AssetDatabase.Refresh();
		
		prefab = AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject));
		prefabs.Add((GameObject)prefab);
		
		//PackedSprite sp = obj.GetComponent<PackedSprite>();
		
        GameObject g = (GameObject)EditorUtility.InstantiatePrefab(prefab); 
		
		
		
		g.transform.position = obj.transform.position;
		g.transform.rotation = obj.transform.rotation;
		g.transform.parent = obj.transform.parent;
		
		
        DestroyImmediate(obj);
		
		
    }
	
}
