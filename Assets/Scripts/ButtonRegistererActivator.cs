using UnityEngine;
using System.Collections;


public class ButtonRegistererActivator : MonoBehaviour 
{
	
	public Indicators indi;
	
	private UIButton3D but;
	private System.Action callBack = delegate {
		Debug.Log("not registered any callback event this button");
	};
	
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
	
	void Click()
	{
		callBack();
	}
}
