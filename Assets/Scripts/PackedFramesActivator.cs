using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PackedFramesActivator : MonoBehaviour 
{
	public GameObject pSpriteAnimContainerGO;
	public GameObject pSpriteAnimDisabledContainerGO;
	
	private static List<string> firstTimeCallers;	//hack var
	
	
	private static List<PackedSprite> activatingSprites = new List<PackedSprite>(0);
	private static List<PackedSprite> disabledSprites = new List<PackedSprite>(0);
	private static Dictionary<PackedSprite,bool> running = new Dictionary<PackedSprite,bool>(0);
	
	
	private static Dictionary<string,System.Action> onSetBusyFalse = new Dictionary<string, System.Action>(0);
	
	
	
	public static void DispatchDrivenAnim(DrivenAnimation anim)
	{
		if (anim.playSprites!=null && anim.playSprites.Length>0)
		{
			foreach(SpriteBase sb in  anim.playSprites)
			{
				Debug.Log("playanim "  + sb.gameObject.name);
				sb.gameObject.active=true;
				sb.renderer.enabled = true;
				//anim.spriteObject.PlayAnim(anim.animID);
				if (sb is AutoSpriteBase)
				{
					AutoSpriteBase autoSB = (sb as AutoSpriteBase);
					
					if (null != autoSB.animations && anim.animID >= 0 && anim.animID < autoSB.animations.Length)
					{
						autoSB.PlayAnim(anim.animID,anim.spriteBaseStartPlayingFrame);
					}
					else
					{
						autoSB.renderer.enabled = true;
					}
					//(anim.spriteObject as AutoSpriteBase).PlayAnim(anim.animID,anim.spriteBaseStartPlayingFrame);
				}
				else
				{
					sb.PlayAnim(anim.driverAnimID);
				}
			}		
	
		}
		
		if (anim.audio)
		{
			if (!anim.playOnce)
				anim.audio.Play();
			else
			{
				if (!anim.soundPlayed)
				{
					anim.soundPlayed = true;
					anim.audio.Play();
				}
			}
		}
		
		if (anim.monoBeh)
		{
			Debug.Log("send message monobehaviour " + anim.msg_name);
			anim.monoBeh.gameObject.SendMessage(anim.msg_name);
		}
		
		List<PackedSprite> hided = new List<PackedSprite>(0);
		if (null != anim.hideSprites && anim.hideSprites.Length > 0)
		{
			foreach(PackedSprite ps in anim.hideSprites)
			{
				if (null != ps)
				{
					ps.Hide(true);	
					hided.Add(ps);
				} 
			}
		}
		
		if (null != anim.blockedSprites && anim.blockedSprites.Length > 0)
		{
			foreach(PackedSprite ps in anim.blockedSprites)
			{
				if (null != ps)
				{
					ChangeBusyStatus(ps,true);
					
				} 
			}
		}
		
		if (null != anim.unblockedSprites && anim.unblockedSprites.Length > 0)
		{
			foreach(PackedSprite ps in anim.unblockedSprites)
			{
				if (null != ps)
				{
					ChangeBusyStatus(ps,false);
				} 
			}
		}
		
		if (null != anim.busyFalseActionFire && anim.busyFalseActionFire.Length > 0)
		{
			foreach(PackedSprite ps in anim.busyFalseActionFire)
			{
				if (null != ps)
				{
					OnBusyFalseAction(ps);
				} 
			}
		}
		
		
		if (null != anim.revertSprites && anim.revertSprites.Length > 0)
		{
			foreach(PackedSprite ps in anim.revertSprites)
			{
				if (null !=ps)
				{
					ps.RevertToStatic();
					if (!hided.Contains(ps))
					{
						ps.renderer.enabled = true;
					}
					SetBusyMode(ps,false);
				}
				
			}
		}
		
		if (anim.playInReverseSprites!=null && anim.playInReverseSprites.Length>0)
		{
			foreach(SpriteBase sb in  anim.playInReverseSprites)
			{
				Debug.Log("playanim "  + sb.gameObject.name);
				sb.gameObject.active=true;
				sb.renderer.enabled = true;
				//anim.spriteObject.PlayAnim(anim.animID);
				if (sb is AutoSpriteBase)
				{
					AutoSpriteBase autoSB = (sb as AutoSpriteBase);
					
					if (null != autoSB.animations && anim.animID >= 0 && anim.animID < autoSB.animations.Length)
					{
						autoSB.PlayAnimInReverse(anim.animID,anim.spriteBaseStartPlayingFrame);
					}
					else
					{
						autoSB.renderer.enabled = true;
					}
					//(anim.spriteObject as AutoSpriteBase).PlayAnim(anim.animID,anim.spriteBaseStartPlayingFrame);
				}
				else
				{
					sb.PlayAnimInReverse(anim.driverAnimID);
				}
			}		
	
		}
		
	}
	
	
	private static void OnFrameTick(AutoSpriteBase autoSBase)
	{
		DrivenAnimation[] drivenAnims = autoSBase.GetDrivenAnimations();
		//Debug.Log("New frame");
		if (drivenAnims!=null && drivenAnims.Length > 0)
		{
			UVAnimation curAnim = autoSBase.GetCurAnim();
			
			int curFrame = curAnim.GetCurPosition();
			int index = curAnim.index;
			foreach(DrivenAnimation animationLooped in drivenAnims)
			{
				DrivenAnimation anim = animationLooped;
				if (anim.driverAnimID == index && anim.actionFrame == curFrame)
				{
					DispatchDrivenAnim(anim);
				}
			}
		}
	}
	
	
	
	
	
	void Start ()
	{
		if (null != pSpriteAnimContainerGO)
		{
			activatingSprites.AddRange(pSpriteAnimContainerGO.GetComponentsInChildren<PackedSprite>());
			disabledSprites.AddRange(pSpriteAnimDisabledContainerGO.GetComponentsInChildren<PackedSprite>());
			
			foreach(PackedSprite ps in activatingSprites)
			{
				running.Add(ps,false);
				//Debug.Log("button activation sprite added " + ps.gameObject.name);
				ps.AddAutoSpriteBaseFrameAction(OnFrameTick);
			}
			
			foreach(PackedSprite ps in disabledSprites)
			{
			//	Debug.Log("driven animation sprite added " + ps.gameObject.name);
				ps.AddAutoSpriteBaseFrameAction(OnFrameTick);
			}
			
		}
		
		firstTimeCallers = new List<string>(0);
		//firstTimeCallers.Add("bolf_Body_00001");
		//firstTimeCallers.Add("gate");
		//firstTimeCallers.Add("gates_00001");
		//firstTimeCallers.Add("birl_Body_00001");	
		
	}
	
	
	public static bool AddActivatingRunningSprites(PackedSprite ps)
	{
		Debug.Log("befor add activaseBut.renderer.enabled = false;");
		if (activatingSprites.Contains(ps))
		{
			return false;
		}
		
		activatingSprites.Add(ps);	
		running.Add(ps,false);
		ps.AddAutoSpriteBaseFrameAction(OnFrameTick);
		
		return true;
	}
	
	public static bool RemoveActivatingRunningSprites(PackedSprite ps)
	{
		
		if (!activatingSprites.Contains(ps))
		{
			Debug.LogError("cant remove activation running sprites doesnt contain packedsprite in running sprites " + ps.name);
			return false;
		}
		
		activatingSprites.Remove(ps);
		running.Remove(ps);
		ps.RemoveAutoSpriteBaseFrameAction(OnFrameTick);
		
		return true;
	}
	
	public static void SetOnSelfRevertAnimationAcition(string name,System.Action act)
	{
		Debug.Log("setonself");
		onSetBusyFalse.Add(name,act);
	}
	
	public static void CleanSelfRevertAnimationAcition(string name)
	{
		onSetBusyFalse.Remove(name);
	}
	
	
	
	private static bool IsBusyRightNow(PackedSprite ps)
	{
		foreach(KeyValuePair<PackedSprite,bool> kvp in running)
		{
			if (kvp.Key == ps)
			{
				return kvp.Value;
			}
		}
		Debug.Log("cant find packedsprite " + ps);
		return true;
	}
	
	public static void Clear()
	{
		activatingSprites = new List<PackedSprite>(0);
		disabledSprites = new List<PackedSprite>(0);
		running = new Dictionary<PackedSprite,bool>(0);
		onSetBusyFalse = new Dictionary<string, System.Action>(0);
	}
	
	
	
	
	public static void ChangeBusyStatus(PackedSprite ps,bool busy)
	{
		
		Debug.Log("cnahge busy status caled " + busy);
		foreach(KeyValuePair<PackedSprite,bool> kvp in running)
		{
			if (kvp.Key == ps)
			{
				running.Remove(ps);
				running.Add(ps,busy);	
				break;
			}
		}
		
	}
	
	public static void OnBusyFalseAction(PackedSprite ps)
	{
		Debug.Log("complete event run " + ps.name);
		foreach(KeyValuePair<PackedSprite,bool> kvp in running)
		{
			if (kvp.Key == ps)
			{
				if (onSetBusyFalse.ContainsKey(ps.name))
				{
					System.Action onSetbusyFalseAction;
					onSetBusyFalse.TryGetValue(ps.name,out onSetbusyFalseAction);
					if (firstTimeCallers.Contains(ps.name))
					{
						firstTimeCallers.Remove(ps.name);
					}
					else
					{
						onSetbusyFalseAction();	
					}
				}
				break;
			}
		}
		
	}
	
	
	
	
	
	public static void SetBusyMode(PackedSprite ps,bool busy)
	{
		if (!busy)
		{
			OnBusyFalseAction(ps);	
		}
		ChangeBusyStatus(ps,busy);
		Debug.Log("set busy mode caled " + busy);
	}
	
	public static System.Action GetButtonCallback(string name)
	{
		return () => 
		{ 
			bool finded = false;
			foreach(PackedSprite ps in activatingSprites)
			{
				PackedSprite packed = ps;
				
				if (packed.gameObject.name == name)
				{
					finded =true;
					
					if (!IsBusyRightNow(ps))
					{
						SetBusyMode(ps,true);
						ps.PlayAnim(0);	
						ps.renderer.enabled = true;
					}
					
				}
			}
			if (!finded)
			{
				Debug.Log("cant find packedSprite named " + name + "to fire button event");
			}
		};
	}
	
	
}
