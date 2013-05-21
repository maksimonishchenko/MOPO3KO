using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BranchesGame : DropLogic
{
 
	
	public PackedSprite fireAnimation;
	public PackedSprite[] branches; 
	
	private Vector3[] branchStartPositions;
	private int droppedCounter;
	
	public AudioSource branchSource;
	
	public override string[] InitGUI ()
	{
		string[] indicatorMiniName = new string[1];
		
		if (null != indi)
		{
			
			indi.InitIndicatorMiniByName(fireAnimation.transform.name);
			indicatorMiniName[0] = fireAnimation.transform.name;

		}
		return indicatorMiniName;
	} 
	
	public override void GameStart()
	{
		if (!playing)
		{
			
			branchStartPositions = new Vector3[branches.Length];
			for(int i=0;i<branches.Length;i++)
			{
				branchStartPositions[i] = branches[i].transform.position;
				PackedSprite ps = branches[i];
				ps.gameObject.active = true;
				messenger.SetDroppableCollider(ps.gameObject,fireAnimation.gameObject);
			}
			droppedCounter = 0;
			messenger.SetDraggable(true);
			playing =true;
			
		}
	}

	
	private void ClearMiniAnimationCallbacks(PackedSprite ps)
	{
		
		PackedSprite shallowSprite = ps;
		PackedFramesActivator.CleanSelfRevertAnimationAcition(shallowSprite.name);
		PackedFramesActivator.RemoveActivatingRunningSprites(shallowSprite);
		UIButton3D button = shallowSprite.GetComponent<UIButton3D>();
		if (null != button)
		{
			button.SetInputDelegate((ref POINTER_INFO p ) => {});
		}
	}
	
	public override void ObjDropped (string dragObjName)
	{
		
		
		
		for(int i=0;i<branches.Length;i++)
		{
			PackedSprite ps =  branches[i];
			if(dragObjName == ps.transform.name)
			{
				if (null!=branchSource)
				{
					branchSource.PlayOneShot(branchSource.clip);
				}
				ClearMiniAnimationCallbacks(ps);
				ps.gameObject.transform.position = branchStartPositions[i];
				ps.gameObject.active = false;
				
				if (PackedFramesActivator.AddActivatingRunningSprites(fireAnimation))
				{
					PackedFramesActivator.SetOnSelfRevertAnimationAcition(fireAnimation.gameObject.name,() => 
					{
						droppedCounter++;
						if (droppedCounter==branches.Length)
						{
							GameFinished();
						}
						
					});	
				}
				PackedFramesActivator.GetButtonCallback(fireAnimation.gameObject.name)();
			}
		}
		
		
	}
	
	
	
	protected override void GameFinished ()
	{
		messenger.SetDraggable(false);
		PlayerPrefs.SetInt(Global.levelNeedToLoad.ToString()+"minifinished",1);
		
		indi.InitIndicatorMiniByName(fireAnimation.transform.name);
		
		indi.PlayRandomMiniClipInTextManager();
		playing = false;
		
		
		
	
		
		
		
	}
	
	
}





