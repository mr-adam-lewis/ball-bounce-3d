using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// A switch that can toggle things.
/// </summary>
public class Switch : MonoBehaviour, IPointerClickHandler {

	/// <summary>
	/// The handle.
	/// </summary>
	public SwitchHandle Handle;
	
	/// <summary>
	/// The duration of the switching animation.
	/// </summary>
	public float DurationOfSwitch = 0.1f;

	/// <summary>
	/// The on switch.
	/// </summary>
	public OnSwitch OnSwitch;

	/// <summary>
	/// The is on.
	/// </summary>
	private bool isOn;

	/// <summary>
	/// Determines whether this instance is switched on.
	/// </summary>
	/// <returns><c>true</c> if this instance is switched on; otherwise, <c>false</c>.</returns>
	public bool IsSwitchedOn () {
		return isOn;
	}

	/// <summary>
	/// Sets the switched on.
	/// </summary>
	/// <param name="isOn">If set to <c>true</c> is on.</param>
	public void SetSwitchedOn (bool isOn) {
		this.isOn = isOn;
		OnSwitch.Invoke (isOn);
		Handle.Animate ();
	}

	/// <summary>
	/// Sets the switched on without invoking listeners.
	/// </summary>
	/// <param name="isOn">If set to <c>true</c> is on.</param>
	public void SetSwitchedOnNoListeners (bool isOn) {
		this.isOn = isOn;
		Handle.Animate ();
	}

	/// <summary>
	/// Toggles the switch.
	/// </summary>
	public void Toggle () {
		SetSwitchedOn (!isOn);
	}

	/// <summary>
	/// Raises the pointer click event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData) {
		Toggle ();
	}

}

/// <summary>
/// On switch event for a switch.
/// </summary>
[System.Serializable]
public class OnSwitch : UnityEvent<bool> {}