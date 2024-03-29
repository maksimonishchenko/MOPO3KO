//-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEngine;
using System.Collections;


/// <remarks>
/// Button class that allows you to invoke a specified method
/// on a specified component script.
/// </remarks>
[AddComponentMenu("EZ GUI/Controls/Button")] 
public class UIButton : AutoSpriteControlBase
{
	/// <summary>
	/// Indicates the state of the button
	/// </summary>
	public enum CONTROL_STATE
	{
		/// <summary>
		/// The button is "normal", awaiting input
		/// </summary>
		NORMAL,

		/// <summary>
		/// The button has an input device hovering over it.
		/// </summary>
		OVER,

		/// <summary>
		/// The button is being pressed
		/// </summary>
		ACTIVE,

		/// <summary>
		/// The button is disabled
		/// </summary>
		DISABLED
	};


	protected CONTROL_STATE m_ctrlState;

	/// <summary>
	/// Gets the current state of the button.
	/// </summary>
	public CONTROL_STATE controlState
	{
		get { return m_ctrlState; }
	}

	public override bool controlIsEnabled
	{
		get { return m_controlIsEnabled; }
		set
		{
			m_controlIsEnabled = value;
			if (!value)
				SetControlState(CONTROL_STATE.DISABLED);
			else
				SetControlState(CONTROL_STATE.NORMAL);
		}
	}

	// State info to use to draw the appearance
	// of the control.
	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
		{
			new TextureAnim("Normal"),
			new TextureAnim("Over"),
			new TextureAnim("Active"),
			new TextureAnim("Disabled")
		};

	public override TextureAnim[] States
	{
		get { return states; }
		set { states = value; }
	}

	// Strings to display for each state.
	[HideInInspector]
	public string[] stateLabels = new string[] { AutoSpriteControlBase.DittoString, AutoSpriteControlBase.DittoString, AutoSpriteControlBase.DittoString, AutoSpriteControlBase.DittoString };

	public override string GetStateLabel(int index)
	{
		return stateLabels[index];
	}

	// Transitions - one set for each state
	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
		{
			new EZTransitionList( new EZTransition[]	// Normal
			{
				new EZTransition("From Over"),
				new EZTransition("From Active"),
				new EZTransition("From Disabled")
			}),
			new EZTransitionList( new EZTransition[]	// Over
			{
				new EZTransition("From Normal"),
				new EZTransition("From Active")
			}),
			new EZTransitionList( new EZTransition[]	// Active
			{
				new EZTransition("From Normal"),
				new EZTransition("From Over")
			}),
			new EZTransitionList( new EZTransition[]	// Disabled
			{
				new EZTransition("From Normal"),
				new EZTransition("From Over"),
				new EZTransition("From Active")
			})
		};

	public override EZTransitionList GetTransitions(int index)
	{
		if (index >= transitions.Length)
			return null;
		return transitions[index];
	}

	public override void SetStateLabel(int index, string label)
	{
		stateLabels[index] = label;
	}

	public override EZTransitionList[] Transitions
	{
		get { return transitions; }
		set { transitions = value; }
	}

	// Helps us keep track of the previous transition:
	EZTransition prevTransition;

	/// <summary>
	/// An array of references to sprites which will
	/// visually represent this control.  Each element
	/// (layer) represents another layer to be drawn.
	/// This allows you to use multiple sprites to draw
	/// a single control, achieving a sort of layered
	/// effect. Ex: You can use a second layer to overlay 
	/// a button with a highlight effect.
	/// </summary>
	public SpriteRoot[] layers = new SpriteRoot[0];


	/// <summary>
	/// Reference to the script component with the method
	/// you wish to invoke when the button is tapped.
	/// </summary>
	public MonoBehaviour scriptWithMethodToInvoke;

	/// <summary>
	/// A string containing the name of the method to be invoked.
	/// </summary>
	public string methodToInvoke;

	/// <summary>
	/// Sets what event should have occurred to 
	/// invoke the associated MonoBehaviour method.
	/// Defaults to TAP.
	/// </summary>
	public POINTER_INFO.INPUT_EVENT whenToInvoke = POINTER_INFO.INPUT_EVENT.TAP;

	/// <summary>
	/// Delay, in seconds, between the time the control is tapped
	/// and the time the method is executed.
	/// </summary>
	public float delay;

	/// <summary>
	/// Sound that will be played when the button is is in an "over" state (mouse over)
	/// </summary>
	public AudioSource soundOnOver;

	/// <summary>
	/// Sound that will be played when the button is activated (pressed)
	/// </summary>
	public AudioSource soundOnClick;

	/// <summary>
	/// When repeat is true, the button will call the various
	/// delegates and invokes as long as the button is held
	/// down.
	/// NOTE: If repeat is true, it overrides any setting of
	/// "whenToInvoke"/"When To Invoke". One exception to this
	/// is that "soundToPlay" is still played based upon
	/// "whenToInvoke".
	/// </summary>
	public bool repeat;


