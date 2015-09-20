using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// An Animated UI Component.
/// </summary>
public class AnimatedUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	
	/// <summary>
	/// The wobble trigger.
	/// </summary>
	public const string WobbleTrigger = "Wobble";
	
	/// <summary>
	/// The name of the wobble state.
	/// </summary>
	public const string WobbleStateName = "Wobble";
	
	/// <summary>
	/// The bounce trigger.
	/// </summary>
	public const string BounceTrigger = "Bounce";
	
	/// <summary>
	/// The name of the bounce state.
	/// </summary>
	public const string BounceStateName = "Bounce";

	/// <summary>
	/// The pulse trigger.
	/// </summary>
	public const string PulseTrigger = "Pulse";

	/// <summary>
	/// The name of the pulse state.
	/// </summary>
	public const string PulseStateName = "Pulse";

	/// <summary>
	/// The highlight bool.
	/// </summary>
	public const string HighlightBool = "Highlighted";

	/// <summary>
	/// The name of the highlight state.
	/// </summary>
	public const string HighlightStateName = "Highlight";

	/// <summary>
	/// The name of the un-highlight state.
	/// </summary>
	public const string UnHighlightStateName = "UnHighlight";
	
	///<summary>
	/// All the possible idle animations.
	/// </summary>
	public enum IdleAnimation {
		Wobble, Bounce, Pulse, None
	}

	/// <summary>
	/// The component animator.
	/// </summary>
	public Animator Animator;

	/// <summary>
	/// The idle animation.
	/// </summary>
	public IdleAnimation IdleAnim = IdleAnimation.Wobble;

	/// <summary>
	/// The time between idle animation repeats in seconds.
	/// </summary>
	public float IdleRepeatTime = 5f;

	/// <summary>
	/// Whether the component is highlightable.
	/// </summary>
	public bool IsHighlightable = true;

	/// <summary>
	/// The pressed.
	/// </summary>
	private bool pressed;
	
	/// <summary>
	/// Highlights this instance.
	/// </summary>
	public void Highlight () {
		if (!IsHighlightable)
			return;
		Animator.SetBool (HighlightBool, true);
		Animator.Play (HighlightStateName);
	}
	
	/// <summary>
	/// Un-Highlights this instance.
	/// </summary>
	public void UnHighlight () {
		if (!IsHighlightable)
			return;
		Animator.SetBool (HighlightBool, false);
		Animator.Play (UnHighlightStateName);
	}

	/// <summary>
	/// Wobbles the ui component to draw attention.
	/// </summary>
	public void Wobble () {
		Animator.SetTrigger (WobbleTrigger);
		Animator.Play (WobbleStateName);
	}

	/// <summary>
	/// Bounces the ui component to draw attention.
	/// </summary>
	public void Bounce () {
		Animator.SetTrigger (BounceTrigger);
		Animator.Play (BounceStateName);
	}

	/// <summary>
	/// Pulse this instance.
	/// </summary>
	public void Pulse () {
		Animator.SetTrigger (PulseTrigger);
		Animator.Play (PulseStateName);
	}

	/// <summary>
	/// The time accumulator;
	/// </summary>
	private float timeAccumulator = 0f;

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () {
		if (IdleAnim == IdleAnimation.None 
		    || Animator.GetBool (HighlightBool))
			return;

		// Run the idle animation every IdleRepeatTime seconds
		timeAccumulator += Time.deltaTime;
		if (timeAccumulator >= IdleRepeatTime) {
			timeAccumulator = 0f;
			switch (IdleAnim) {
				case IdleAnimation.Bounce : Bounce ();
					break;
				case IdleAnimation.Wobble : Wobble ();
					break;
				case IdleAnimation.Pulse : Pulse ();
					break;
			}
		}
	}

	/// <summary>
	/// Raises the pointer enter event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerEnter(PointerEventData eventData) {
		Highlight ();
	}

	/// <summary>
	/// Raises the pointer exit event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerExit(PointerEventData eventData) {
		if (!pressed)
			UnHighlight ();
	}
}
