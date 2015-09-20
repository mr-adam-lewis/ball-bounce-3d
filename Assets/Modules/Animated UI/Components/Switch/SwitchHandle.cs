using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// The handle of a switch
/// </summary>
public class SwitchHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	/// <summary>
	/// The parent switch.
	/// </summary>
	public Switch Parent;

	/// <summary>
	/// The switch container.
	/// </summary>
	public RectTransform SwitchContainer;
	
	/// <summary>
	/// The handle.
	/// </summary>
	public RectTransform Handle;
	
	/// <summary>
	/// The fill area.
	/// </summary>
	public RectTransform Fill;

	/// <summary>
	/// The original position of the handle.
	/// </summary>
	private Vector2 originalPosition;

	/// <summary>
	/// The dragging.
	/// </summary>
	private bool dragging;

	/// <summary>
	/// Raises the begin drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnBeginDrag (PointerEventData eventData) {
		originalPosition = Handle.anchoredPosition;
		dragging = true;
	}
	
	/// <summary>
	/// Raises the drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnDrag (PointerEventData eventData) {
		float xMax = SwitchContainer.rect.width - Handle.rect.width;
		Vector3 displacement = originalPosition + eventData.position - eventData.pressPosition;
		Handle.anchoredPosition = new Vector2 (Mathf.Clamp (displacement.x, 0, xMax), Handle.anchoredPosition.y);
		Fill.offsetMax = new Vector2 (Handle.anchoredPosition.x + 1, Fill.offsetMax.y);
	}
	
	/// <summary>
	/// Raises the end drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnEndDrag (PointerEventData eventData) {
		float ratio = Handle.anchoredPosition.x / (SwitchContainer.rect.width - Handle.rect.width);
		Parent.SetSwitchedOn (ratio >= 0.5f);
		dragging = false;
		Animate ();
	}

	/// <summary>
	/// Animate the handle of the switch to the current switch position.
	/// </summary>
	public void Animate () {
		StartCoroutine (PerformAnimation ());
	}

	/// <summary>
	/// Performs the animation.
	/// </summary>
	private IEnumerator PerformAnimation () {
		// Initialize goals
		bool reachedGoal = false;
		float goal = 0f;

		// Calculate the increment
		float increment = (SwitchContainer.rect.width - Handle.rect.width) / Parent.DurationOfSwitch;
		increment /= 60f;

		for (int i=0; i<60; i++) {
			if (reachedGoal || dragging)
				break;

			// Delay to animate
			yield return new WaitForSeconds (0.01666666666f); // 1/60
			
			// Determine goal
			if (Parent.IsSwitchedOn ())
				goal = SwitchContainer.rect.width - Handle.rect.width;
			else
				goal = 0f;
			
			// Set the current positions
			if (Handle.anchoredPosition.x < goal)
				Handle.anchoredPosition = new Vector2 (Handle.anchoredPosition.x + increment, Handle.anchoredPosition.y);
			else
				Handle.anchoredPosition = new Vector2 (Handle.anchoredPosition.x - increment, Handle.anchoredPosition.y);
			Fill.offsetMax = new Vector2 (Handle.anchoredPosition.x + 1, Fill.offsetMax.y);
			
			// Determine whether reached goal position
			reachedGoal = Mathf.Abs(goal - Handle.anchoredPosition.x) <= increment;
		}

		// Set final position
		if (!dragging) {
			Handle.anchoredPosition = new Vector2 (goal, Handle.anchoredPosition.y);
			Fill.offsetMax = new Vector2 (Handle.anchoredPosition.x + 1, Fill.offsetMax.y);
		}
	}

}
