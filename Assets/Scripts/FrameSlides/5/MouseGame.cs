using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseGame : MiniGame
{
 
	public PackedSprite mouseAppear;
	public PackedSprite mouseHitted;
	public GameObject rndPosesParent;
	
	public AudioSource stopPlayingSource;
	
	private float pauseCounter = 0f;
	private float appearPeriod = 2f;
	
	private GameObject[] randomPoses;
	
	public override string[] InitGUI ()
	{
		string[] indicatorMiniName = new string[1];
		
		if (null != indi)
		{
			
			indi.InitIndicatorMiniByName(mouseAppear.transform.name);
			indicatorMiniName[0] = mouseAppear.transform.name;
			
		}
		return indicatorMiniName;
	} 
	
	public override void GameStart()
	{
		
		if (!playing)
		{
			playing = true;
			Transform[] randomPosesT = rndPosesParent.GetComponentsInChildren<Transform>();
			List<GameObject> randomPosesList = new List<GameObject>(0);
			foreach(Transform t in randomPosesT)
			{
				randomPosesList.Add(t.gameObject);
			}
			randomPoses = randomPosesList.ToArray();
			
			pauseCounter = appearPeriod;
		}
	}
	
	
	
	
	void Update()
	{
		if (playing)
		{
			if (pauseCounter>0f)
			{
				pauseCounter-=Time.deltaTime;
				if (pauseCounter<0f)
				{
					ShowMouse();
				}	
			}
			
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
	
	
	private void ShowMouse()
	{
		GameObject resp = GetNewRndRespGO();
		mouseAppear.transform.position = resp.transform.position;
		mouseAppear.transform.rotation = resp.transform.rotation;
		
		mouseAppear.GetComponent<UIButton3D>().SetInputDelegate((ref POINTER_INFO p ) => 
		{
			if (p.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				mouseAppear.renderer.enabled = false;	
				ClearMiniAnimationCallbacks(mouseAppear);
				
				
				if (PackedFramesActivator.AddActivatingRunningSprites(mouseHitted))
				{
					if (null!=stopPlayingSource)
					{
						stopPlayingSource.Stop();	
					}
					PackedFramesActivator.SetOnSelfRevertAnimationAcition(mouseHitted.gameObject.name,() => 
					{	
						GameFinished();
					});	
					PackedFramesActivator.GetButtonCallback(mouseHitted.gameObject.transform.name)();
				}	
			}
			
		});
		
		if (PackedFramesActivator.AddActivatingRunningSprites(mouseAppear))
		{
			PackedFramesActivator.SetOnSelfRevertAnimationAcition(mouseAppear.gameObject.name,() => 
			{
				ClearMiniAnimationCallbacks(mouseHitted);
				ClearMiniAnimationCallbacks(mouseAppear);
				pauseCounter = appearPeriod;
			});	
			PackedFramesActivator.GetButtonCallback(mouseAppear.gameObject.transform.name)();
		}
		
		
		
	}
	
	private GameObject GetNewRndRespGO()
	{
		Vector3 newPos = mouseAppear.gameObject.transform.position;
		int c = UnityEngine.Random.Range(0,randomPoses.Length-1);
		while(mouseAppear.gameObject.transform.position==randomPoses[c].transform.position)
		{
			c = UnityEngine.Random.Range(0,randomPoses.Length-1);
		}
		
		return randomPoses[c].gameObject;
	}
	
	protected override void GameFinished ()
	{
		ClearMiniAnimationCallbacks(mouseHitted);
		PlayerPrefs.SetInt(Global.levelNeedToLoad.ToString()+"minifinished",1);
		indi.InitIndicatorMiniByName(mouseAppear.transform.name);
		indi.PlayRandomMiniClipInTextManager();
		playing = false;
		
		
		
	}
	
	
	
	
}



