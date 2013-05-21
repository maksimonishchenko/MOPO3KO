using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class ChangeAllPackedSprites : EditorWindow 
{
	
	static ChangeAllPackedSprites instance;


	int frameRate = 30;
	
	[MenuItem("Window/Change Framerate")]
	static public void ShowEditor()
	{
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}
		
		instance = (ChangeAllPackedSprites)EditorWindow.GetWindow(typeof(ChangeAllPackedSprites), false, "Change Framerate");
		
		instance.ShowUtility();
	}
	
	void OnGUI()
	{
		
		
		
		frameRate = EditorGUI.IntField(new Rect(0,0,400,20),"Framerate ",frameRate);
		
		
		
		
		if (GUI.Button(new Rect(0,40,100,20),"Assign") && frameRate>0)
		{
			Debug.Log("into utto asign");
			foreach(GameObject g in GameObject.FindObjectsOfType(typeof(GameObject)))
			{
				PackedSprite ps = g.GetComponent<PackedSprite>();
				if (null != ps && null != ps.textureAnimations && ps.textureAnimations.Length>0)
				{
						ps.textureAnimations[0].framerate = frameRate;
						PrefabUtility.ReplacePrefab(g,PrefabUtility.GetPrefabParent(g),ReplacePrefabOptions.ConnectToPrefab);
				}
				
				
			}
			
		}	
	}
}
