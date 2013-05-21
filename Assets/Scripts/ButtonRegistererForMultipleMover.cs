using UnityEngine;
using System.Collections;


public class ButtonRegistererForMultipleMover : MonoBehaviour 
{
	public Mover[] movers;
	public Indicators indi;
	public string[] names;
	private int counter = 0;
	private bool completeClickAllAnimations = false; 
	
	private UIButton3D but;
	private System.Action[] callBacks = new System.Action[]{delegate {
		Debug.Log("not registered any callback event this button");
	},delegate {
		Debug.Log("not registered any callback event this button");
	},delegate {
		Debug.Log("not registered any callback event this button");
	},delegate {
		Debug.Log("not registered any callback event this button");
	}};
	
	private bool isMovingRightNow =false;
	
	void Start () 
	{
		if (null != gameObject.GetComponent<UIButton3D>())
		{
			but = gameObject.GetComponent<UIButton3D>();
			but.scriptWithMethodToInvoke = this;
			but.methodToInvoke = "Click";
			
			for(int c=0;c<names.Length;c++)
			{
				callBacks[c] = PackedFramesActivator.GetButtonCallback(names[c]);
			}
			
			
			if (null != indi)
			{
				indi.InitIndicatorAnimByName(gameObject.name);
				
				for(int c=0;c<names.Length;c++)
				{
					PackedFramesActivator.SetOnSelfRevertAnimationAcition(names[c],() =>
					{
						//if (completeClickAllAnimations)
						//{
							
						//}
						indi.AnimationCompleteCallback(gameObject.name);
						Debug.Log("multiple amationmove counter ++");
						isMovingRightNow = false;
						counter++;
						if (counter==names.Length)
						{
							counter=0;
							completeClickAllAnimations = true;
						}
					});
				}
					
				
			}
		}
	}
	
	void Click()
	{
		if (!isMovingRightNow)
		{
			callBacks[counter]();
			movers[counter].StartMoving();
			isMovingRightNow = true;	
		}
	}
}
