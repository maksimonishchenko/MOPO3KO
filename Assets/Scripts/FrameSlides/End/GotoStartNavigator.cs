using UnityEngine;
using System.Collections;

public class GotoStartNavigator : MonoBehaviour 
{
	public UIButton3D gotoStart;
	
	// Use this for initialization
	void Start () 
	{
		if (null != gotoStart)
		{
			gotoStart.scriptWithMethodToInvoke = this;
			gotoStart.methodToInvoke = "GotoStart";
		}
	}
	
	// Update is called once per frame
	void GotoStart () 
	{
		PlayerPrefs.DeleteAll();
		Global.levelNeedToLoad = 1;
		Application.LoadLevel(0);
	}
}
