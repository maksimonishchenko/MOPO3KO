using UnityEngine;
using System.Collections;

public class GotoSlideNumUIButton : MonoBehaviour 
{
	public UIButton3D button;
	public int slideNum = 10;
	public bool deletePreferences = false;
	
	// Use this for initialization
	void Start () 
	{
		if (null != button)
		{
			button.scriptWithMethodToInvoke = this;
			button.methodToInvoke = "Click";
		}
	}
	
	// Update is called once per frame
	void Click () 
	{
		if (deletePreferences)
		{
			PlayerPrefs.DeleteAll();	
		}
		
		Global.levelNeedToLoad = slideNum;
		Application.LoadLevel(0);
	}
}
