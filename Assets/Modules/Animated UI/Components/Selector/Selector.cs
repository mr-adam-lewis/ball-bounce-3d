using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// A Selector for selecting objects in game.
/// </summary>
public class Selector : MonoBehaviour, IPointerClickHandler {

	/// <summary>
	/// The options container.
	/// </summary>
	public RectTransform OptionsContainer;

	/// <summary>
	/// The options container background.
	/// </summary>
	public RectTransform OptionsContainerBackground;

	/// <summary>
	/// The option prefab.
	/// </summary>
	public GameObject OptionPrefab;

	/// <summary>
	/// The selection text.
	/// </summary>
	public Text SelectionText;

	/// <summary>
	/// The options.
	/// </summary>
	public string[] Options;

	/// <summary>
	/// The default option index.
	/// </summary>
	public int DefaultOptionIndex;

	/// <summary>
	/// The odd list item color.
	/// </summary>
	public Color ListItemColorOdd = Color.white;

	/// <summary>
	/// The even list item color.
	/// </summary>
	public Color ListItemColorEven = new Color (0.9f, 0.9f, 0.9f);

	/// <summary>
	/// The list item color highlight.
	/// </summary>
	public Color ListItemColorHighlight = new Color (77f / 255f, 126 / 255f, 1f);

	/// <summary>
	/// Whether the list items are highlightable.
	/// </summary>
	public bool IsHighlightable = true;

	/// <summary>
	/// The on select.
	/// </summary>
	public OnSelect OnSelect;

	/// <summary>
	/// The selected.
	/// </summary>
	private int selected;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		PopulateList ();
		SetSelectedItemNoListeners (DefaultOptionIndex);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () {
		Input.simulateMouseWithTouches = true;

		// If clicked
		if (Input.GetMouseButtonUp (0) && !OptionsContainerBackground.gameObject.activeSelf)
			OptionsContainerBackground.gameObject.SetActive (false);
	}

	/// <summary>
	/// Populates the list with the options.
	/// </summary>
	public void PopulateList () {
		// Destroy current list items
		foreach (Transform child in OptionsContainer)
			Destroy (child.gameObject);

		// Set the size of the container to 0
		OptionsContainer.sizeDelta = new Vector2(OptionsContainer.sizeDelta.x, 0);

		// For all the options, create a gameobject for them
		for (int i=0; i<Options.Length; i++) {
			// Instantiate the game object and get the selector option component
			SelectorOption option = Instantiate (OptionPrefab).GetComponent <SelectorOption> ();

			// Put the option within the container
			option.transform.SetParent (OptionsContainer.transform);

			// Get the rect transform from the option
			RectTransform rect = option.GetComponent <RectTransform> ();

			// Set the option text
			option.Label.text = Options[i];

			// Extend the container
			OptionsContainer.sizeDelta = new Vector2 (OptionsContainer.sizeDelta.x, OptionsContainer.sizeDelta.y + rect.rect.height);

			// Set the position
			rect.localScale = Vector3.one;
			rect.anchoredPosition = new Vector2 (rect.anchoredPosition.x, -i * rect.rect.height);
			rect.anchoredPosition3D = new Vector3 (rect.anchoredPosition.x, rect.anchoredPosition.y, 0);
			rect.offsetMax = new Vector2 (0, rect.offsetMax.y);
			rect.offsetMin = new Vector2 (0, rect.offsetMin.y);

			// Set the background
			option.Parent = this;
			option.Index = i;
			option.SetBackgroundColor ();
		}
	}

	/// <summary>
	/// Sets the selected item.
	/// </summary>
	/// <param name="option">Option.</param>
	public void SetSelectedItem (SelectorOption option) {
		if (SelectionText.text != Options[option.Index]) {
			SetSelectedItem (option.Index);
			if (OnSelect != null)
				OnSelect.Invoke (option.Index);
		}
	}
	
	/// <summary>
	/// Sets the selected item.
	/// </summary>
	/// <param name="index">Index.</param>
	public void SetSelectedItem (int index) {
		SelectionText.text = Options [index];
		selected = index;
		if (OnSelect != null)
			OnSelect.Invoke (index);
	}
	
	/// <summary>
	/// Sets the selected item without invoking listeners.
	/// </summary>
	/// <param name="index">Index.</param>
	public void SetSelectedItemNoListeners (int index) {
		SelectionText.text = Options [index];
		selected = index;
	}

	/// <summary>
	/// Gets the index of the selected.
	/// </summary>
	/// <returns>The selected index.</returns>
	public int GetSelectedIndex () {
		return selected;
	}

	/// <summary>
	/// Raises the pointer click event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData) {
		if (!OptionsContainerBackground.gameObject.activeSelf) {
			OptionsContainerBackground.gameObject.SetActive (true);
			foreach (Transform child in OptionsContainer.transform) {
				SelectorOption option = child.GetComponent <SelectorOption> ();
				if (option != null)
					option.SetBackgroundColor ();
			}
		}
	}
}

/// <summary>
/// On select item for a selector.
/// </summary>
[System.Serializable]
public class OnSelect : UnityEvent<int> {}