using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WordGame : DropLogic
{
	public GameObject[] alphabetPrefabs;
	public GameObject cubesParent;
	public PackedSprite boxSprite;

	public GameObject cubePlaces3Parent;
	public GameObject cubePlaces4Parent;
	
	public string[] words;
	public GameObject shadow;
	public GameObject objectCollidersParent;
	public GameObject rndPosesWordsParent;
	
	public AudioSource rightPlacingsSource;
	public AudioSource placeOneSource;
	private PackedSprite[] wordsSprites;
	private PackedSprite[] cubeSprites;
	private Vector3[] cubePlaces;
	private int counterDrop;
	private Vector3[] startPoses;
	private int wordNumber=-1;
	private GameObject[] randomScreenPoints;
	private List<int> occpiedNumbers;
	
	public void InitGUI()
	{
		if (null != indi)
		{
			indi.InitIndicatorMiniByName(boxSprite.gameObject.transform.name);
		}
	}
	
	private int PickNewWordNumber(int oldNumber)
	{
		int newNumber = oldNumber;	
		int attepmts = 0;
		int maxAttempts = 100;
		while(newNumber==oldNumber && attepmts<maxAttempts)
		{
			newNumber = UnityEngine.Random.Range(0,words.Length);
			attepmts++;
		}

		return newNumber;
	}
	
	public override void GameStart()
	{
		if (!playing)
		{
			wordNumber = PickNewWordNumber(wordNumber);
			List<int> alphabetNumbers = ConvertStringToAlphabetNumbers(words[wordNumber]);
			List<PackedSprite> alphabetsList = new List<PackedSprite>(0);
			
			occpiedNumbers = new List<int>(0);
			Transform[] randomPosesWordsParentT = rndPosesWordsParent.GetComponentsInChildren<Transform>();
			List<GameObject> randomPosesWordsParentList = new List<GameObject>(0);
			foreach(Transform t in randomPosesWordsParentT)
			{
				if (t.name.StartsWith("WORDPOS"))
				{
					randomPosesWordsParentList.Add(t.gameObject);	
				}
			}
			randomScreenPoints = randomPosesWordsParentList.ToArray();
			
			
			foreach(int number in alphabetNumbers)
			{
				GameObject charPrefab = Instantiate(alphabetPrefabs[number],GetRandomPlaceHolderPosition(),Quaternion.identity) as GameObject;
				charPrefab.name = "ch"+number.ToString()+"_"+Time.realtimeSinceStartup;
				alphabetsList.Add(charPrefab.GetComponent<PackedSprite>());	
			}
			wordsSprites = alphabetsList.ToArray();
			
			cubeSprites = cubesParent.GetComponentsInChildren<PackedSprite>();
			
			
			Transform[] cubePlacesT = words[wordNumber].Length == 4 ? cubePlaces4Parent.GetComponentsInChildren<Transform>() : cubePlaces3Parent.GetComponentsInChildren<Transform>();
			List<Vector3> cubePlacesList = new List<Vector3>(0);
			for(int k=0;k<cubePlacesT.Length;k++)
			{
				if (cubePlacesT[k].name.StartsWith("P"))
				{
					cubePlacesList.Add(cubePlacesT[k].transform.position);	
				}
			}
			cubePlaces = cubePlacesList.ToArray();
			
			
			startPoses = new Vector3[wordsSprites.Length];
			for(int l=0;l<wordsSprites.Length;l++)
			{
				cubeSprites[l].gameObject.collider.enabled = true;
				cubeSprites[l].renderer.enabled = true;
				cubeSprites[l].transform.position = cubePlaces[l];
				startPoses[l] = wordsSprites[l].transform.position;
				int[] allowedDropsNums = GetGONumsByCharInName(wordsSprites,ExtractChNumFromName(wordsSprites[l].gameObject)); 
				messenger.SetDroppableCollider(wordsSprites[l].gameObject,GetGOByIndNum(cubeSprites,allowedDropsNums));
			}
			
			boxSprite.gameObject.active = true;
			shadow.gameObject.active = true;
			playing =true;
			messenger.SetAllowDropsOneTime(true);
			messenger.SetDraggable(true);
			messenger.SetPickDropShift(true);
			objectCollidersParent.gameObject.SetActiveRecursively(false);
			
			counterDrop=0;
		}
	}
	
	private GameObject[] GetGOByIndNum(PackedSprite[] psprites,int[] nums)
	{
		List<GameObject> gos = new List<GameObject>(0);
		foreach(int idx in nums)
		{
			gos.Add(psprites[idx].gameObject);
		}
		return gos.ToArray();
		
	}
	private int[] GetGONumsByCharInName(PackedSprite[] gos,int charNum)
	{
		List<int> filtered = new List<int>(0);
		for(int i=0;i<gos.Length;i++)
		{
			if (gos[i].gameObject.name.Contains("ch"+charNum))
			{
				filtered.Add(i);
			}
		}
		return filtered.ToArray();
	}
	
	private int ExtractChNumFromName(GameObject go)
	{
		int indexOfPrefix = go.name.IndexOf("ch")+2;
		int indexOfPostfix = go.name.IndexOf("_");
		
		string numSubstring = go.name.Substring(indexOfPrefix,indexOfPostfix - indexOfPrefix);
		int num = int.Parse(numSubstring);
		return num;
	}
	
	
	private List<int> ConvertStringToAlphabetNumbers(string word)
	{
		List<int> listInt = new List<int>(0);
		foreach(char c in word)
		{
			listInt.Add(((int) c) - 1072);
			Debug.Log("char " + c + " int " + (int) c);	//cyriccli small 1072 - 1103
		}
		return listInt;
	}
	
	public override void ObjDropped (string dragObjName)
	{
		for(int h=0;h<wordsSprites.Length;h++)
		{
			if (wordsSprites[h].gameObject.name == dragObjName)
			{
				wordsSprites[h].gameObject.collider.enabled = false;
				Vector3 hitPos = messenger.GetLastHitPanelScreenPos();
				wordsSprites[h].gameObject.transform.position = new Vector3(hitPos.x,hitPos.y,hitPos.z-1f);
			}
			
		}
		
		counterDrop++;
		if (counterDrop == wordsSprites.Length)
		{
			if (null != placeOneSource)
			{
				placeOneSource.PlayOneShot(placeOneSource.clip);	
			}
			GameFinished();
		}	
		else
		{
			if (null != rightPlacingsSource)
			{
				rightPlacingsSource.PlayOneShot(rightPlacingsSource.clip);	
			}
		}
		
	}
	
	
	protected override void GameFinished ()
	{
		messenger.ResetOneTimers();
		messenger.SetDraggable(false);
		PlayerPrefs.SetInt(Global.levelNeedToLoad.ToString()+"minifinished",1);
		
		indi.InitIndicatorMiniByName(boxSprite.gameObject.transform.name);
		indi.PlayRandomMiniClipInTextManager();
		
		Invoke("AfterFinishedLogic",2f);
		
	}
	
	private void AfterFinishedLogic()
	{
		
		shadow.gameObject.active = false;
		boxSprite.gameObject.active =false;
		
		foreach(PackedSprite pc in cubeSprites)
		{
			pc.collider.enabled = false;
			pc.renderer.enabled = false;
		}
		
		foreach(PackedSprite ps in wordsSprites)
		{
			Destroy (ps.gameObject);
		}
		
		playing = false;
		objectCollidersParent.gameObject.SetActiveRecursively(true);
		
	}
	
	
	private Vector3 GetRandomPlaceHolderPosition()
	{
		
		int c = 0;
		while(occpiedNumbers.Contains(c))
		{
			c = UnityEngine.Random.Range(0,randomScreenPoints.Length);
		}
		occpiedNumbers.Add(c);
		return randomScreenPoints[c].transform.position;
	
	}
	
	
	
	
	
	
}


