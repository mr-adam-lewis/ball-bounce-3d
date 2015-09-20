using UnityEngine;
using System.Collections;

/// <summary>
/// A scene controller has helper methods for controlling the app through the unity editor.
/// </summary>
public class SceneController : MonoBehaviour {

	/// <summary>
	/// The minimum load time.
	/// </summary>
	public float MinimumLoadTime = 2f;

	/// <summary>
	/// The level load operation.
	/// </summary>
	private AsyncOperation levelLoadOperation;

	/// <summary>
	/// The level load progress.
	/// </summary>
	private float levelLoadProgress;

	/// <summary>
	/// The allow scene activation.
	/// </summary>
	private bool allowSceneActivation;

	/// <summary>
	/// Loads the level.
	/// </summary>
	/// <param name="levelIndex">Level index.</param>
	public void LoadLevel (int levelIndex) {
		Application.LoadLevel (levelIndex);
	}

    /// <summary>
    /// Loads the level with the given level index and updates the given progress bar.
    /// </summary>
    /// <param name="levelIndex">The level index.</param>
    /// <param name="progressbar">The progress bar.</param>
    /// <param name="allowSceneActivation">Whether or not to activate the scene when loading completes.</param>
    public void LoadLevel(int levelIndex, ProgressBar progressbar, bool allowSceneActivation) {
        LoadLevelAsync (levelIndex, progressbar, allowSceneActivation);
    }

    /// <summary>
    /// Loads the level asynchronously.
    /// </summary>
    /// <param name="levelIndex">Level index.</param>
    /// <param name="allowSceneActivation">Whether or not to activate the scene immediately when loaded.</param>
    public void LoadLevelAsync(int levelIndex, bool allowSceneActivation) {
        LoadLevelAsync(levelIndex, null, allowSceneActivation);
    }

    /// <summary>
    /// Loads the level asynchronously.
    /// </summary>
    /// <param name="levelIndex">Level index.</param>
    /// <param name="progressbar">The progress bar.</param>
    /// <param name="allowSceneActivation">Whether or not to activate the scene immediately when loaded.</param>
    private void LoadLevelAsync(int levelIndex, ProgressBar progressbar, bool allowSceneActivation) {
        levelLoadOperation = Application.LoadLevelAsync(levelIndex);
        levelLoadOperation.allowSceneActivation = false;
        this.allowSceneActivation = allowSceneActivation;
        StartCoroutine(IncreaseProgress());
        if (progressbar != null)
            StartCoroutine(UpdateProgressBar(progressbar));
    }

	/// <summary>
	/// Gets the level load progress.
	/// </summary>
	/// <returns>The level load progress.</returns>
	public float GetLevelLoadProgress () {
		float progress = Mathf.Min (levelLoadOperation.progress + 0.2f, levelLoadProgress);
        if (progress > 1f)
            progress = 1f;
        return progress;
	}

	/// <summary>
	/// Advances to loaded level.
	/// </summary>
	public void AdvanceToLoadedLevel () {
		if (levelLoadOperation == null)
			return;
		levelLoadOperation.allowSceneActivation = true;
	}

    /// <summary>
    /// Increases the progress.
    /// </summary>
    private IEnumerator IncreaseProgress() {
        float delay = MinimumLoadTime / 100f;
        for (float i = 0; i < 100f; i++) {
            yield return new WaitForSeconds(delay);
            levelLoadProgress = i / 98.5f;
        }
        if (allowSceneActivation)
            levelLoadOperation.allowSceneActivation = true;
    }

    /// <summary>
    /// Updates the progress bar.
    /// </summary>
    /// <param name="progressbar">The progress bar.</param>
    private IEnumerator UpdateProgressBar(ProgressBar progressbar) {
        while (progressbar != null
            && GetLevelLoadProgress () < 1f) {
                progressbar.SetProgress(GetLevelLoadProgress());
            yield return new WaitForEndOfFrame();
        }
    }

	/// <summary>
	/// Opens the URL.
	/// </summary>
	/// <param name="url">URL.</param>
	public void OpenURL (string url) {
		Application.OpenURL (url);
	}

	/// <summary>
	/// Exits the app.
	/// </summary>
	public void ExitApp () {
		Application.Quit ();
	}
}
