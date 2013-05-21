using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragDropMessenger : MonoBehaviour 
{
	
	public DropLogic dropLogic;
	
	private Vector3 startPoss;
	private float error = 60f;
	
	private Vector3 capturedPosition;
	private GameObject capturedTarget;
	private bool draggable = true;
	private Vector3 mouseLastPos;
	private bool coimpareToStartPosition = false;
	private bool pickDropShift = false;
	
	private GameObject lastHittedPanel = null;
	private bool allDropsOneTime = false;
	private List<GameObject> unallowedDrops = new List<GameObject>(0);
	
	public Vector3 GetLastHitPanelScreenPos()
	{
		Vector3 screenPos = Vector3.zero;
		return lastHittedPanel.transform.position;
	}
	public void SetPickDropShift(bool b)
	{
		pickDropShift = b;
	}
	
	public void SetDraggable(bool b)
	{
		draggable = b;
	}
	
	public void SetAllowDropsOneTime(bool oneTimeOnly)
	{
		allDropsOneTime = oneTimeOnly;
	}
	
	public void ResetOneTimers()
	{
		unallowedDrops = new List<GameObject>(0);
	}
	
	public void SetCompareToStartBeforeAcceptDrop()
	{
		coimpareToStartPosition = true;
	}
	
	public void UNsetDroppableCollider(GameObject uiButObj)
	{
		uiButObj.GetComponent<UIButton3D>().SetInputDelegate((ref POINTER_INFO p) => {});
	}
	public void SetDroppableCollider(GameObject uiButObj,params GameObject[] colliderDropObj)
	{
		GameObject goBut = uiButObj;
		Vector3 startPosition = goBut.transform.position;
		UIButton3D but = uiButObj.GetComponent<UIButton3D>();
		List<GameObject> allowedDrops = new List<GameObject>(0);
		allowedDrops.AddRange(colliderDropObj);
		
		but.SetInputDelegate((ref POINTER_INFO ptr) => 
		{
			if (draggable)
			{
				if (!goBut.renderer.enabled)
				{
					goBut.renderer.enabled = true;
				}
				Ray ray = Camera.main.ScreenPointToRay(ptr.devicePos);
				
				if (ptr.evt == POINTER_INFO.INPUT_EVENT.DRAG)
				{
					
					if (pickDropShift)
					{
						goBut.transform.position = new Vector3(ray.origin.x,ray.origin.y,startPosition.z - 1f);
					}
					else
					{
						goBut.transform.position = new Vector3(ray.origin.x,ray.origin.y,startPosition.z);
					}
				}
				else if (ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF || ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
				{
					//int layermask = 1 << LayerMask.NameToLayer("Droppable");
					RaycastHit hit;
					//if (Physics.Raycast(ray,out hit,200,layermask))
					if (Physics.Raycast(ray,out hit,200) && allowedDrops.Contains(hit.transform.gameObject))
					{
						if (coimpareToStartPosition && Vector3.Distance(goBut.transform.position,startPosition) > error)
						{
							Debug.Log("distance more than error  not same " + Vector3.Distance(goBut.transform.position,startPosition) );
							return;
						}
						if (allDropsOneTime)
						{
							if (unallowedDrops.Contains(hit.transform.gameObject))
							{
								Debug.Log("already drop this game on " +  hit.transform.gameObject);
								return;
							}
							else
							{
								unallowedDrops.Add(hit.transform.gameObject);
							}
						}
						lastHittedPanel = hit.transform.gameObject;
						//---dropped on droppable ---
						dropLogic.ObjDropped(goBut.name);

						
					}
					
					else if ( hit.transform == but.transform && Physics.Raycast(new Ray(hit.point,Vector3.forward),out hit,200) && allowedDrops.Contains(hit.transform.gameObject))
					{
						if (coimpareToStartPosition && Vector3.Distance(goBut.transform.position,startPosition) > error)
						{
							Debug.Log("distance more than error first same  " + Vector3.Distance(goBut.transform.position,startPosition));
							return;
						}
						if (allDropsOneTime)
						{
							if (unallowedDrops.Contains(hit.transform.gameObject))
							{
								Debug.Log("already drop this game on " +  hit.transform.gameObject);
								return;
							}
							else
							{
								unallowedDrops.Add(hit.transform.gameObject);
							}
						}
						
						lastHittedPanel = hit.transform.gameObject;
						//---dropped on droppable ---
						dropLogic.ObjDropped(goBut.name);
						
						
					}

					else
					{
						if (pickDropShift)
						{
							goBut.transform.position= new Vector3(goBut.transform.position.x,goBut.transform.position.y,startPosition.z);
						}
					}
				}
			}
		});
	}
	
	
}

