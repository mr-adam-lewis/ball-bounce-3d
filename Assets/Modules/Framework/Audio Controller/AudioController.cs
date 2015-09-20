using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// The audio controller.
/// </summary>
public class AudioController : MonoBehaviour {

	/// <summary>
	/// The music volume slider.
	/// </summary>
	public Slider MusicVolumeSlider;

	/// <summary>
	/// The sound effects volume slider.
	/// </summary>
	public Slider SfxVolumeSlider;

	/// <summary>
	/// The music volume player prefs key.
	/// </summary>
	private const string MusicVolumePlayerPrefsKey = "MusicVolume";

	/// <summary>
	/// The sfx volume player prefs key.
	/// </summary>
	private const string SfxVolumePlayerPrefsKey = "SfxVolume";

	/// <summary>
	/// The first launch player prefs key.
	/// </summary>
	private const string FirstLaunchPlayerPrefsKey = "AudioControllerFirstLaunch";

	/// <summary>
	/// The music.
	/// </summary>
	private AudioSource music;

	/// <summary>
	/// The sfx.
	/// </summary>
	private Dictionary <string, AudioSource> sfx;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
		// Get the music audio source
		music = GetComponent <AudioSource> ();

		// Initialize the sfx dictionary
		sfx = new Dictionary<string, AudioSource> ();

		// Get all sfx audio sources
		foreach (Transform child in transform) {
			AudioSource audio = child.GetComponent <AudioSource> ();
			if (audio != null)
				sfx.Add (child.name, audio);
		}

		// Check for first launch and initialize if so
		if (PlayerPrefs.GetInt (FirstLaunchPlayerPrefsKey) == 0) {
			// First Launch
			PlayerPrefs.SetInt (FirstLaunchPlayerPrefsKey, 1);

			// Set music volume
			PlayerPrefs.SetFloat (MusicVolumePlayerPrefsKey, 1f);

			// Set sfx volume
			PlayerPrefs.SetFloat (SfxVolumePlayerPrefsKey, 1f);

			// Set the music volume
			music.volume = 1f;
			
			// Set the sfx volume
			foreach (AudioSource audio in sfx.Values)
				audio.volume = 1f;
		} else {
			// Not First Launch

			// Set the music volume
			music.volume = PlayerPrefs.GetFloat (MusicVolumePlayerPrefsKey);

			// Get the sfx volume
			float sfxVol = PlayerPrefs.GetFloat (SfxVolumePlayerPrefsKey);

			// Set the sfx volume
			foreach (AudioSource audio in sfx.Values)
				audio.volume = sfxVol;
		}

		if (MusicVolumeSlider != null) {
			// Set slider position based on saved volume
			MusicVolumeSlider.value = GetMusicVolume ();

			// Add a listener to change the volume
			MusicVolumeSlider.onValueChanged.AddListener (SetMusicVolume);
		}

