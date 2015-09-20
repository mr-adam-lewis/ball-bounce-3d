using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// An in-app purchase in a game.
/// </summary>
public class InAppPurchase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {

    /// <summary>
    /// The description container.
    /// </summary>
    public GameObject DescriptionContainer;

    /// <summary>
    /// The text component containing the price.
    /// </summary>
    public Text Price;

    /// <summary>
    /// The name of the in-app purchase.
    /// </summary>
    public string Name;

    /// <summary>
    /// Whether the user has purchased this in-app purchase.
    /// </summary>
    public bool Purchased;

    /// <summary>
    /// The on purchase event listener.
    /// </summary>
    public OnPurchase OnPurchase;

    /// <summary>
    /// Starts this instance
    /// </summary>
    void Start() {
        // Get the player prefs key for the in-app purchase
        int purchased = PlayerPrefs.GetInt(GeneratePlayerPrefsKey());

        // If previously achieved, set as achieved
        if (purchased != 0)
            Purchased = purchased == 2;
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
    /// Record that the in-app purchase has been purchased.
    /// </summary>
    public void Purchase() {
        PlayerPrefs.SetInt(GeneratePlayerPrefsKey(), 2);
        if (OnPurchase != null)
            OnPurchase.Invoke();
    }

    /// <summary>
    /// Record that the in-app purchase has been unpurchased.
    /// </summary>
    public void UnPurchase() {
        PlayerPrefs.SetInt(GeneratePlayerPrefsKey(), 1);
    }

    /// <summary>
    /// Whether the in-app purchase has been purchased or not.
    /// </summary>
    /// <returns></returns>
    public bool IsPurchased() {
        return Purchased;
    }

    /// <summary>
    /// Generates the player prefs key.
    /// </summary>
    /// <returns>The player prefs key.</returns>
    private string GeneratePlayerPrefsKey() {
        return "InAppPurchase-" + name;
    }

}

/// <summary>
/// The on purchase event listener.
/// </summary>
[System.Serializable]
public class OnPurchase : UnityEvent { }