using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// An achievement in a game.
/// </summary>
public class Achievement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {

    /// <summary>
    /// The description container.
    /// </summary>
    public GameObject DescriptionContainer;

    /// <summary>
    /// The fader.
    /// </summary>
    public GameObject Fader;

    /// <summary>
    /// The name of the achievement.
    /// </summary>
    public string Name;

    /// <summary>
    /// Whether the user has achieved this achievement.
    /// </summary>
    public bool Achieved;

    /// <summary>
    /// The on achieve event listener.
    /// </summary>
    public OnAchieve OnAchieve;

    /// <summary>
    /// Starts this instance
    /// </summary>
    void Start() {
        // Get the player prefs key for the achievement
        int achieved = PlayerPrefs.GetInt(GeneratePlayerPrefsKey());

        // If previously achieved, set as achieved
        if (achieved != 0)
            Achieved = achieved == 2;

        // Set the fader visibility
        Fader.SetActive(!Achieved);
    }

    /// <summary>
    /// Shows the description.
    /// </summary>
    public void ShowDescription() {
        DescriptionContainer.SetActive(true);
    }

    /// <summary>
    /// Hides the description.
    /// </summary>
    public void HideDescription() {
        DescriptionContainer.SetActive(false);
    }

    /// <summary>
    /// Raises the pointer down event.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        ShowDescription();
    }

    /// <summary>
    /// Raises the pointer up event.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        HideDescription();
    }

    /// <summary>
    /// Raises the pointer exit event.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        HideDescription();
    }
    
    /// <summary>
    /// Record that the achievement has been achieved.
    /// </summary>
    public void Achieve() {
        PlayerPrefs.SetInt(GeneratePlayerPrefsKey(), 2);
        if (OnAchieve != null)
            OnAchieve.Invoke();
    }

    /// <summary>
    /// Record that the achievement has been unachieved.
    /// </summary>
    public void UnAchieve() {
        PlayerPrefs.SetInt(GeneratePlayerPrefsKey(), 1);
    }

    /// <summary>
    /// Whether the achievement has been achieved or not.
    /// </summary>
    /// <returns></returns>
    public bool IsAchieved() {
        return Achieved;
    }

    /// <summary>
    /// Generates the player prefs key.
    /// </summary>
    /// <returns>The player prefs key.</returns>
    private string GeneratePlayerPrefsKey() {
        return "Achievement-" + name;
    }

}

/// <summary>
/// The on achieve event listener.
/// </summary>
[System.Serializable]
public class OnAchieve : UnityEvent { }