using UnityEngine;
using System.Collections;


public class ButtonRegistererForMover : MonoBehaviour {
	
	public Mover mover;
	private UIButton3D but;
	private System.Action callBack = delegate {
		Debug.Log("not registered any callback event this button");
	};
	public Indicators indi;
	
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
		mover.StartMoving();
	}
}