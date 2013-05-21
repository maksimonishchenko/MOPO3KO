using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class ChangeTextureType : EditorWindow {
	
	static ChangeTextureType instance;
	public string path="";

	UnityEngine.Object obj;
	
	[MenuItem("Window/Change Texture Type &y")]
	static public void ShowEditor()
	{
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}
		
		instance = (ChangeTextureType)EditorWindow.GetWindow(typeof(ChangeTextureType), false, "Change Texture Type");
		
		instance.ShowUtility();
	}
	
	void OnGUI()
	{
		//path = EditorGUI.TextField(new Rect(0,0,400,20), "Directory: ",path);
		
		
		obj = EditorGUI.ObjectField (new Rect(0,0,400,20),"Directory: ",obj,typeof(UnityEngine.Object));
		
		//if (obj!=null)
		//	Debug.Log(AssetDatabase.GetAssetPath(obj));
		
		
		
		if (GUI.Button(new Rect(0,20,100,20),"Change prop") && obj!=null)
		{
		
		path = AssetDatabase.GetAssetPath(obj); 			
			//Object asset = AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D));
			
		string[] filePaths = Directory.GetFiles(path, "*",
                                         SearchOption.AllDirectories);
		
			
		foreach(string ph in filePaths)
		{

			Debug.Log(ph);
			try 
				{
					TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(ph);
					
					Debug.Log(importer.textureType.ToString());
					TextureImporterSettings ti = new TextureImporterSettings();
					
					//Assets/Textures/GUI/facebook.png	
					importer.ReadTextureSettings(ti);
					
					importer.textureType = TextureImporterType.GUI;
					importer.textureFormat  = TextureImporterFormat.AutomaticTruecolor;
					importer.filterMode  = FilterMode.Point;
					importer.isReadable = true;
					
					
					Debug.Log(importer.textureType.ToString());
					
					
					ti.ApplyTextureType(TextureImporterType.GUI,true);
					
					importer.SetTextureSettings(ti);
					
					AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
					
					AssetDatabase.ImportAsset(ph, ImportAssetOptions.ForceSynchronousImport);
					
					AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
					
					
					importer = (TextureImporter)TextureImporter.GetAtPath(ph);
					
					
					importer.textureFormat  = TextureImporterFormat.AutomaticTruecolor;
					importer.filterMode  = FilterMode.Point;
					importer.isReadable = true;
					
					AssetDatabase.ImportAsset(ph, ImportAssetOptions.ForceSynchronousImport);
					
					
					AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
					
					
					Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(ph,typeof(Texture2D));
					
					if (null != tex)
					{
						Color[] cc = tex.GetPixels();
						
						for(int i =0 ; i < cc.Length;i++)
						{
							if (cc[i].a == 0)
							{
								cc[i].r = 0;
								cc[i].g = 0;
								cc[i].b = 0;
							}
						}
							
							
						tex.SetPixels(cc);
							
						// Save the atlas as an asset:
						byte[] bytes = tex.EncodeToPNG();
						
						
						// Write out the atlas file:
						using (FileStream fs = File.Create(ph))
						{
							fs.Write(bytes, 0, bytes.Length);
							fs.Close();
						}
						
						
						// Flag this memory to be freed:
						bytes = null;
						AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
						//System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.
					}
					tex = null;
					importer = null;
					ti =null;
					
					
				} 
			catch(InvalidCastException e) 
				{
					Debug.Log("Tryed to convert not a texture " +ph +  "error " + e.Message);
				}
			}
		}
	}

}
