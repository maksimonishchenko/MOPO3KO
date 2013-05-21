using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour 
{
	public static int levelID = 0;
	public static string levelName ="";  	
		
	void Start()
	{
		if (levelID!=-1)
		{
			Debug.Log("Loading by level number " + levelID);
			Application.LoadLevel(levelID);
		}
		else
		{
			Debug.Log("Loading by level name " + levelName);
			Application.LoadLevel(levelName);
		}
	}
    
	
}
