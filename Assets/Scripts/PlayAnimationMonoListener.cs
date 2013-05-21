using UnityEngine;
using System.Collections;

public class PlayAnimationMonoListener : MonoBehaviour {
	
	public Animation anim;
	
	
	public void PlayAnim()
	{
		Debug.Log("play anim 1" + anim.name);
		if (anim)
		{
			Debug.Log("play anim 2" + anim.name);
			anim.Play(PlayMode.StopAll);
		}
	}
}
