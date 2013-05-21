using UnityEngine;
using System.Collections;

public class SceneNavigator : MonoBehaviour 
{
	public UIButton3D nextSceneButton;
	public UIButton3D prevSceneButton;
	
	public PackedSprite nextSceneSprite;
	public PackedSprite prevSceneSprite;
	
	private bool block = false;
	
	public AwakeTextureLoad textureLoad;
	
	void Start()
	{
		if (null != nextSceneButton)
		{
			nextSceneButton.scriptWithMethodToInvoke = this;
			nextSceneButton.methodToInvoke = "Next";
		}
		
		if (null != prevSceneButton)
		{
			prevSceneButton.scriptWithMethodToInvoke = this;
			prevSceneButton.methodToInvoke = "Prev";
		}
		
		if (null != nextSceneSprite )
		{
			
		}
		
		if (null != prevSceneSprite && Global.levelNeedToLoad == 1)
		{
			prevSceneSprite.Setup(0f,0f);
			(prevSceneButton.collider as BoxCollider).size = Vector3.zero;
		}
		
	}
	
	

	
	
	
	void Next()
	{
		if (block)
		{
			return;
		}
		block = true;
		PackedFramesActivator.Clear();
		Global.levelNeedToLoad+=1;
		Debug.Log("level Need to Load " + Global.levelNeedToLoad);
		Application.LoadLevel(0);
	}
	
	
	void Prev()
	{
		if (block)
		{
			return;
		}
		block = true;
		PackedFramesActivator.Clear();
		Global.levelNeedToLoad-=1;
		Application.LoadLevel(0);
	}
}
