using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropKnifeMilk : DropLogic
{
 
	
	//drag object
	public UIButton3D knifeBut;
	//drop object
	public Collider breadCollider;
	
	//switches
	public PackedSprite defaultBreadSprite;
	public PackedSprite newBreadSprite;
	public PackedSprite milkSprite;

	//drag
	public UIButton3D vaseBut;
	//drop
	public Collider cupCollider;
	//switches
	public PackedSprite milkAnimation;
	
	//
	public AudioSource breadKnifeSound;
	private Vector3 knifeStartPos = Vector3.zero;
	private Vector3 vaseStartPos  = Vector3.zero;
	
	private bool[] processing;
	private List<string> activated;
	private int objsDropped = 2;
	
	public override string[] InitGUI ()
	{
		Debug.Log("InitGUI DropMilkGrain");
		string[] indicatorMiniName = new string[objsDropped];
		
		if (null != indi)
		{
			
			indi.InitIndicatorMiniByName(knifeBut.transform.name);
			indicatorMiniName[0] = knifeBut.transform.name;
			
			indi.InitIndicatorMiniByName(vaseBut.transform.name);
			indicatorMiniName[1] = vaseBut.transform.name;
			
		}
		return indicatorMiniName;
	} 
	
	public override void GameStart()
	{
		if (!playing)
		{
			
			
			knifeBut.gameObject.active = true;
			knifeBut.collider.enabled = true;
			knifeBut.renderer.enabled = true;
			
			knifeStartPos = knifeBut.transform.position;
			messenger.SetDroppableCollider(knifeBut.gameObject,breadCollider.gameObject);
			
			vaseBut.gameObject.active =true;
			vaseStartPos = vaseBut.transform.position;
			messenger.SetDroppableCollider(vaseBut.gameObject,cupCollider.gameObject);
			
			defaultBreadSprite.renderer.enabled = true;
			newBreadSprite.renderer.enabled  = false;
			milkSprite.renderer.enabled = false;
			vaseBut.collider.enabled = true;
			
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
		
		if (activated.Count == objsDropped)
		{
			GameFinished();
		}

	}
	
	public override void ObjDropped (string dragObjName)
	{
		
		if(dragObjName ==  knifeBut.transform.name)
		{
			knifeBut.collider.enabled = false;	
			knifeBut.transform.position = knifeStartPos;
			knifeBut.renderer.enabled = false;
			
			defaultBreadSprite.renderer.enabled = false;
			newBreadSprite.renderer.enabled = true;
			
			if (null != breadKnifeSound)
			{
				breadKnifeSound.PlayOneShot(breadKnifeSound.clip);	
			}
			
			CheckFinishedCondition(dragObjName);
			
		}
		
		if(dragObjName == vaseBut.transform.name)
		{
			
			
			vaseBut.transform.position = vaseStartPos;
			vaseBut.collider.enabled = false;
			vaseBut.renderer.enabled = false;
			
			if (PackedFramesActivator.AddActivatingRunningSprites(milkAnimation))
			{
				Debug.Log("addactiv");
				PackedFramesActivator.SetOnSelfRevertAnimationAcition(milkAnimation.gameObject.name,() => 
				{
					if (null != indi)
					{
						indi.MiniGameCompleteCallback(milkAnimation.name,"default");
					}	
					vaseBut.renderer.enabled = true;
					CheckFinishedCondition(dragObjName);
				});	
			}
			PackedFramesActivator.GetButtonCallback(milkAnimation.gameObject.name)();
		}
	}
	
	
	
	protected override void GameFinished ()
	{
		messenger.SetDraggable(false);
		PlayerPrefs.SetInt(Global.levelNeedToLoad.ToString()+"minifinished",1);
		
		indi.InitIndicatorMiniByName(knifeBut.transform.name);
		
		indi.PlayRandomMiniClipInTextManager();
		playing = false;
		activated = null;
		
		
		
		knifeBut.gameObject.active = false;
		knifeBut.transform.position= knifeStartPos;
		
			
		vaseBut.gameObject.active =false;
		vaseBut.transform.position = vaseStartPos;

	
		
		
		
	}
	
	
}