		if (SfxVolumeSlider != null) {
			// Set slider position based on saved volume
			SfxVolumeSlider.value = GetSfxVolume ();

			// Add a listener to change the volume
			SfxVolumeSlider.onValueChanged.AddListener (SetSfxVolume);

			// Add the sfx volume previewer to the handle
			SfxVolumePreviewer previewer = SfxVolumeSlider.gameObject.AddComponent <SfxVolumePreviewer> ();
			previewer.Controller = this;
		}
	}

    /// <summary>
    /// Play the sound effect with the specified name.
    /// </summary>
    /// <param name="name">Name.</param>
    public void PlaySfx(string name) {
        // Attempt to get audio source
        AudioSource audio;
        sfx.TryGetValue(name, out audio);

        // Play the audio source or log message
        if (audio != null)
            audio.Play();
        else
            Debug.LogError("AudioSource object of name '" + name + "' does not exist within this audio controller.");
    }

    /// <summary>
    /// Play the sound effect with the specified name after the given delay.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="delay">Delay in seconds.</param>
    public void PlaySfx(string name, float delay) {
        StartCoroutine(PlaySfxAfterDelay(name, delay));
    }

    /// <summary>
    /// Plays the sound effect with the specified name after the given delay.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="delay">Delay in seconds.</param>
    /// <returns></returns>
    private IEnumerator PlaySfxAfterDelay(string name, float delay) {
        yield return new WaitForSeconds(delay);
        PlaySfx(name);
    }

	/// <summary>
	/// Stops the sound effect with the specified name.
	/// </summary>
	/// <param name="name">Name.</param>
	public void StopSfx (string name) {
		// Attempt to get audio source
		AudioSource audio;
		sfx.TryGetValue (name, out audio);
		
		// Stop the audio source or log message
		if (audio != null)
			audio.Stop ();
		else
			Debug.LogError ("AudioSource object of name '" + name + "' does not exist within this audio controller.");
	}

	/// <summary>
	/// Plays the music.
	/// </summary>
	public void PlayMusic () {
		// Check if music exists and log message if it does not
		if (music == null) {
			Debug.LogError ("The music audio source is not attached to the audio controller's game object.");
			return;
		}
		music.Play ();
	}

	/// <summary>
	/// Pauses the music.
	/// </summary>
	public void PauseMusic () {
		// Check if music exists and log message if it does not
		if (music == null) {
			Debug.LogError ("The music audio source is not attached to the audio controller's game object.");
			return;
		}
		music.Pause ();
	}

	/// <summary>
	/// Stops the music.
	/// </summary>
	public void StopMusic () {
		// Check if music exists and log message if it does not
		if (music == null) {
			Debug.LogError ("The music audio source is not attached to the audio controller's game object.");
			return;
		}
		music.Stop ();
	}

	/// <summary>
	/// Sets the music volume.
	/// </summary>
	/// <param name="volume">Volume.</param>
	public void SetMusicVolume (float volume) {
		// Check if music exists and log message if it does not
		if (music == null) {
			Debug.LogError ("The music audio source is not attached to the audio controller's game object.");
			return;
		}
		music.volume = volume;
		SaveVolumesToPlayerPrefs ();
	}

	/// <summary>
	/// Gets the music volume.
	/// </summary>
	/// <returns>The music volume.</returns>
	public float GetMusicVolume () {
		return music.volume;
	}

	/// <summary>
	/// Sets the sound effects volume.
	/// </summary>
	/// <param name="volume">Volume.</param>
	public void SetSfxVolume (float volume) {
		// Set the sfx volume for each audio source
		foreach (AudioSource audio in sfx.Values) {
			audio.volume = volume;
		}
		SaveVolumesToPlayerPrefs ();
	}

	/// <summary>
	/// Gets the sound effects volume.
	/// </summary>
	/// <returns>The sfx volume.</returns>
	public float GetSfxVolume () {
		// Return 0 if no sfx
		if (sfx.Values.Count == 0)
			return 0f;

		IEnumerator <AudioSource> enumerator = sfx.Values.GetEnumerator ();
		enumerator.MoveNext ();

		// Get first sfx
		AudioSource audio = enumerator.Current;

		// Return 0 if no sfx
		if (audio == null)
			return 0f;

		// Return the volume
		return audio.volume;
	}

	/// <summary>
	/// Saves the volumes to player prefs.
	/// </summary>
	public void SaveVolumesToPlayerPrefs () {
		PlayerPrefs.SetFloat (MusicVolumePlayerPrefsKey, GetMusicVolume ());
		PlayerPrefs.SetFloat (SfxVolumePlayerPrefsKey, GetSfxVolume ());
	}

	/// <summary>
	/// Previews the sfx volume.
	/// </summary>
	public void PreviewSfxVolume () {
		// Return if no sfx
		if (sfx.Values.Count == 0)
			return;

		// Get first sfx
		IEnumerator <AudioSource> enumerator = sfx.Values.GetEnumerator ();
		enumerator.MoveNext ();
		
		// Get first sfx
		AudioSource audio = enumerator.Current;
		
		// Return if no sfx
		if (audio == null)
			return;

		// Set volume and play
		if (SfxVolumeSlider != null)
			audio.volume = SfxVolumeSlider.value;
		audio.Play ();
	}

}
