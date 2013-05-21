using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextsManager : MonoBehaviour 
{
	
	public string[] turnSplittedStrings ;
	public AudioClip[] turnSplittedAudioClips ;
	public SpriteText[] pagesSpriteTexts;
	
	
	public AudioClip[] speakerMiniClips;
	
	public AudioSource speakerMiniSource;
	public AudioSource speakerAudioSource;
	public AudioSource speakerHintAudioSource;
	
	public float PAUSE_BETWEEN_TURN   	= 	0.5f;
	public float PAUSE_BETWEEN_LETTER   = 	0.1f;
	
	
	
	
	public void PlayRandomMiniClip()
	{
		if (null != speakerMiniSource)
		{
			speakerMiniSource.PlayOneShot(speakerMiniClips[UnityEngine.Random.Range(0,speakerMiniClips.Length)]);
		}
	}
	
	
	private AudioClip hintWindowClip;
	public AudioClip HintWindowClip
	{
		get
		{
			return hintWindowClip;
		}
		set
		{
			hintWindowClip = value;
			speakerHintAudioSource.clip = hintWindowClip;
		}
	}
	
	private string hintWindowText;
	public string HintWindiwText
	{
		get
		{
			return hintWindowText;
		}
		set
		{
			hintWindowText = value;
			if (null != bucket && null != bucket.TEXT_HINT && bucket.TEXT_HINT.active)
			{
				bucket.TEXT_HINT.GetComponent<SpriteText>().Text = value;	
			}
		}
	}
	
	private int pageNumber ;
	private int letterNumber;

	private float timePageInited;
	private int lettersThisTurn = 1;
	
	private bool activated 			=	 false;
	private bool onPause   			= 	false;
	
	private UIPanel bigBookPanel;
	private UIPanel smallBookPanel;
	private UIPanel smallShieldPanel;
	
	
	private float audioClipReturnFromCloseDelay = 0;
	
	
	private UIBucket bucket;
	private bool closedMainBook = true;
	private bool closedHintBook = true;
	
	void Start()
	{
		

		if (null!=pagesSpriteTexts[0])
		{
			pagesSpriteTexts[0].Text = "";	
		}
		bucket = GetComponent<UIBucket>();
		if (null == bucket)
		{
			return;
		}
		
		if (turnSplittedStrings.Length != turnSplittedAudioClips.Length)
		{
			Debug.LogError("number of texts != number of audioclips assiged : TextManager " );
		}

		pageNumber = letterNumber = -1;
		OPEN();
		TurnPageRight();
		
	}
	
	private void TurnPageRight()
	{
		if (pageNumber>=turnSplittedStrings.Length-1)
		{
			return;
		}
		
		pageNumber++;
		InitPageParameters();
	}
	
	private void TurnPageLeft()
	{
		if (pageNumber<=0)
		{
			return;
		}
		
		pageNumber--;
		InitPageParameters();
	}
	
	private int GetStringLengthBySymbolsLength(string str,int symbolsCount)
	{
		int countLetters = 0;
		
		for(int z=0;z<str.Length;z++)
		{
			if (symbolsCount==countLetters)
			{
				return z;
			}
			if (str.Substring(z,1)!=" ")
			{
				countLetters++;
			}
			
		}
		return str.Length;
	}
	
	private void StringFormatFinalizator(int lettersMayBeActuallyDisplayedThisTime,string final,SpriteText stext)
	{
		lettersMayBeActuallyDisplayedThisTime = GetStringLengthBySymbolsLength(final,lettersMayBeActuallyDisplayedThisTime);
		stext.Text = final.Substring(0,lettersMayBeActuallyDisplayedThisTime);
		if (stext.Text.Length>1 && stext.Text.Substring(stext.Text.Length-1,1)!=" " && lettersMayBeActuallyDisplayedThisTime<final.Length-1 && final.Substring(lettersMayBeActuallyDisplayedThisTime,1)!=" ")
		{
			int lineEnd = 0;
			int lineSI = 0;
			int lineEI = 0;
			stext.Text = final;
			stext.GetDisplayLineCount(lettersMayBeActuallyDisplayedThisTime-1,out lineEnd,out lineSI,out lineEI);
			
			
			stext.Text = final.Substring(0,lettersMayBeActuallyDisplayedThisTime);
			
			while(stext.GetDisplayLineCount()!=lineEnd)
			{
				string nonSpaced = stext.Text;
				stext.Text = nonSpaced.Insert(nonSpaced.LastIndexOf(" "),"  ");
			}
			
			
		}	
		
	}
	
	void Update()
	{
		if (closedMainBook)
		{
			timePageInited = Time.realtimeSinceStartup;
			return;
		}
		if (pageNumber<0 || pageNumber>=turnSplittedStrings.Length)
		{
			return;
		}
		
		int lettersNeedToBeDisplayedByThisTime =  Mathf.RoundToInt ( (Time.realtimeSinceStartup - timePageInited)/PAUSE_BETWEEN_LETTER);
		int lettersMayBeActuallyDisplayedThisTime = Mathf.Min(lettersThisTurn+1,lettersNeedToBeDisplayedByThisTime);
		
		
		StringFormatFinalizator(lettersMayBeActuallyDisplayedThisTime,turnSplittedStrings[pageNumber],pagesSpriteTexts[0]);
	
		
		if (lettersNeedToBeDisplayedByThisTime > lettersThisTurn + PAUSE_BETWEEN_TURN/PAUSE_BETWEEN_LETTER)
		{
			if (pageNumber>=turnSplittedStrings.Length-1)
			{
				if (!IsInvoking("OPENHINT"))
				{
					Invoke("OPENHINT",5f);	
				}
			}
			else
			{
				TurnPageRight();		
			}
		}
		
	}
	
	
	
	
	private void CLOSE()
	{
		closedMainBook = true;
		bucket.MAIN_BOOK.gameObject.SetActiveRecursively(false);
		speakerAudioSource.Stop();
	}
	
	private void OPEN()
	{
		if (closedMainBook)
		{
			if (!closedHintBook)
			{
				CLOSEHINT();
			}
			closedMainBook = false;
			bucket.MAIN_BOOK.SetActiveRecursively(true);
			RepaintLeftRightPageRenders(pageNumber);
			speakerAudioSource.Play();	
			RepaintSoundToggles("speakerON",bucket.SOUND_OFF_BUTTON_MAIN,bucket.SOUND_ON_BUTTON_MAIN);
			ReinitSoundStatus("speakerON",speakerAudioSource);
		}
		
	}
	
	private void OPENHINT()
	{
		if (closedHintBook)
		{
			if (!closedMainBook)
			{
				CLOSE();
			}
			bucket.HELP_WINDOW.SetActiveRecursively(true);
			bucket.TEXT_HINT.GetComponent<SpriteText>().Text = hintWindowText;	
			closedHintBook = false;
			speakerHintAudioSource.Play();
			if (!IsInvoking("CLOSEHINT"))
			{
				Invoke("CLOSEHINT",speakerHintAudioSource.clip.length +3f);
			}
			RepaintSoundToggles("speakerONHELP",bucket.SOUND_OFF_BUTTON_HINT,bucket.SOUND_ON_BUTTON_HINT);
			ReinitSoundStatus("speakerONHELP",speakerHintAudioSource);
		}
	}
	
	private void CLOSEHINT()
	{
		bucket.HELP_WINDOW.SetActiveRecursively(false);
		closedHintBook = true;
		speakerHintAudioSource.Stop();
	}
	
	
	private void SOFF()
	{
		string prefsField = "speakerON";
		PlayerPrefs.SetInt(prefsField,1);
		RepaintSoundToggles(prefsField,bucket.SOUND_OFF_BUTTON_MAIN,bucket.SOUND_ON_BUTTON_MAIN);
		ReinitSoundStatus(prefsField,speakerAudioSource);
	}
	private void SON()
	{
		string prefsField = "speakerON";
		PlayerPrefs.SetInt(prefsField,0);
		RepaintSoundToggles(prefsField,bucket.SOUND_OFF_BUTTON_MAIN,bucket.SOUND_ON_BUTTON_MAIN);
		ReinitSoundStatus(prefsField,speakerAudioSource);
	}
	
	private void SOFFHINT()
	{
		string prefsField = "speakerONHELP";
		PlayerPrefs.SetInt(prefsField,1);
		RepaintSoundToggles(prefsField,bucket.SOUND_OFF_BUTTON_HINT,bucket.SOUND_ON_BUTTON_HINT);
		ReinitSoundStatus(prefsField,speakerHintAudioSource);
	}
	private void SONHINT()
	{
		string prefsField = "speakerONHELP";
		PlayerPrefs.SetInt(prefsField,0);
		RepaintSoundToggles(prefsField,bucket.SOUND_OFF_BUTTON_HINT,bucket.SOUND_ON_BUTTON_HINT);
		ReinitSoundStatus(prefsField,speakerHintAudioSource);
	}
	
	
	
	private void RepaintSoundToggles(string prefsFiels,GameObject OFFObj,GameObject ONObj)
	{
		if (PlayerPrefs.GetInt(prefsFiels,1) == 1)
		{
			OFFObj.SetActiveRecursively(false);	
			ONObj.SetActiveRecursively(true);	
			PlayerPrefs.SetInt(prefsFiels,1);
		}
		else
		{
			
			ONObj.SetActiveRecursively(false);
			OFFObj.SetActiveRecursively(true);	
			PlayerPrefs.SetInt(prefsFiels,0);
		}
	}
	
	private void ReinitSoundStatus(string prefsField,AudioSource source)
	{
		if (PlayerPrefs.GetInt(prefsField,1) == 1)
		{
			source.mute = false;
		}
		else
		{
			source.mute = true;
		}
	}
	
	
	private void InitPageParameters()
	{
		timePageInited  = Time.realtimeSinceStartup;
		letterNumber = 0;
		speakerAudioSource.Stop();
		speakerAudioSource.clip = turnSplittedAudioClips[pageNumber];
		speakerAudioSource.Play();
		RepaintLeftRightPageRenders(pageNumber);
		lettersThisTurn = turnSplittedStrings[pageNumber].Replace(" ","").Length;
		PAUSE_BETWEEN_LETTER =  turnSplittedAudioClips[pageNumber].length/lettersThisTurn; 
		
	}
	
	
	
	private void RepaintLeftRightPageRenders(int pageNumber)
	{
		if (pageNumber>0)
		{
			bucket.LISTLEFT_BUTTON_MAIN.gameObject.SetActiveRecursively(true);
		}
		else
		{
			bucket.LISTLEFT_BUTTON_MAIN.gameObject.SetActiveRecursively(false);
		}
		if (pageNumber<turnSplittedStrings.Length-1)
		{
			bucket.LISTRIGHT_BUTTON_MAIN.gameObject.SetActiveRecursively(true);
		}
		else
		{
			bucket.LISTRIGHT_BUTTON_MAIN.gameObject.SetActiveRecursively(false);	
		}
	}
}

