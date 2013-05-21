using UnityEngine;
using System.Collections;


public class ButtonRegistererRevertToStatic : MonoBehaviour 
{
	
	public Indicators indi;
	
	private UIButton3D but;
	private System.Action callBack = delegate {
		Debug.Log("not registered any callback event this button");
	};
	
	
	public PackedSprite[] revertToStaticSprites;
	public float cooldownTime = 1.5f;
	private bool logicRevertValve = false;
	private float timeToSwitch = -1f;
	
	public AudioSource stopPlayinClip;
	public AudioSource playOneShotClip;
	
	void Start () 
	{
		if (null != gameObject.GetComponent<UIButton3D>())
		{
			but = gameObject.GetComponent<UIButton3D>();
			but.scriptWithMethodToInvoke = this;
			but.methodToInvoke = "Click";
			callBack = PackedFramesActivator.GetButtonCallback(gameObject.name);
			
			if (null != indi)
			{
				indi.InitIndicatorAnimByName(gameObject.name);
				PackedFramesActivator.SetOnSelfRevertAnimationAcition(gameObject.name,() => {indi.AnimationCompleteCallback(gameObject.name);});
			}		
		}
	}
	
	void Update()
	{
		if (timeToSwitch>0f)
		{
			timeToSwitch-=Time.deltaTime;
			if (timeToSwitch<0f)
			{
				Switch();
			}
		}
	}
	
	private void Switch()
	{
		logicRevertValve = !logicRevertValve;
	}
	
	
	void Click()
	{
		if (logicRevertValve)
		{
			foreach(PackedSprite ps in revertToStaticSprites)
			{
				ps.RevertToStatic();
			}
			if (null != stopPlayinClip)
			{
				stopPlayinClip.Stop();
			}
			if (null != playOneShotClip)
			{
				playOneShotClip.PlayOneShot(playOneShotClip.clip);
			}
			Switch();
		}
		else if (timeToSwitch<0f)
		{
			callBack();	
			timeToSwitch = cooldownTime;
		}
	}
}
