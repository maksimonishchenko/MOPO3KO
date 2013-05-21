using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class ChangeMaterial : EditorWindow {
	
	static ChangeMaterial instance;
	public string path="";

	UnityEngine.Material obj;
	
	List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
	
	[MenuItem("Window/Change Material &l")]
	static public void ShowEditor()
	{
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}
		
		instance = (ChangeMaterial)EditorWindow.GetWindow(typeof(ChangeMaterial), false, "Change Material");
		
		instance.ShowUtility();
	}
	
	void OnGUI()
	{
		obj = (UnityEngine.Material)EditorGUI.ObjectField (new Rect(0,0,400,20),"Material: ",obj,typeof(UnityEngine.Material));
				
		if (GUI.Button(new Rect(0,110,100,20),"Change prop") && obj!=null)
		{
		
			foreach(UnityEngine.Object o in objs)
			{
				if (o is GameObject)
				{
					
					GameObject l = (GameObject)o;
					changeMaterial(l);
					
				}
			}
		
		}
		
		if (GUI.Button(new Rect(0,140,100,20),"Clear objs") && objs!=null)
		{
			objs.Clear();
		}
		
		int i =0;
		foreach(UnityEngine.Object o in objs)
		{
			if (o is GameObject)
			{
				GameObject l = (GameObject)o;
				GUI.Label(new Rect(0,160+25*i,100,20),l.name);
				i++;
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
	void changeMaterial(GameObject g)
	{
		if (g.renderer)
			g.renderer.material = obj;
		foreach(Transform t in g.transform)
		{
			changeMaterial(t.gameObject);
		}
	}
}
