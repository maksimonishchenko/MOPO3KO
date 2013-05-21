using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropMilkGrain : DropLogic
{
 
	public PackedSprite sackSprite;
	public GameObject grain;
	
	public UIButton3D defaultGooseBut;
	public PackedSprite defaultGooseSprite;
	
	public PackedSprite newGooseSprite;
	public Collider newGooseCol;
	
	
	
	public UIButton3D defaultCatBut;
	public PackedSprite defaultCatSprite;
	
	public PackedSprite newCatSprite;
	public Collider newCatCollider;
	
	public GameObject milk;
	public Mover catMilkMover;
	
	private List<string> activated;
	private int objsDropped = 2;
	
	
	private Vector3 grainStartPos =Vector3.zero;
	private Vector3 milkStartPos = Vector3.zero;
	
	private bool[] processing;
	
	public override string[] InitGUI ()
	{
		Debug.Log("InitGUI DropMilkGrain");
		string[] indicatorMiniName = new string[objsDropped];
		
		if (null != indi)
		{
			
			indi.InitIndicatorMiniByName(grain.transform.name);
			indicatorMiniName[0] = grain.transform.name;
			
			indi.InitIndicatorMiniByName(milk.transform.name);
			indicatorMiniName[1] = milk.transform.name;
			
		}
		return indicatorMiniName;
	} 
	
	public override void GameStart()
	{
		if (!playing)
		{
			
			
			defaultGooseBut.gameObject.active = false;
			defaultGooseSprite.gameObject.active = false;
			
			newGooseSprite.gameObject.active = true;
			newGooseCol.gameObject.active = true;
			
			sackSprite.gameObject.active = true;
			
			grainStartPos = grain.transform.position;
			messenger.SetDroppableCollider(grain,newGooseCol.gameObject);
			
			defaultCatBut.gameObject.active = false;
			
			newCatSprite.gameObject.active = true;
			newCatCollider.gameObject.active = true;
			
			milk.gameObject.active = true;
			milk.renderer.enabled = true;
			
			milkStartPos = milk.gameObject.transform.position;
			
			messenger.SetDroppableCollider(milk,newCatCollider.gameObject);
			
			
			processing = new bool[objsDropped];
			activated = new List<string>(objsDropped);
			messenger.SetDraggable(true);
			playing =true;
		}
	}
	
	public void CheckFinishedCondition(string objName)
	{
		if (!activated.Contains(objName))
		{
			activated.Add(objName);
		}
		
		if (activated.Count == objsDropped && !processing[0] && !processing[1])
		{
			GameFinished();
		}
	}
	
	public override void ObjDropped (string dragObjName)
	{
		
		if(dragObjName == grain.transform.name)
		{
			grain.collider.enabled = false;	
			grain.transform.position = grainStartPos;
			grain.renderer.enabled = false;
			processing[0] = true;
			if (PackedFramesActivator.AddActivatingRunningSprites(newGooseSprite))
			{
				PackedFramesActivator.SetOnSelfRevertAnimationAcition(newGooseSprite.name,() => 
				{
					if (null != indi)
					{
						indi.MiniGameCompleteCallback(grain.name,"default");
					}	
					processing[0] = false;
					grain.collider.enabled = true;
					CheckFinishedCondition(dragObjName);
				});	
			}
			
			PackedFramesActivator.GetButtonCallback(newGooseSprite.name)();
		}
		
		if(dragObjName == milk.transform.name)
		{
			defaultCatSprite.gameObject.active = false;	
			milk.transform.position = milkStartPos;
			milk.collider.enabled = false;
			processing[1] = true;
			
			if (PackedFramesActivator.AddActivatingRunningSprites(newCatSprite))
			{
				PackedFramesActivator.SetOnSelfRevertAnimationAcition(newCatSprite.name,() => 
				{
					defaultCatSprite.gameObject.active = true;	
					if (null != indi)
					{
						indi.MiniGameCompleteCallback(milk.name,"default");
					}	
					processing[1] = false;
					milk.collider.enabled = true;
					CheckFinishedCondition(dragObjName);
				});	
			}
			
			catMilkMover.StartMoving();
		}
	}
	
	
	
	protected override void GameFinished ()
	{
		messenger.SetDraggable(false);
		PlayerPrefs.SetInt(Global.levelNeedToLoad.ToString()+"minifinished",1);
		indi.InitIndicatorMiniByName(grain.transform.name);
		indi.PlayRandomMiniClipInTextManager();
		playing = false;
		activated = null;
		
		
		defaultGooseBut.gameObject.active = true;
		defaultGooseSprite.gameObject.active = true;
		
		newGooseSprite.gameObject.active = false;
		newGooseCol.gameObject.active = false;
		
		sackSprite.gameObject.active = false;
		
		grain.transform.position = grainStartPos;
		grain.renderer.enabled = false;
		
		defaultCatBut.gameObject.active = true;
		
		newCatSprite.gameObject.active = false;
		newCatCollider.gameObject.active = false;
		
		milk.gameObject.active = false;
		milk.renderer.enabled = false;
		
		milk.gameObject.transform.position = milkStartPos;
	
		
		
		
	}
	
	
}


