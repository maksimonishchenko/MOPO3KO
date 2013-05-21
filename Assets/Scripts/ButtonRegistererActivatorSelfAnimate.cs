using UnityEngine;
using System.Collections;


public class ButtonRegistererActivatorSelfAnimate : MonoBehaviour 
{
	
	public Indicators indi;
	
	private UIButton3D but;
	private System.Action callBack = delegate {
		Debug.Log("not registered any callback event this button");
	};
	public Animation anim;
	public AudioSource sound;
	
	void Start () 
	{
		if (null != gameObject.GetComponentInChildren<UIButton3D>())
		{
			but = gameObject.GetComponentInChildren<UIButton3D>();
			but.scriptWithMethodToInvoke = this;
			but.methodToInvoke = "Click";
			
			
			if (null != indi)
			{
				indi.InitIndicatorAnimByName(gameObject.name);
			}
			
			
		}
	}
	//calls from Unity Animation component in Animation Editor
	public void AnimateCompleteCallback()
	{
		indi.AnimationCompleteCallback(gameObject.name);
	}
	
	void Click()
	{
		if (!anim.isPlaying)
		{
			anim.clip =  anim.GetClip(gameObject.name);
			anim.Play();
			if (null != sound)
			{
				sound.Play();	
			}
		}
		
	}
}
