using UnityEngine;
using System.Collections;

public class StartFromStart : MonoBehaviour 
{
	public int animID;
	public int frameNum;
	public SpriteBase playFromStart;
	
	
	void Start()
	{
		if (playFromStart!=null)
					{
						Debug.Log("playanim "  + playFromStart.gameObject.name);
						playFromStart.gameObject.active=true;
						playFromStart.renderer.enabled = true;
						//playFromStart.PlayAnim(anim.animID);
						if (playFromStart is AutoSpriteBase)
						{
							AutoSpriteBase autoSB = (playFromStart as AutoSpriteBase);
							
							if (null != autoSB.animations)
							{
								autoSB.PlayAnim(animID,frameNum);
							}
							else
							{
								autoSB.renderer.enabled = true;
							}
							//(playFromStart as AutoSpriteBase).PlayAnim(anim.animID,anim.spriteBaseStartPlayingFrame);
						}
						else
						{
							playFromStart.PlayAnim(animID);
						}
					
					}
		
		
	}
	
}
