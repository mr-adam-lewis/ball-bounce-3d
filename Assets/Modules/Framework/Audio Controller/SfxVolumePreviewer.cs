using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Sfx volume previewer.
/// </summary>
public class SfxVolumePreviewer : MonoBehaviour, IPointerUpHandler {

	/// <summary>
	/// The audio controller.
	/// </summary>
	public AudioController Controller;

	/// <summary>
	/// Raises the pointer up event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerUp (PointerEventData eventData) {
		Controller.PreviewSfxVolume ();
	}
}