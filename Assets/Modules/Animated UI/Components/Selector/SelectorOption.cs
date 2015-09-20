using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// An option in a selector.
/// </summary>
public class SelectorOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	/// <summary>
	/// The parent selector.
	/// </summary>
	public Selector Parent;

	/// <summary>
	/// The index.
	/// </summary>
	public int Index;

	/// <summary>
	/// The label.
	/// </summary>
	public Text Label;

	/// <summary>
	/// The background.
	/// </summary>
	public Image Background;

	/// <summary>
	/// Select this instance.
	/// </summary>
	public void Select () {
		Background.color = Parent.ListItemColorHighlight;
		Parent.SetSelectedItem (this);
		Parent.OptionsContainerBackground.gameObject.SetActive (false);
	}

	/// <summary>
	/// Sets the color of the background.
	/// </summary>
	public void SetBackgroundColor () {
		Background = GetComponent <Image> ();
		if (Index % 2 == 0)
			Background.color = Parent.ListItemColorEven;
		else
			Background.color = Parent.ListItemColorOdd;
	}

	/// <summary>
	/// Raises the pointer enter event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerEnter (PointerEventData eventData) {
		if (Parent.IsHighlightable)
			Background.color = Parent.ListItemColorHighlight;
	}

	/// <summary>
	/// Raises the pointer exit event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerExit (PointerEventData eventData) {
		SetBackgroundColor ();
	}

	/// <summary>
	/// Raises the pointer click event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData) {
		Select ();
	}
}