	//---------------------------------------------------
	// State tracking:
	//---------------------------------------------------
	protected int[,] stateIndices;

	
	//---------------------------------------------------
	// Input handling:
	//---------------------------------------------------
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (!m_controlIsEnabled || IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}

		if (inputDelegate != null)
			inputDelegate(ref ptr);

		// Change the state if necessary:
		switch(ptr.evt)
		{
			case POINTER_INFO.INPUT_EVENT.MOVE:
				if (m_ctrlState != CONTROL_STATE.OVER)
				{
					SetControlState(CONTROL_STATE.OVER);
					if (soundOnOver != null)
						soundOnOver.PlayOneShot(soundOnOver.clip);
				}
				break;
			case POINTER_INFO.INPUT_EVENT.DRAG:
			case POINTER_INFO.INPUT_EVENT.PRESS:
				SetControlState(CONTROL_STATE.ACTIVE);
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.TAP:
				// Only go to the OVER state if we have
				// have frame info for that or if we aren't
				// in touchpad mode, or if the collider hit
				// by the touch was actually us, indicating
				// that we're still under the pointer:
				if (ptr.type != POINTER_INFO.POINTER_TYPE.TOUCHPAD &&
					ptr.hitInfo.collider == collider)
					SetControlState(CONTROL_STATE.OVER);
				else
					SetControlState(CONTROL_STATE.NORMAL);
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				SetControlState(CONTROL_STATE.NORMAL);
				break;
		}

		base.OnInput(ref ptr);

		if (repeat)
		{
			if (m_ctrlState == CONTROL_STATE.ACTIVE)
				goto Invoke;
		}
		else if (ptr.evt == whenToInvoke)
			goto Invoke;

		return;

