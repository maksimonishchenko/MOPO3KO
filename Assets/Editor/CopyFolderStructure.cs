using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class CopyFolderStructure : EditorWindow 
{
	
	static CopyFolderStructure instance;
	public string pathToImages="";
	public string pathToPrefabs="";

	UnityEngine.Object objI;
	UnityEngine.Object objP;
	
	[MenuItem("Window/CopyFolderStructure")]
	static public void ShowEditor()
	{
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}
		
		instance = (CopyFolderStructure)EditorWindow.GetWindow(typeof(CopyFolderStructure), false, "CopyFolderStructure");
		
		instance.ShowUtility();
	}
	
	void OnGUI()
	{
		
		
		
		objI = EditorGUI.ObjectField (new Rect(0,0,400,20),"Directory of images: ",objI,typeof(UnityEngine.Object));
		//EditorGUI.
		objP = EditorGUI.ObjectField (new Rect(0,20,400,20),"Directory of prefabs: ",objP,typeof(UnityEngine.Object));
		
		
		
		if (GUI.Button(new Rect(0,40,100,20),"Copy") && objI!=null && objI!=objP)
		{
		
			pathToImages = AssetDatabase.GetAssetPath(objI); 			
		
			pathToPrefabs = AssetDatabase.GetAssetPath(objP); 			
			
			string[] dirPaths = Directory.GetDirectories(pathToImages, "*",
                                         SearchOption.AllDirectories);
			
			
			
			string lastPrefabPath = pathToPrefabs.Substring(pathToPrefabs.LastIndexOf(@"/") + 1,pathToPrefabs.Length - 1 - pathToPrefabs.LastIndexOf(@"/"));
			
			
			
		foreach(string ph in dirPaths)
		{
				
			
			int indOfEqual = ph.LastIndexOf(lastPrefabPath);
			int lastInfOfEqual = 	indOfEqual + lastPrefabPath.Length;
			string diff = ph.Substring(lastInfOfEqual,ph.Length - lastInfOfEqual);	
					
			try 
				{
					
					
					Directory.CreateDirectory(pathToPrefabs + diff);
				} 
			catch(InvalidCastException e) 
				{
					Debug.Log("Tryed to convert not a texture " +ph +  "error " + e.Message);
				}
			}
		}
	}

}
