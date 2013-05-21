using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropLogic : MiniGame
{
	
	public DragDropMessenger messenger;

	
	
	//mini game indicator callbacks 
	public override string[] InitGUI ()
	{
		return null;
	} 
	
	public override void GameStart()
	{
		
	}
	
	protected override void GameFinished()
	{
		
	}
	
	//messenger callbacks
	public virtual void ObjDropped(string dragObjName)
	{
		
	}
	
}

