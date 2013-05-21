using UnityEngine;
using System.Collections;

public class LoadingAndGUI : MonoBehaviour 
{
	
	public UIProgressBar loadedProgressBar;
	public Material loadingMaterial;
	
	
	void Start()
	{	
		
		if (Screen.height<1536)
		{
			Texture2D text1 = Resources.Load( "1/"+ loadingMaterial.name,typeof(Texture2D)) as Texture2D;
			loadingMaterial.mainTexture = text1;	
		}
		else
		{
			Texture2D text1 = Resources.Load( "2/"+ loadingMaterial.name,typeof(Texture2D)) as Texture2D;
			loadingMaterial.mainTexture = text1;	
		}
		
		loadedProgressBar.transform.parent.gameObject.SetActiveRecursively(true);
		
		StartCoroutine(LoadingExecutorRoller(Global.levelNeedToLoad));
	}
	
	private IEnumerator LoadingExecutorRoller(int levelNum) 
	{
		AwakeTextureLoad.UnLoadSpriteTextsExceptForLevel(levelNum);
		AwakeTextureLoad.UnLoadTexturesExceptForLevel(levelNum);
		
		
		AsyncOperation oper = Application.LoadLevelAdditiveAsync(levelNum);
		//yield return oper;
		while(!oper.isDone)
		{
			yield return new WaitForSeconds(0.02f);
			loadedProgressBar.Value = oper.progress;		
		}
		loadedProgressBar.transform.parent.gameObject.SetActiveRecursively(false);
		
		Resources.UnloadAsset(loadingMaterial.mainTexture);	
		Destroy(loadedProgressBar.transform.parent.gameObject);
		Destroy(this.gameObject);
		//Resources.UnloadUnusedAssets();
	}
	
}
