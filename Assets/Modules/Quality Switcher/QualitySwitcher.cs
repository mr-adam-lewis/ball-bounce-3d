using UnityEngine;
using System.Collections;

/// <summary>
/// Quality switcher.
/// </summary>
public class QualitySwitcher : MonoBehaviour {

	/// <summary>
	/// The first launch player prefs key.
	/// </summary>
	private const string FirstLaunchPlayerPrefsKey = "QualitySwitcherFirstLaunch";

	/// <summary>
	/// The quality player prefs key.
	/// </summary>
	public const string QualityPlayerPrefsKey = "Quality";

	/// <summary>
	/// The selector.
	/// </summary>
	public Selector Selector;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		// Handle first launch
		if (PlayerPrefs.GetInt (FirstLaunchPlayerPrefsKey) == 0) {
			PlayerPrefs.SetInt (FirstLaunchPlayerPrefsKey, 1);
			PlayerPrefs.SetInt (QualityPlayerPrefsKey, 0);
		}

		// Get saved quality level
		int quality = PlayerPrefs.GetInt (QualityPlayerPrefsKey);

		// Set selector values
		Selector.Options = new string[] {"Low", "Medium", "High"};
		Selector.DefaultOptionIndex = quality;
		Selector.PopulateList ();
		Selector.SetSelectedItemNoListeners (Selector.DefaultOptionIndex);

		// Add listener to switch the quality
		Selector.OnSelect.AddListener (SwitchQuality);

        SwitchQuality(quality);
	}

	/// <summary>
	/// Switchs the quality.
	/// </summary>
	/// <param name="index">Index.</param>
	public void SwitchQuality (int index) {
		QualitySettings.SetQualityLevel (index);
        AdjustScreenResolution(index);
		PlayerPrefs.SetInt (QualityPlayerPrefsKey, index);
	}

    /// <summary>
    /// Adjusts the screen resolution to improve frame rates.
    /// <param name="index">The quality index</param>
    /// </summary>
    public void AdjustScreenResolution(int index)
    {
        float targetWidth = 800;

        float ratio = (float)Screen.resolutions[0].height / (float)Screen.resolutions[0].width;

        switch (index)
        {
            case 0: targetWidth = Mathf.Max (Screen.resolutions[0].width / 4f, 800);
                break;
            case 1: targetWidth = Mathf.Max (Screen.resolutions[0].width / 2f, 1200);
                break;
            case 2: targetWidth = Screen.resolutions[0].width;
                break;
        }

        float targetHeight = ratio * targetWidth;

        Screen.SetResolution((int)targetWidth, (int)targetHeight, true);
    }
}
