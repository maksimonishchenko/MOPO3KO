using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropPuzzle : DropLogic
{
	public PackedSprite[] NumberNamedElems;
	public GameObject rndPosesParent;
	private GameObject[] randomScreenPoints;
	
	public PackedSprite foregroundSprite;
	public PackedSprite backgroundSprite;
	public GameObject shadow;	
		
	private Vector3[] startPoses;
	private Vector3[] endPoses;

	
	private List<int> occpiedNumbers;
	public GameObject objectCollidersParent;
	
	public AudioSource successDropSource;
	private int counterDrop;
	public void InitGUI()
	{
		if (null != indi)
		{
			for(int i=0;i<NumberNamedElems.Length;i++)
			{
				indi.InitIndicatorMiniByName(NumberNamedElems[i].transform.name);
			}
		}

	}
	
	public override void GameStart()
	{
		if (!playing)
		{
			
			Transform[] randomPosesT = rndPosesParent.GetComponentsInChildren<Transform>();
			List<GameObject> randomPosesList = new List<GameObject>(0);
			foreach(Transform t in randomPosesT)
			{
				randomPosesList.Add(t.gameObject);
			}
			randomScreenPoints = randomPosesList.ToArray();
			
			startPoses = new Vector3[NumberNamedElems.Length];
			endPoses = new Vector3[NumberNamedElems.Length];
			occpiedNumbers = new List<int>(0);
		
			
			for(int i=0;i<NumberNamedElems.Length;i++)
			{
				startPoses[i] = NumberNamedElems[i].transform.position;
				endPoses[i] = GetRandomPlaceHolderPosition();
				NumberNamedElems[i].GetComponent<Collider>().enabled = true;
				NumberNamedElems[i].gameObject.active = true;
				messenger.SetDroppableCollider(NumberNamedElems[i].gameObject,backgroundSprite.gameObject);
				NumberNamedElems[i].transform.position = endPoses[i];
			}
			
			backgroundSprite.gameObject.active = true;
			shadow.gameObject.active = true;
			playing =true;
			messenger.SetDraggable(true);
			messenger.SetCompareToStartBeforeAcceptDrop();
			messenger.SetPickDropShift(true);
			objectCollidersParent.gameObject.SetActiveRecursively(false);
			
			counterDrop=0;
		}
	}
	
	public override void ObjDropped (string dragObjName)
	{
		if (null!=successDropSource)
		{
			successDropSource.PlayOneShot(successDropSource.clip);
		}
		int eleNumber = int.Parse(dragObjName.Substring(2,2)) -1;
		PlaceAtStart(eleNumber);
		messenger.UNsetDroppableCollider(NumberNamedElems[eleNumber].gameObject);
		counterDrop++;
		if (counterDrop == NumberNamedElems.Length)
		{
			GameFinished();
		}
	}
	
	
	protected override void GameFinished ()
	{
		
		for(int i=0;i<NumberNamedElems.Length;i++)
		{
			NumberNamedElems[i].gameObject.active =false;
		}
		
		backgroundSprite.gameObject.active = false;
		foregroundSprite.gameObject.active = true;
		
		messenger.SetDraggable(false);
		PlayerPrefs.SetInt(Global.levelNeedToLoad.ToString()+"minifinished",1);
		
		indi.InitIndicatorMiniByName(NumberNamedElems[0].transform.name);
		indi.PlayRandomMiniClipInTextManager();
		
		Invoke("AfterFinishedLogic",2f);
	}
	
	private void AfterFinishedLogic()
	{
		
		shadow.gameObject.active = false;
		foregroundSprite.gameObject.active = false;
		playing = false;
		objectCollidersParent.gameObject.SetActiveRecursively(true);
	}
	
	
	private void PlaceAtStart(int n)
	{
		NumberNamedElems[n].transform.position = new Vector3(startPoses[n].x,startPoses[n].y,startPoses[n].z + 0.5f);
	}
	
	private Vector3 GetRandomPlaceHolderPosition()
	{
		
		bool dropped = false;
		
		int c = 0;
		while(occpiedNumbers.Contains(c))
		{
			c = UnityEngine.Random.Range(0,randomScreenPoints.Length);
		}
		dropped = true;
		occpiedNumbers.Add(c);
		return randomScreenPoints[c].transform.position;
	
	}

	
	
	
	
}

