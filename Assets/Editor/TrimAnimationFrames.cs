using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class TrimAnimationFrames : EditorWindow {
	
	public enum ANCHOR_METHOD
	{
		UPPER_LEFT,
		UPPER_CENTER,
		UPPER_RIGHT,
		MIDDLE_LEFT,
		MIDDLE_CENTER,
		MIDDLE_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_CENTER,
		BOTTOM_RIGHT
	}
	
	public ANCHOR_METHOD anchor = ANCHOR_METHOD.MIDDLE_CENTER;
	
	static TrimAnimationFrames instance;
	public string path="";
	
	public float alphaAllowence = 0.01f;
	
	UnityEngine.Object obj;
	
	[MenuItem("Window/Trim Animation Frames &f")]
	static public void ShowEditor()
	{
		if (instance != null)
		{
			instance.ShowUtility();
			return;
		}
		
		instance = (TrimAnimationFrames)EditorWindow.GetWindow(typeof(TrimAnimationFrames), false, "Trim Animation Frames");
		
		instance.ShowUtility();
	}
	
	void OnGUI()
	{
		//path = EditorGUI.TextField(new Rect(0,0,400,20), "Directory: ",path);
		
		
		obj = EditorGUI.ObjectField (new Rect(0,0,400,20),"Directory: ",obj,typeof(UnityEngine.Object));
		
		//if (obj!=null)
		//	Debug.Log(AssetDatabase.GetAssetPath(obj));
		
		alphaAllowence = EditorGUI.FloatField(new Rect(0,20,400,20),"Alpha Allowence: ",alphaAllowence);
		
		anchor = (ANCHOR_METHOD)EditorGUI.EnumPopup(new Rect(0,40,400,20),"Anchor: ",anchor);
		
		if (GUI.Button(new Rect(0,60,100,20),"Trim Frames") && obj!=null)
		{
		
			path = AssetDatabase.GetAssetPath(obj); 
			
				//Object asset = AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D));
				
			string[] filePaths = Directory.GetFiles(path, "*",
	                                         SearchOption.AllDirectories);
			
				
			foreach(string ph in filePaths)
			{
	
				Debug.Log(ph);
				try {
						
						TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(ph);
						
						importer.textureFormat  = TextureImporterFormat.AutomaticTruecolor;
						importer.filterMode  = FilterMode.Point;
						importer.isReadable = true;
			
						AssetDatabase.ImportAsset(ph, ImportAssetOptions.ForceSynchronousImport);
					
					
						//AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
					
						Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(ph,typeof(Texture2D));
					
					if (tex)
					{
						Rect r = GetOccupiedRect(tex,alphaAllowence);
						Debug.Log(r.ToString());
						Rect result = new Rect(r);
						switch(anchor)
						{
							case ANCHOR_METHOD.UPPER_LEFT:
								//UP
								result.y = r.y;
								result.height=tex.height-result.y;
							
								//LEFT
								result.x = 0;
								result.width=r.x+r.width;
							
							
								break;
							case ANCHOR_METHOD.UPPER_CENTER:
								//UP
								result.y = r.y;
								result.height=tex.height-result.y;
							
								//CENTER
								if (r.x > tex.width-r.x-r.width)
									result.x = tex.width-r.x-r.width;
								else
									result.x = r.x;
								result.width=tex.width- 2*result.x;
							
								break;
							case ANCHOR_METHOD.UPPER_RIGHT:
								//UP
								result.y = r.y;
								result.height=tex.height-result.y;
							
								//RIGHT
								result.x = r.x;
								result.width=tex.width-result.x;
							
							
								break;
							case ANCHOR_METHOD.MIDDLE_LEFT:
								//MIDDLE
								if (r.y > tex.height-r.y-r.height)
									result.y = tex.height-r.y-r.height;
								else
									result.y = r.y;
								result.height=tex.height- 2*result.y;

								//LEFT
								result.x = 0;
								result.width=r.x+r.width;							
							
								break;
							case ANCHOR_METHOD.MIDDLE_CENTER:
								//MIDDLE
								if (r.y > tex.height-r.y-r.height)
									result.y = tex.height-r.y-r.height;
								else
									result.y = r.y;
								result.height=tex.height- 2*result.y;

								//CENTER
								if (r.x > tex.width-r.x-r.width)
									result.x = tex.width-r.x-r.width;
								else
									result.x = r.x;
								result.width=tex.width- 2*result.x;
							
								break;
							case ANCHOR_METHOD.MIDDLE_RIGHT:
								//MIDDLE
								if (r.y > tex.height-r.y-r.height)
									result.y = tex.height-r.y-r.height;
								else
									result.y = r.y;
								result.height=tex.height- 2*result.y;

								//RIGHT
								result.x = r.x;
								result.width=tex.width-result.x;							
							
								break;
							case ANCHOR_METHOD.BOTTOM_LEFT:
								//DOWN
								result.y = 0;
								result.height=r.y+r.height;

								//LEFT
								result.x = 0;
								result.width=r.x+r.width;							
							
								break;
							case ANCHOR_METHOD.BOTTOM_CENTER:
								//DOWN
								result.y = 0;
								result.height=r.y+r.height;
								
								//CENTER
								if (r.x > tex.width-r.x-r.width)
									result.x = tex.width-r.x-r.width;
								else
									result.x = r.x;
								result.width=tex.width- 2*result.x;							
								break;
							case ANCHOR_METHOD.BOTTOM_RIGHT:
								//DOWN
								result.y = 0;
								result.height=r.y+r.height;
							
								//RIGHT
								result.x = r.x;
								result.width=tex.width-result.x;
							
								break;
														
						}
						
						Texture2D tt = SnipArea(tex,result);
						
						//AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
						
						
						// Save the atlas as an asset:
						byte[] bytes = tt.EncodeToPNG();
					
						
						//AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
						
						
						tt = null;
						// Write out the atlas file:
						using (FileStream fs = File.Create(ph))
						{
							fs.Write(bytes, 0, bytes.Length);
							fs.Close();
						}
						//Debug.Log("after write " + ph + "   " + System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64);
						
						// Flag this memory to be freed:
						bytes = null;
						//AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
						//EditorApplication.OpenScene(EditorApplication.
					}
					
				
				} catch(InvalidCastException e) {
					Debug.Log("Tryed to convert not a texture " +ph);
				}
				
			}
		
		}
	}
	
	
	
		// Returns a Rect indicating the area of the texture that
	// contains non-transparent pixels:
	Rect GetOccupiedRect(Texture2D tex, float alpha)
	{
		Rect area = new Rect(0, 0, 0, 0);
		int x, y;
		Color[] pixels;

		// NOTE: GetPixel() assumes the Y-axis runs from
		// 0 at the bottom to N at the top, just like UV
		// coordinates.

		pixels = tex.GetPixels(0);

		// Find the first column containing non-zero alpha:
		for (x = 0; x < tex.width; ++x)
		{
			for (y = 0; y < tex.height; ++y)
			{
				if (pixels[y * tex.width + x].a > alpha)
				{
					area.x = x;
					x = tex.width;
					break;
				}
			}
		}

		// Find the bottom-most row containing non-zero alpha:
		for (y = 0; y < tex.height; ++y)
		{
			for (x = 0; x < tex.width; ++x)
			{
				if (pixels[y * tex.width + x].a > alpha)
				{
					area.y = y;
					y = tex.height;
					break;
				}
			}
		}

		// Find the last column containing non-zero alpha:
		for (x = tex.width - 1; x >= 0; --x)
		{
			for (y = 0; y < tex.height; ++y)
			{
				if (pixels[y * tex.width + x].a > alpha)
				{
					area.xMax = x + 1;
					x = 0;
					break;
				}
			}
		}

		// Find the top-most row containing non-zero alpha:
		for (y = tex.height - 1; y >= 0; --y)
		{
			for (x = 0; x < tex.width; ++x)
			{
				if (pixels[y * tex.width + x].a > alpha)
				{
					area.yMax = y + 1;
					y = 0;
					break;
				}
			}
		}

		// Check for an empty frame, and in which case, 
		// leave a 2x2 pixel area:
		if (area.width == 0 || area.height == 0)
			area = new Rect(0, 0, 2f, 2f);

		return area;
	}
	
	// Returns a texture that consists entirely of only
	// the pixels in the area specified from the source
	// texture:
	Texture2D SnipArea(Texture2D tex, Rect area)
	{
		if (tex.width == area.width && tex.height == area.height)
			return tex;

		Texture2D newTex = new Texture2D((int)area.width, (int)area.height, TextureFormat.ARGB32, false);

		newTex.SetPixels(tex.GetPixels((int)area.xMin, (int)area.yMin, (int)area.width, (int)area.height, 0));
		//newTex.Apply(false);

		return newTex;
	}
	
}
