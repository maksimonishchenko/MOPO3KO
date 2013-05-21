using UnityEngine;
using System.Collections;


public class ButtonRegistererActivatorMultipleCollider : MonoBehaviour 
{
	
	public Indicators indi;
	
	private UIButton3D but;
	private System.Action callBack = delegate {
		Debug.Log("not registered any callback event this button");
	};
	
	public Collider[] colliders;
	
	void Start () 
	{
		
		if (null != colliders)
		{
			foreach(Collider c in colliders)
			{
				if (null!=c.gameObject.GetComponent<UIButton3D>())
				{
					but = c.gameObject.GetComponent<UIButton3D>();
					but.scriptWithMethodToInvoke = this;
					but.methodToInvoke = "Click";
				}
			}
		}
		callBack = PackedFramesActivator.GetButtonCallback(gameObject.name);
		if (null != indi)
		{
			indi.InitIndicatorAnimByName(gameObject.name);
			PackedFramesActivator.SetOnSelfRevertAnimationAcition(gameObject.name,() => {indi.AnimationCompleteCallback(gameObject.name);});
		}
	}
	
	void Click()
	{
		callBack();
	}
}

	