using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour 
{
	
	public bool revertPositionOnEnd = false;
	public bool oneTimeRunner = false;
	public GameObject moveGO;

	Vector3 startPosition = Vector3.zero;

	
	public GameObject[] points;

	public float[] timeToMovesMoveOne;
	public float[] pauses;
	public DrivenAnimation[] drivenAnimsIntervals;
	public DrivenAnimation[] drivenAnimsPauses;
	
	
	private bool[] eventsIntFlags;
	private bool[] eventsPausFlags;
	
	private int wayPointsIterator = -1;
	private float stepMoveTime = 0f;
	private float pauseTime = 0f;
	private bool stopExecuteFlag = false;
	
	private bool earlyOut = false;
	void Start()
	{
		//Application.targetFrameRate  =10;
		if (points.Length!=timeToMovesMoveOne.Length || timeToMovesMoveOne.Length!=pauses.Length)
		{
			Debug.LogError("Mover Component on " + gameObject.name + " have incorrect values ");
			Destroy(this);
		}
		startPosition = moveGO.transform.position;
	}
	
	public void StartMoving()
	{
		if (wayPointsIterator==-1)
		{
			wayPointsIterator=0;
			stepMoveTime = 0f;
			pauseTime = 0f;		
			eventsIntFlags = new bool[drivenAnimsIntervals.Length];
			eventsPausFlags  = new bool[drivenAnimsPauses.Length];
		}
	}
	
	public void ResetPosition()
	{
		moveGO.transform.position = startPosition;	
	}
	
	
	
	void Update()
	{
		if (earlyOut)
		{
			return;
		}
		if (wayPointsIterator!=-1 && !stopExecuteFlag)
		{
			bool validIterator =  wayPointsIterator<(timeToMovesMoveOne.Length);
			if ( validIterator && stepMoveTime<timeToMovesMoveOne[wayPointsIterator])
			{
				if (null != eventsIntFlags && eventsIntFlags.Length>wayPointsIterator && !eventsIntFlags[wayPointsIterator])
				{
					PackedFramesActivator.DispatchDrivenAnim(drivenAnimsIntervals[wayPointsIterator]);
					eventsIntFlags[wayPointsIterator] = true;
				}
				stepMoveTime = Mathf.Min(stepMoveTime+Time.deltaTime,timeToMovesMoveOne[wayPointsIterator]);
				Vector3 lastPointPos = (wayPointsIterator >0) ? points[wayPointsIterator-1].gameObject.transform.position : startPosition;
				Vector3 way = (points[wayPointsIterator].gameObject.transform.position - lastPointPos);
				Vector3 curPos = lastPointPos +  way* (stepMoveTime/timeToMovesMoveOne[wayPointsIterator]);
				moveGO.transform.position = curPos;
			}
			else if (validIterator && pauseTime<=pauses[wayPointsIterator])
			{
				if (null != eventsPausFlags && eventsPausFlags.Length>wayPointsIterator && !eventsPausFlags[wayPointsIterator])
				{
					PackedFramesActivator.DispatchDrivenAnim(drivenAnimsPauses[wayPointsIterator]);
					eventsPausFlags[wayPointsIterator] = true;
				}
				pauseTime+=Time.deltaTime;
				moveGO.transform.position = points[wayPointsIterator].gameObject.transform.position;
				
			}
			else if (validIterator)
			{
				stepMoveTime = 0f;
				pauseTime = 0f;
				wayPointsIterator++;
			}
			else
			{
				if (revertPositionOnEnd)
				{
					ResetPosition();
				}

				wayPointsIterator = -1;		
				if (oneTimeRunner)
				{
					earlyOut = true;
				}
			}
		}
		else if (stopExecuteFlag)
		{
			wayPointsIterator = -1;
			stepMoveTime = 0f;
			pauseTime = 0f;	
			stopExecuteFlag = false;
		}
	}
	
	private void StopExecuteCoroutines()
	{
		stopExecuteFlag = true;
	}
	
	
}
