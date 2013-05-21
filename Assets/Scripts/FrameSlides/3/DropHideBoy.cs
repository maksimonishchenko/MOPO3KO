using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropHideBoy : DropLogic
{
 
	
	//drag object
	public UIButton3D boyButton;
	//drop object
	public Collider oldManCollider;
	
	//switches
	public GameObject rndPosesParent;

	public AudioSource dropBoy;
	
	private Vector3 boyStartPos = Vector3.zero;
	private int objsDropped = 1;
	private List<int> numsOccupied;
	GameObject[] randomPoses;
		
	public PackedSprite blockedSprite;
	public Collider blockedCol1;
	public Collider blockedCol2;
	
	public override string[] InitGUI ()
	{
		Debug.Log("InitGUI DropMilkGrain");
		string[] indicatorMiniName = new string[objsDropped];
		
		if (null != indi)
		{
			
			indi.InitIndicatorMiniByName(boyButton.transform.name);
			indicatorMiniName[0] = boyButton.transform.name;
			
			
		}
		return indicatorMiniName;
	} 
	
	public override void GameStart()
	{
		if (!playing)
		{
			
			blockedSprite.renderer.enabled = false;
			blockedCol1.enabled = false;
			blockedCol2.enabled = false;
			
			numsOccupied =  new List<int>(0);
			Transform[] randomPosesT = rndPosesParent.GetComponentsInChildren<Transform>();
			List<GameObject> randomPosesList = new List<GameObject>(0);
			foreach(Transform t in randomPosesT)
			{
				if (t.name.StartsWith("HIDE"))
				{
					randomPosesList.Add(t.gameObject);	
				}
			}
			randomPoses = randomPosesList.ToArray();
			
			
			oldManCollider.enabled = true;
			boyButton.collider.enabled = true;	
			boyButton.renderer.enabled = true;
			boyStartPos = boyButton.transform.position;
			messenger.SetDroppableCollider(boyButton.gameObject,oldManCollider.gameObject);
			boyButton.transform.position = GetRandomPlaceHolderPosition();
			
			messenger.SetDraggable(true);
			playing = true;
		}
	}
	

	
	public override void ObjDropped (string dragObjName)
	{
		
		if(dragObjName ==  boyButton.transform.name)
		{
			boyButton.collider.enabled = false;	
			boyButton.transform.position = boyStartPos;
			if(null!=dropBoy)
			{
				dropBoy.PlayOneShot(dropBoy.clip);
			}
			GameFinished();
			
		}
		
		
	}
	
	
	
	protected override void GameFinished ()
	{
		messenger.SetDraggable(false);
		PlayerPrefs.SetInt(Global.levelNeedToLoad.ToString()+"minifinished",1);
		
		indi.InitIndicatorMiniByName(boyButton.transform.name);
		
		indi.PlayRandomMiniClipInTextManager();
		
		blockedSprite.renderer.enabled = true;
		boyButton.renderer.enabled = false;
		
		
		
		Invoke("DelayedOn",2f);
	}
	
	private void DelayedOn()
	{
		blockedCol1.enabled = true;
		blockedCol2.enabled = true;
		
		oldManCollider.enabled = false;
		boyButton.collider.enabled = false;
		blockedSprite.renderer.enabled = true;
		playing = false;
	}
	
	private Vector3 GetRandomPlaceHolderPosition()
	{
		
		
		
		int c = UnityEngine.Random.Range(0,randomPoses.Length-1);
		while(numsOccupied.Contains(c))
		{
			c = UnityEngine.Random.Range(0,randomPoses.Length-1);
		}
		
		numsOccupied.Add(c);
		
		return randomPoses[c].transform.position;
		
		
	}
	
	
}


