using UnityEngine;
using System.Collections;

public class TextureOffsetWrapper : MonoBehaviour 
{

	public PackedSprite[] ps;
	
	public void MiddleLeft()
	{
		foreach(PackedSprite sprite in ps)
		{
			sprite.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
			sprite.UpdateUVs();	
		}
		
	}
	
	public void TextureOffset()
	{
		foreach(PackedSprite sprite in ps)
		{
			sprite.anchor  = SpriteRoot.ANCHOR_METHOD.TEXTURE_OFFSET;
			sprite.UpdateUVs();	
		}
		
	}
	
}
