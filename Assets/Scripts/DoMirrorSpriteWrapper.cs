using UnityEngine;
using System.Collections;

public class DoMirrorSpriteWrapper : MonoBehaviour {
	
	public PackedSprite[] ps;
	
	public void DoMirror()
	{
		foreach(PackedSprite sprite in ps)
		{
			sprite.doMirror = !sprite.doMirror;
			sprite.UpdateUVs();	
		}
		
	}
}
