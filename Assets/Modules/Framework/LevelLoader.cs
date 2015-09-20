using UnityEngine;
using System.Collections;

/// <summary>
/// Loads levels with a progress bar asynchronously.
/// </summary>
public class LevelLoader : MonoBehaviour {

    /// <summary>
    /// The scene controller.
    /// </summary>
    public SceneController SceneController;

    /// <summary>
    /// The progress bar.
    /// </summary>
    public ProgressBar ProgressBar;

    /// <summary>
    /// Loads the given level.
    /// </summary>
    /// <param name="levelIndex">The level index.</param>
    public void LoadLevel(int levelIndex) {
        SceneController.LoadLevel(levelIndex, ProgressBar, true);
    }
}
