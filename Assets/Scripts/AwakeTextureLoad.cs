using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AwakeTextureLoad : MonoBehaviour 
{
	
	public Material[] materialsToLoad;
	public SpriteText[] spritesToLoad;
	
	public GameObject[] afterLoadActivate;
	
	private static List<LevMat> unmanagedMaterials;
	private static List<LevSpriteText> unmanagedSpriteTexts;
	
	private class LevMat
	{
		public int Level = -1;
		public Material Mater = null;
		public LevMat(int l ,Material m)
		{
			this.Level = l;
			this.Mater = m;
		}
		
	}
	
	private class LevSpriteText
	{
		public int Level = -1;
		public SpriteText SText = null;
		
		public LevSpriteText(int l ,SpriteText t)
		{
			this.Level = l;
			this.SText = t;
		}
		
	}
	
	
	void Awake()
	{
		int generation = 2;
		if (Screen.height<1536)
		{
			generation = 1;
		}
		
		foreach(Material m in materialsToLoad)
		{
			LoadTextures(Global.levelNeedToLoad,generation,m);	
		}
		
		foreach(SpriteText s in spritesToLoad)
		{
			LoadSpriteTexts(Global.levelNeedToLoad,generation,s);	
		}
		
		foreach(GameObject g in afterLoadActivate)
		{
			g.SetActiveRecursively(true);
		}
	}
	
	
	
	public void LoadSpriteTexts(int level,int generation,SpriteText stext)
	{
		Debug.Log("Loading spritext for level  "   + level  + "  sprite text " + stext + "generation" + generation);
		
		
		TextAsset font = Resources.Load( "Font/" +(generation.ToString()) +"/"+ stext.font.name ,typeof(TextAsset)) as TextAsset;
		stext.font = font;	
		
		Texture2D text = Resources.Load( "Font/" +(generation.ToString()) +"/"+ stext.font.name+stext.font.name,typeof(Texture2D)) as Texture2D;
		stext.renderer.sharedMaterial.mainTexture = text;
		
		
		
		if (null == unmanagedSpriteTexts)
		{
			unmanagedSpriteTexts =  new List<LevSpriteText>(0);
		}
		
		unmanagedSpriteTexts.Add(new LevSpriteText(level,stext));
		
	}
	
	
	public static void UnLoadSpriteTextsExceptForLevel(int level)
	{
		Debug.Log("Load sprite text  ecept " + level);
		if (null != unmanagedSpriteTexts && unmanagedSpriteTexts.Count>0)
		{
			List<LevSpriteText> buf = new List<LevSpriteText>();
			
			foreach(LevSpriteText ls in unmanagedSpriteTexts)
			{
				if (ls.Level!=level)
				{
					if (null != ls.SText)
					{
						Debug.Log("unload spritetext for level " + ls.Level  + "   spruitetext name " + ls.SText.name);
						Resources.UnloadAsset(ls.SText.renderer.material.mainTexture);
						Resources.UnloadAsset(ls.SText.font);
					}
					buf.Add(ls);
				}
			}
			
			foreach(LevSpriteText ls in buf)
			{
				unmanagedSpriteTexts.Remove(ls);
			}
			Debug.Log("unmanaged spritetexts " + unmanagedSpriteTexts.Count);
		}
		
	}
	
	
	public void LoadTextures(int level,int generation,Material material)
	{
		Debug.Log("Load textures for level  "   + level  + "  mater " + material);
		Texture2D text1 = Resources.Load( (generation.ToString()) +"/"+ material.name,typeof(Texture2D)) as Texture2D;
		material.mainTexture = text1;	
		
		if (null == unmanagedMaterials)
		{
			unmanagedMaterials =  new List<LevMat>(0);
		}
		
		unmanagedMaterials.Add(new LevMat(level,material));
	}
	
	
	public static void UnLoadTexturesExceptForLevel(int level)
	{
		if (null != unmanagedMaterials && unmanagedMaterials.Count>0)
		{
			List<LevMat> buf = new List<LevMat>();
			
			foreach(LevMat lm in unmanagedMaterials)
			{
				if (lm.Level!=level)
				{
					Debug.Log("Unload textues level" + lm.Level +" for material " + lm.Mater.name);
					Resources.UnloadAsset(lm.Mater.mainTexture);
					buf.Add(lm);
				}
			}
			
			foreach(LevMat ls in buf)
			{
				unmanagedMaterials.Remove(ls);
			}
			Debug.Log("unmanaged materials count " + unmanagedMaterials.Count);
		}
		
	}
}
