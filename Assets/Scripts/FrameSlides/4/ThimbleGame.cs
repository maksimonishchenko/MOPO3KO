using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThimbleGame : MiniGame
{
 
	
	public PackedSprite[] thimbles;
	public PackedSprite coin;
	public AudioSource openSoundEffect;
		
	private Vector3[] positions;
	private float[] showSignals;
	private Queue shiftSignalsQueue;
	private ShiftInfo currentShift;
	
	private class ShiftInfo
	{
		public Vector3[] startPos;
		public GameObject[] shiftGO;
		public Vector3[] finalPos;
		public bool clockWise;
		
		public ShiftInfo(Vector3[] startP,Vector3[] finalP,GameObject[] gos)
		{
			this.startPos = startP;
			this.shiftGO  = gos;
			this.finalPos = finalP;
			this.clockWise = UnityEngine.Random.Range(0f,1f)>0.5f ? true : false;
		}
		
	}
	
	private int numberOfShifts= 13;
	
	private bool[] processing;
	
	
	private class ShiftSignal
	{
		public int x1;
		public int x2;
		

		
		public ShiftSignal(int inX1,int inX2)
		{
			this.x1 = inX1;
			this.x2 = inX2;
			
		}
	}
	
	public override string[] InitGUI ()
	{
		string[] indicatorMiniName = new string[1];
		
		if (null != indi)
		{
			
			indi.InitIndicatorMiniByName(coin.transform.name);
			indicatorMiniName[0] = coin.transform.name;
			
		}
		return indicatorMiniName;
	} 
	
	public override void GameStart()
	{
		
		if (!playing)
		{
			playing = true;
			
			processing = new bool[thimbles.Length];
			
			
			//init and turn on
			positions = new Vector3[thimbles.Length];
			showSignals = new float[]{-1f,-1f,-1f};
			for(int i =0;i<thimbles.Length;i++)
			{
				thimbles[i].gameObject.active = true;
				positions[i] = thimbles[i].gameObject.transform.position;
			}
			coin.gameObject.SetActiveRecursively(true);
			
			//place coin
			coin.transform.parent = thimbles[UnityEngine.Random.Range(0,thimbles.Length)].gameObject.transform;
			coin.transform.localPosition = new Vector3(25f,-75f,1f);
			
			//play animations
			int counter=0;
			
			for(int j =0;j<thimbles.Length;j++)
			{
				if (PackedFramesActivator.AddActivatingRunningSprites(thimbles[j]))
				{
					PackedFramesActivator.SetOnSelfRevertAnimationAcition(thimbles[j].gameObject.name,() => 
					{
						counter++;
						if (counter==thimbles.Length)
						{
							ClearMiniAnimationCallbacks();
							GenerateShiftSignals(numberOfShifts);
						}
					});	
				}
				showSignals[j] = (float) j+0.1f;
			}
			
			
			
			//0 place coin
			//1 play thimbles animations
			//2 shuffle
			//3 wait for input -> uncover by click when uncover with coin GameFinished
		}
	}
	
	private void ClearMiniAnimationCallbacks()
	{
		for(int i =0;i<thimbles.Length;i++)
		{
			PackedSprite shallowSprite = thimbles[i];
			PackedFramesActivator.CleanSelfRevertAnimationAcition(shallowSprite.name);
			PackedFramesActivator.RemoveActivatingRunningSprites(shallowSprite);
			shallowSprite.GetComponent<UIButton3D>().SetInputDelegate((ref POINTER_INFO p ) => {});
		}
		
	}
	
	
	void Update()
	{
		if (null != showSignals)
		{
			for(int i=0;i<showSignals.Length;i++)
			{
				if (showSignals[i]>0)
				{
					showSignals[i]-=Time.deltaTime;
					if (showSignals[i]<0)
					{
						PackedFramesActivator.GetButtonCallback(thimbles[i].gameObject.name)();
					}
				}
			}	
		}
		if (null!=shiftSignalsQueue && shiftSignalsQueue.Count>0)
		{
			if (null == currentShift)
			{
				currentShift = GenerateShiftInfo(shiftSignalsQueue.Peek() as ShiftSignal);
			}
			if (!ProcessShiftInfo(Time.deltaTime,currentShift,(float) numberOfShifts))
			{
				currentShift = null;
				shiftSignalsQueue.Dequeue();
				if (shiftSignalsQueue.Count==0)
				{
					bool openedUpRightOne = false;
					
					for(int j =0;j<thimbles.Length;j++)
					{
						PackedSprite shallowSprite = thimbles[j];
						int shallowJ = j;
						if (PackedFramesActivator.AddActivatingRunningSprites(shallowSprite))
						{
							PackedFramesActivator.SetOnSelfRevertAnimationAcition(shallowSprite.gameObject.name,() => 
							{
								processing[shallowJ] = false;
								if (shallowSprite.transform.GetChildCount()==1)
								{
									openedUpRightOne = true;	
								}
								if (openedUpRightOne && IsNotProcessingAnimation())
								{
									openedUpRightOne = false;
									ClearMiniAnimationCallbacks();
									GameFinished();		
								}
							});	
						}
					}
					for(int k =0;k<thimbles.Length;k++)
					{
						int shallowK = k;
						UIButton3D shallowButton = thimbles[k].GetComponent<UIButton3D>();
						shallowButton.SetInputDelegate((ref POINTER_INFO p) =>
						{
							if (p.evt == POINTER_INFO.INPUT_EVENT.TAP)
							{
								processing[shallowK] = true;
								if (null != openSoundEffect)
								{
									openSoundEffect.PlayOneShot(openSoundEffect.clip);
								}
								PackedFramesActivator.GetButtonCallback(shallowButton.gameObject.name)();
							}
						});
					}
				}
			}
		}
	}
	
	private bool IsNotProcessingAnimation()
	{
		bool isNotProcessingAnimation = true;
		foreach(bool b in processing)
		{
			if (b) 
			{
				isNotProcessingAnimation = false;
			}
		}
		return isNotProcessingAnimation;					
	}

	private bool ProcessShiftInfo(float deltaTime,ShiftInfo shiftI,float left)
	{
		bool shifted = false;
		float shiftAbsoluteError = 3f;
		
		for(int i=0;i<shiftI.shiftGO.Length;i++)
		{
			if (Vector3.Distance(shiftI.finalPos[i],shiftI.shiftGO[i].transform.position)>shiftAbsoluteError)
			{
				shifted = true;	
				shiftI.shiftGO[i].transform.position = Vector3.Lerp(shiftI.shiftGO[i].transform.position,shiftI.finalPos[i],deltaTime*(30f/left));
			}
			else
			{
				shiftI.shiftGO[i].transform.position = shiftI.finalPos[i];
			}
		}
		return shifted;
	}
	
	private ShiftInfo GenerateShiftInfo(ShiftSignal signal)
	{
		
		GameObject[] shiftPair = new GameObject[2];	
		
		shiftPair[0] = thimbles[signal.x1].gameObject;
		shiftPair[1] = thimbles[signal.x2].gameObject;
		
				
		Vector3[] shiftStartPoints  = new Vector3[2];
		
		shiftStartPoints[0] = shiftPair[0].gameObject.transform.position;
		shiftStartPoints[1] = shiftPair[1].gameObject.transform.position;
		
		Vector3[] shiftFinalPoints  = new Vector3[2];
		
		shiftFinalPoints[0] = shiftPair[1].gameObject.transform.position;
		shiftFinalPoints[1] = shiftPair[0].gameObject.transform.position;
		
		ShiftInfo newShiftInfo = new ShiftInfo(shiftStartPoints,shiftFinalPoints,shiftPair);
		return newShiftInfo;
		
		
	}

	
	private void GenerateShiftSignals(int num)
	{
		shiftSignalsQueue = new Queue();
		
		for(int i=0;i<num;i++)
		{
			int first = UnityEngine.Random.Range(0,3);
			int second;
			if (first==0) second = UnityEngine.Random.Range(1,3);
			else if (first==2) second = UnityEngine.Random.Range(0,2);
			else second = (UnityEngine.Random.Range(0f,1f)>0.5f)? 0 : 2; 
			
			shiftSignalsQueue.Enqueue(new ShiftSignal(first,second));
		}
		
	}
	


	
	
	protected override void GameFinished ()
	{
		PlayerPrefs.SetInt(Global.levelNeedToLoad.ToString()+"minifinished",1);
		indi.InitIndicatorMiniByName(coin.transform.name);
		indi.PlayRandomMiniClipInTextManager();
		playing = false;
		
		coin.transform.parent = gameObject.transform;
		
		coin.gameObject.SetActiveRecursively(false);
		
		foreach(PackedSprite ps in thimbles)
		{
			ps.gameObject.active = false;
		}
		shiftSignalsQueue = null;
		currentShift = null;
		
	}
	
	
	
	
}



