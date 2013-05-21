using UnityEngine;
using System.Collections;


public class Indicators : MonoBehaviour 
{
	
	public SpriteText animsCur;
	public SpriteText animsFull;
	
	public SpriteText miniCur;
	public SpriteText miniFull;
	
	public PackedSprite animRect;
	public PackedSprite miniRect;

	public string uniqueHelpMessage;
	
	public string default1HelpMessage;   
	public string default2HelpMessage;  
	
	public AudioClip[] default1HelpAudioClip;
	public AudioClip[] default2HelpAudioClip;
	public AudioClip[] uniqueMiniAudioClip;
	
	public TextsManager txtManager;

	public MiniGame mini;
	void Start()
	{
	
		string levelname = Global.levelNeedToLoad.ToString();
		
		if (levelname == "10")
		{
			levelname = "2";
		}
		if (PlayerPrefs.GetInt(levelname+"mini",0) == 1)
		{
			InitIndicatorMiniByName("doesntmatter",levelname);
			if (null!=default2HelpMessage && null !=txtManager)
			{
				txtManager.HintWindiwText = default2HelpMessage;
				txtManager.HintWindowClip = default2HelpAudioClip[UnityEngine.Random.Range(0,default2HelpAudioClip.Length)];
			}
			ShowRedHidePink();
			
		}
		else
		{
			if (null!=default1HelpMessage && null !=txtManager)
			{
				txtManager.HintWindiwText = default1HelpMessage;
				txtManager.HintWindowClip = default1HelpAudioClip[UnityEngine.Random.Range(0,default1HelpAudioClip.Length)];
			}
		}
		
		Debug.Log("  levelName loaded " + levelname);
	}
	
	
	private void ShowRedHidePink()
	{
		Transform pink = gameObject.transform.Find("rectangle_anims/pinkMedal");
		if (null != pink)
		{
			pink.gameObject.active = false;	
		}
		Transform red = gameObject.transform.Find("redMedal");
		if (null != red)
		{
			red.gameObject.active = true;	
		}
		Debug.Log("show red hide pink code ");
	}
	
	private void ShowBlueHideTeal()
	{
		Transform teal = gameObject.transform.Find("rectangle_interactive/tealMedal");
		if (null != teal)
		{
			teal.gameObject.active = false;	
		}
		Transform blue = gameObject.transform.Find("blueMedal");
		if (null != blue)
		{
			blue.gameObject.active = true;	
		}
		
	}
	
		
		
	//One of the child button callback
	public void EnableMiniSetHelpMessage()
	{
		mini.GameStart();
		if (null != txtManager && null != uniqueHelpMessage)
		{
			txtManager.HintWindiwText = uniqueHelpMessage;
			txtManager.HintWindowClip = uniqueMiniAudioClip[UnityEngine.Random.Range(0,uniqueMiniAudioClip.Length)];
			txtManager.Invoke("OPENHINT",0f);
		}
	}
	
	public void AnimationCompleteCallback(string animName,string levelName = "default")
	{
		if (levelName == "default")
		{
			levelName = Global.levelNeedToLoad.ToString();
		}
		
		if (PlayerPrefs.GetInt(levelName+animName,0) == 0)
		{
			PlayerPrefs.SetInt(levelName+animName,1);
			animsCur.Text = (int.Parse(animsCur.Text) + 1).ToString();
			
			if ( int.Parse(animsCur.Text) > int.Parse(animsFull.Text.Replace("/","")))
			{
				animsCur.Text = int.Parse(animsFull.Text.Replace("/","")).ToString();
			}
			if ( int.Parse(animsCur.Text) == int.Parse(animsFull.Text.Replace("/","")) &&  null !=mini)
			{
				mini.GameStart();
				if (null != txtManager && null != uniqueHelpMessage)
				{
					txtManager.HintWindiwText = uniqueHelpMessage;
					txtManager.HintWindowClip = uniqueMiniAudioClip[UnityEngine.Random.Range(0,uniqueMiniAudioClip.Length)];
					txtManager.Invoke("OPENHINT",2f);
				}
				PlayerPrefs.SetInt(levelName+"mini",1);
				mini.InitGUI();
				txtManager.PlayRandomMiniClip();
				ShowRedHidePink();
			}
		}
	}
	
	public void MiniGameCompleteCallback(string animName,string levelName = "default")
	{
		
	}
	
	
	public void InitIndicatorAnimByName(string name,string levelName = "default")
	{
		if (!animRect.gameObject.active)
		{
			bool pinkActive = gameObject.transform.Find("rectangle_anims/pinkMedal").gameObject.active;
			
			animRect.gameObject.SetActiveRecursively(true);
			
			gameObject.transform.Find("rectangle_anims/pinkMedal").gameObject.active = pinkActive;
		}
		
		int oldNum = 0;
		if (int.TryParse(animsFull.Text.Replace("/",""),out oldNum))
		{
			animsFull.Text = "/"+(oldNum+1).ToString();
		}
		else
		{
			Debug.Log("cant parse " + animsFull.Text);
			animsFull.Text = "/" +"1";	
		}
		
		if (animsCur.Text=="")
		{
			animsCur.Text = "0";
		}
		
		if (levelName == "default")
		{
			levelName = Global.levelNeedToLoad.ToString();
		}
		if (PlayerPrefs.GetInt(levelName+name,0) == 1)
		{
			animsCur.Text = (int.Parse(animsCur.Text) + 1).ToString();
		}
		
		
		
	}
	
	public void PlayRandomMiniClipInTextManager()
	{
		txtManager.PlayRandomMiniClip();
	}
	
	
	public void InitIndicatorMiniByName(string name,string levelName = "default")
	{
		
		if (levelName == "default")
		{
			levelName = Global.levelNeedToLoad.ToString();
		}
		if (!miniRect.gameObject.active)
			{
				miniRect.gameObject.SetActiveRecursively(true);
			}
		
		if (PlayerPrefs.GetInt(levelName+"minifinished",0) == 0)
		{
			miniFull.Text = "/1";
			miniCur.Text = "0";
		}
		else
		{
			miniFull.Text = "/1";
			miniCur.Text = "1";
			if (null!=default2HelpMessage && null !=txtManager)
			{
				txtManager.HintWindiwText = default2HelpMessage;
				txtManager.HintWindowClip = default2HelpAudioClip[UnityEngine.Random.Range(0,default2HelpAudioClip.Length)];
			}
			
			ShowBlueHideTeal();
			
		}
		
	}
	
	
}