		Invoke:
		if (ptr.evt == whenToInvoke)
		{
			if (soundOnClick != null && soundOnClick.enabled)
				soundOnClick.PlayOneShot(soundOnClick.clip);
		}
		if (scriptWithMethodToInvoke != null)
			scriptWithMethodToInvoke.Invoke(methodToInvoke, delay);
		if (changeDelegate != null)
			changeDelegate(this);
	}

	
	//---------------------------------------------------
	// Misc
	//---------------------------------------------------
	public override void Start()
	{
		base.Start();

		// Runtime init stuff:
		if(Application.isPlaying)
		{
			// Assign our aggregate layers:
			aggregateLayers = new SpriteRoot[1][];
			aggregateLayers[0] = layers;

			// Setup our transitions:
			transitions[0].list[0].MainSubject = this.gameObject;
			transitions[0].list[1].MainSubject = this.gameObject;
			transitions[0].list[2].MainSubject = this.gameObject;
			transitions[1].list[0].MainSubject = this.gameObject;
			transitions[1].list[1].MainSubject = this.gameObject;
			transitions[2].list[0].MainSubject = this.gameObject;
			transitions[2].list[1].MainSubject = this.gameObject;
			transitions[3].list[0].MainSubject = this.gameObject;
			transitions[3].list[1].MainSubject = this.gameObject;
			transitions[3].list[2].MainSubject = this.gameObject;

			stateIndices = new int[layers.Length, 4];

			// Populate our state indices based on if we
			// find any valid states/animations in each 
			// sprite layer:
			for (int i = 0; i < layers.Length; ++i)
			{
				if (layers[i] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + name + "\". Please fill in the layer reference, or remove the empty element.");
					continue;
				}

				stateIndices[i, (int)CONTROL_STATE.NORMAL] = layers[i].GetStateIndex("normal");
				stateIndices[i, (int)CONTROL_STATE.OVER] = layers[i].GetStateIndex("over");
				stateIndices[i, (int)CONTROL_STATE.ACTIVE] = layers[i].GetStateIndex("active");
				stateIndices[i, (int)CONTROL_STATE.DISABLED] = layers[i].GetStateIndex("disabled");

				// Add this as a subject of our transition for 
				// each state, as appropriate:
				if (stateIndices[i, 0] != -1)
				{
					transitions[0].list[0].AddSubSubject(layers[i].gameObject);
					transitions[0].list[1].AddSubSubject(layers[i].gameObject);
					transitions[0].list[2].AddSubSubject(layers[i].gameObject);
				}
				if (stateIndices[i, 1] != -1)
				{
					transitions[1].list[0].AddSubSubject(layers[i].gameObject);
					transitions[1].list[1].AddSubSubject(layers[i].gameObject);
				}
				if (stateIndices[i, 2] != -1)
				{
					transitions[2].list[0].AddSubSubject(layers[i].gameObject);
					transitions[2].list[1].AddSubSubject(layers[i].gameObject);
				}
				if (stateIndices[i, 3] != -1)
				{
					transitions[3].list[0].AddSubSubject(layers[i].gameObject);
					transitions[3].list[1].AddSubSubject(layers[i].gameObject);
					transitions[3].list[2].AddSubSubject(layers[i].gameObject);
				}

				// Setup the layer:
				if (stateIndices[i, (int)m_ctrlState] != -1)
					layers[i].SetState(stateIndices[i, (int)m_ctrlState]);
				else
					layers[i].Hide(true);
			}

			// Create a default collider if none exists:
			if (collider == null)
				AddCollider();

			SetState((int)m_ctrlState);
		}

		// Since hiding while managed depends on
		// setting our mesh extents to 0, and the
		// foregoing code causes us to not be set
		// to 0, re-hide ourselves:
		if (managed && m_hidden)
			Hide(true);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		// Revert to our normal state, but if there isn't
		// an EZAnimator, then we either don't need to
		// revert, or we don't want to because the scene
		// is closing:
		if (EZAnimator.Exists())
			SetControlState(CONTROL_STATE.NORMAL);
	}

	public override void Copy(SpriteRoot s)
	{
		Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);

		if (!(s is UIButton))
			return;

		UIButton btn = (UIButton)s;

		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			prevTransition = btn.prevTransition;

			if (Application.isPlaying)
				SetControlState(btn.controlState);
		}

		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			scriptWithMethodToInvoke = btn.scriptWithMethodToInvoke;
			methodToInvoke = btn.methodToInvoke;
			whenToInvoke = btn.whenToInvoke;
			delay = btn.delay;
		}

		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			soundOnOver = btn.soundOnOver;
			soundOnClick = btn.soundOnClick;
		}

		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			repeat = btn.repeat;
		}
	}

	// Switches the displayed sprite(s) to match the current state:
	public void SetControlState(CONTROL_STATE s)
	{
		// If this is the same as the current state, ignore:
		if (m_ctrlState == s)
			return;

		int prevState = (int)m_ctrlState;

		m_ctrlState = s;

		// Validate that we can go to this appearance:
		if(animations[(int)s].GetFrameCount() > 0)
			this.SetState((int)s);

		this.UseStateLabel((int)s);

		if (s == CONTROL_STATE.DISABLED)
			m_controlIsEnabled = false;
		else
			m_controlIsEnabled = true;

		// Recalculate our collider
		UpdateCollider();

		// Loop through each layer and set its state,
		// provided we have a valid index for that state:
		for (int i = 0; i < layers.Length; ++i)
		{
			if (-1 != stateIndices[i, (int)s])
			{
				layers[i].Hide(false);
				layers[i].SetState(stateIndices[i, (int)s]);
			}
			else
				layers[i].Hide(true);
		}

		// End any current transition:
		if (prevTransition != null)
			prevTransition.StopSafe();

		// Start a new transition:
		StartTransition((int)s, prevState);
	}

	// Starts the appropriate transition
	protected void StartTransition(int newState, int prevState)
	{
		int transIndex = 0;

		// What state are we now in?
		switch(newState)
		{
			case 0:	// Normal
				// Where did we come from?
				switch(prevState)
				{
					case 1: // Over
						transIndex = 0;
						break;
					case 2:	// Active
						transIndex = 1;
						break;
					case 3:	// Disabled
						transIndex = 2;
						break;
				}
				break;
			case 1:	// Over
				// Where did we come from?
				switch (prevState)
				{
					case 0: // Normal
						transIndex = 0;
						break;
					case 2:	// Active
						transIndex = 1;
						break;
				}
				break;
			case 2:	// Active
				// Where did we come from?
				switch (prevState)
				{
					case 0: // Normal
						transIndex = 0;
						break;
					case 1:	// Over
						transIndex = 1;
						break;
				}
				break;
			case 3:	// Disabled
				// Where did we come from?
				switch (prevState)
				{
					case 0: // Normal
						transIndex = 0;
						break;
					case 1:	// Over
						transIndex = 1;
						break;
					case 2:	// Active
						transIndex = 2;
						break;
				}
				break;
		}

		prevTransition = transitions[newState].list[transIndex];
		prevTransition.Start();
	}

	// Sets the default UVs:
	public override void InitUVs()
	{
		if (states[0].spriteFrames.Length != 0)
			frameInfo.Copy(states[0].spriteFrames[0]);

		base.InitUVs();
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIButton Create(string name, Vector3 pos)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		return (UIButton)go.AddComponent(typeof(UIButton));
	}

	/// <summary>
	/// Creates a GameObject and attaches this
	/// component type to it.
	/// </summary>
	/// <param name="name">Name to give to the new GameObject.</param>
	/// <param name="pos">Position, in world space, where the new object should be created.</param>
	/// <param name="rotation">Rotation of the object.</param>
	/// <returns>Returns a reference to the component.</returns>
	static public UIButton Create(string name, Vector3 pos, Quaternion rotation)
	{
		GameObject go = new GameObject(name);
		go.transform.position = pos;
		go.transform.rotation = rotation;
		return (UIButton)go.AddComponent(typeof(UIButton));
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		scriptWithMethodToInvoke = gui.DrawScriptSelection(scriptWithMethodToInvoke, ref methodToInvoke);
	}
}
