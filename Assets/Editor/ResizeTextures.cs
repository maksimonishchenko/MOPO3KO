using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class ResizeTextures : EditorWindow 
{
	
	static ResizeTextures instance;
	
	

	TextureFormat format = TextureFormat.Alpha8;
	int delimiter = 2;
	
	[MenuItem("Window/Resize Textures")]
	static public void ShowEditor()
	{
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}
		
		instance = (ResizeTextures)EditorWindow.GetWindow(typeof(ResizeTextures), false, "Resize Textures");
		
		instance.ShowUtility();
	}
	
	void OnGUI()
	{
	
		delimiter = EditorGUI.IntField (new Rect(0,20,400,20),"Proportion",delimiter);
		format =  (TextureFormat) EditorGUI.EnumPopup(new Rect(0,45,400,20),(System.Enum) format);
		
		
		if (GUI.Button(new Rect(0,65,100,20),"Resize"))
		{			
			UnityEngine.Object[] texObjs = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
			foreach(UnityEngine.Object sourceTex in texObjs)
			{
				Texture2D source = sourceTex as Texture2D;
				
				
				Texture2D tempTex = (Texture2D)Instantiate(source);
				tempTex.name =source.name+"halfres";
				tempTex.Resize(source.width/2, source.height/2, format, false);
				tempTex.Apply(false);
				
				var bytes = tempTex.EncodeToPNG();
				
     			File.WriteAllBytes(AssetDatabase.GetAssetPath(source)+"h", bytes);
			}	
			
	
			
		}
	}
}

