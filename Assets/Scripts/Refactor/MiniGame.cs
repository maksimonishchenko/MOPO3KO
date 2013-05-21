using UnityEngine;
using System.Collections;

public class MiniGame : MonoBehaviour 
{
	public Indicators indi;
	protected bool playing = false;
	
	public virtual string[] InitGUI ()
	{
		Debug.Log("minigame INITGUI ");
		return null;
	}
	public virtual void GameStart()
	{
		Debug.Log("minigame start ");
	}
	
	protected virtual void GameFinished()
	{
		Debug.Log("minigame end ");
	}
}
