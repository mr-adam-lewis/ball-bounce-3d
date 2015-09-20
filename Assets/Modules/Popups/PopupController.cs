using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Controls all popups.
/// </summary>
public class PopupController : MonoBehaviour {

	/// <summary>
	/// The visible key.
	/// </summary>
	public const string VisibleKey = "isVisible";

	/// <summary>
	/// The show key.
	/// </summary>
	public const string ShowKey = "Show";

	/// <summary>
	/// The hide key.
	/// </summary>
	public const string HideKey = "Hide";

	/// <summary>
	/// The dimmer animator.
	/// </summary>
	public Animator DimmerAnimator;

	/// <summary>
	/// The popup animator.
	/// </summary>
	public Animator PopupAnimator;

	/// <summary>
	/// The dimmer.
	/// </summary>
	public GameObject Dimmer;

	/// <summary>
	/// The popup container.
	/// </summary>
	public GameObject PopupContainer;

	/// <summary>
	/// The popups.
	/// </summary>
	public GameObject[] Popups;

	/// <summary>
	/// The on show popup.
	/// </summary>
	public OnShowPopup OnShowPopup;

	/// <summary>
	/// The on hide popup.
	/// </summary>
	public OnHidePopup OnHidePopup;

	/// <summary>
	/// Starts the script.
	/// </summary>
	void Start () {
		PopupContainer.SetActive (false);
		Dimmer.SetActive (false);
	}
	
	/// <summary>
	/// Shows the popup container and all its active contents.
	/// </summary>
	private void Show () {
		PopupContainer.SetActive (true);
		PopupAnimator.SetBool (VisibleKey, true);
		PopupAnimator.Play (ShowKey);
		Dimmer.SetActive (true);
		DimmerAnimator.SetBool (VisibleKey, true);
		DimmerAnimator.Play (ShowKey);
		if (OnShowPopup != null)
			OnShowPopup.Invoke ();
	}
	
	/// <summary>
	/// Hides the popup container and all its active contents.
	/// </summary>
	public void Hide () {
		PopupAnimator.SetBool (VisibleKey, false);
		PopupAnimator.Play (HideKey);
		DimmerAnimator.SetBool (VisibleKey, false);
		DimmerAnimator.Play (HideKey);
		StartCoroutine (DeactivateCanvasAfterTime (PopupAnimator.GetCurrentAnimatorStateInfo (0).length));
		if (OnHidePopup != null)
			OnHidePopup.Invoke ();
	}
	
	/// <summary>
	/// Deactivates the canvas after time.
	/// </summary>
	/// <returns>The canvas after time.</returns>
	/// <param name="seconds">Seconds.</param>
	private IEnumerator DeactivateCanvasAfterTime (float seconds) {
		yield return new WaitForSeconds (seconds);
		DisableAllPopups ();
		PopupContainer.SetActive (false);
		Dimmer.SetActive (false);
	}
	
	/// <summary>
	/// Disables all popups.
	/// </summary>
	private void DisableAllPopups () {
		foreach (GameObject popup in Popups)
			popup.SetActive (false);
	}

	/// <summary>
	/// Shows the popup with the given index.
	/// </summary>
	/// <param name="index">Index.</param>
	public void ShowPopup (int index) {
		foreach (GameObject popup in Popups)
			if (popup.activeSelf) {
				Hide ();
                ShowPopup(index, PopupAnimator.GetCurrentAnimatorStateInfo(0).length);
				return;
            }
        DisableAllPopups();
        Popups[index].SetActive(true);
        Show();
	}

	/// <summary>
	/// Shows the popup with the given index after the given delay in seconds.
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="seconds">The delay in seconds.</param>
	public void ShowPopup(int index, float seconds) {
		StartCoroutine (ShowPopupAfterTime (index, seconds));
	}

	/// <summary>
	/// Shows the popup after time.
	/// </summary>
	/// <returns>The popup after time.</returns>
	/// <param name="index">Index.</param>
	/// <param name="seconds">Seconds.</param>
	private IEnumerator ShowPopupAfterTime (int index, float seconds) {
		yield return new WaitForSeconds (seconds);
        yield return new WaitForEndOfFrame();
		ShowPopup (index);
	}

}

/// <summary>
/// On select item for a selector.
/// </summary>
[System.Serializable]
public class OnShowPopup : UnityEvent {}

/// <summary>
/// On select item for a selector.
/// </summary>
[System.Serializable]
public class OnHidePopup : UnityEvent {}