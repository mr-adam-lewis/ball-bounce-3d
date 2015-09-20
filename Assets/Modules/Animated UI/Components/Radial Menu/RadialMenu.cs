using UnityEngine;
using System.Collections;

/// <summary>
/// A radial menu.
/// </summary>
public class RadialMenu : MonoBehaviour { 

	/// <summary>
	/// The expand distance.
	/// </summary>
	public float ExpandDistance = 100f;

	/// <summary>
	/// From degrees.
	/// </summary>
	public float FromDegrees = 0f;

	/// <summary>
	/// To degrees.
	/// </summary>
	public float ToDegrees = 360f;

	/// <summary>
	/// The duration of expand.
	/// </summary>
	public float DurationOfExpand = 0.2f;
	
	/// <summary>
	/// Whether to start with the menu expanded.
	/// </summary>
	public bool StartExpanded = false;

	/// <summary>
	/// The components.
	/// </summary>
	public RectTransform[] Components;

	/// <summary>
	/// The destinations.
	/// </summary>
	private Vector2[] destinations;

	/// <summary>
	/// The expanded.
	/// </summary>
	private bool expanded;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		if (StartExpanded) {
			SetDestinations ();
			expanded = true;
			AppearAtDestinations ();
		}
	}

	/// <summary>
	/// Sets the destinations.
	/// </summary>
	private void SetDestinations () {
		// Instantiate destinations
		destinations = new Vector2[Components.Length];

        float divisor = destinations.Length - 1f;
        if (Mathf.Approximately(Mathf.Abs(FromDegrees - ToDegrees) % 360f, 0f)
            || Mathf.Approximately(Mathf.Abs(FromDegrees - ToDegrees) % 360f, 360f))
            divisor = destinations.Length;
		
		// Loop through setting destinations
		for (int i=0; i<destinations.Length; i++) {
			float angle = FromDegrees + (float) (i * (ToDegrees - FromDegrees)) / (float) divisor;
			float x = ExpandDistance * Mathf.Cos (Mathf.Deg2Rad * angle);
			float y = ExpandDistance * Mathf.Sin (Mathf.Deg2Rad * angle);
			destinations[i] = new Vector2 (x, y);
		}
	}

	/// <summary>
	/// Expand the menu.
	/// </summary>
	public void Expand () {
		// Set the destinations
		SetDestinations ();

		expanded = true;

		// Animate
		StartCoroutine (AnimateToDestinations ());
	}

	/// <summary>
	/// Unexpand the menu.
	/// </summary>
	public void UnExpand () {
		if (destinations == null)
			return;

		expanded = false;

		// Animate
		StartCoroutine (AnimateToDestinations ());
	}

	/// <summary>
	/// Toggles whether the radial menu is expanded.
	/// </summary>
	public void Toggle () {
		if (expanded)
			UnExpand ();
		else
			Expand ();
	}

	/// <summary>
	/// All components appear at destinations.
	/// </summary>
	private void AppearAtDestinations () {
		// Loop through setting destinations
		for (int i=0; i<Components.Length; i++) {
			RectTransform rect = Components[i];
			// Set to destination
			if (expanded)
				rect.anchoredPosition = destinations[i];
			else
				rect.anchoredPosition = Vector2.zero;
		}
	}

	/// <summary>
	/// Animates to destinations.
	/// </summary>
	/// <returns>The to destinations.</returns>
	private IEnumerator AnimateToDestinations () {
		float frames = DurationOfExpand * 60f;
		float duration = DurationOfExpand / frames;
		for (float i=0f; i<frames; i++) {
			yield return new WaitForSeconds (duration);
			for (int j=0; j<Components.Length; j++) {
				RectTransform rect = Components[j];
				rect.anchoredPosition = Vector2.Lerp (Vector2.zero, destinations[j], i / frames);
				if (!expanded)
					rect.anchoredPosition = destinations[j] - rect.anchoredPosition;
			}
		}
	}

}