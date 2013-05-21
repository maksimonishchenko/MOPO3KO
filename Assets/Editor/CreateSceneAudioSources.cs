using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class CreateSceneAudioSources : EditorWindow 
{
	
	static CreateSceneAudioSources instance;
	public string pathToAudioAssets="";

	UnityEngine.Object objI;
	
	[MenuItem("Window/Create Scene AudioSources")]
	static public void ShowEditor()
	{
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}
		
		instance = (CreateSceneAudioSources)EditorWindow.GetWindow(typeof(CreateSceneAudioSources), false, "Create Scene AudioSources");
		
		instance.ShowUtility();
	}
	
	void OnGUI()
	{
		
		
		
		objI = EditorGUI.ObjectField (new Rect(0,0,400,20),"Directory with audio assets: ",objI,typeof(UnityEngine.Object));
		
		
		
		
		if (GUI.Button(new Rect(0,40,100,20),"Create") && objI!=null)
		{
		
			pathToAudioAssets = AssetDatabase.GetAssetPath(objI); 			
		
						
			
			string[] audioPaths = Directory.GetFiles(pathToAudioAssets, "*",
                                         SearchOption.AllDirectories);
			
			foreach(string ph in audioPaths)
			{
					
				
				GameObject audioSource = new GameObject();
				audioSource.name = ph.Substring(ph.LastIndexOf(@"\"),ph.Length - ph.LastIndexOf(@"\"));
				AudioSource newSource = audioSource.AddComponent<AudioSource>();
				newSource.clip = AssetDatabase.LoadAssetAtPath(ph,typeof(AudioClip)) as AudioClip;
				newSource.playOnAwake = false;
			}
		}	
	}
}
