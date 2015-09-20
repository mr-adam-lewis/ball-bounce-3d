using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// A progress bar ui component.
/// </summary>
public class ProgressBar : MonoBehaviour {

    /// <summary>
    /// The background.
    /// </summary>
    public RectTransform Background;

    /// <summary>
    /// The fill.
    /// </summary>
    public RectTransform Fill;

    /// <summary>
    /// The on change listener.
    /// </summary>
    public OnChange OnChange;

    /// <summary>
    /// The on complete listener.
    /// </summary>
    public OnComplete OnComplete;

    /// <summary>
    /// The progress of the bar.
    /// </summary>
    private float progress;

    /// <summary>
    /// Gets the progress.
    /// </summary>
    /// <returns>The progress.</returns>
    public float GetProgress() {
        return progress;
    }

    /// <summary>
    /// Sets the progress.
    /// </summary>
    /// <param name="progress">The progress between 0 and 1 (will clamp).</param>
    public void SetProgress(float progress) {
        if (progress < 0f)
            progress = 0f;
        if (progress > 1f)
            progress = 1f;

        this.progress = progress;

        // Adjust the anchor max
        Fill.anchorMax = new Vector2(GetProgress (), Fill.anchorMax.y);

        if (OnChange != null)
            OnChange.Invoke(GetProgress ());

        if (progress == 1f
            && OnComplete != null)
            OnComplete.Invoke();
    }


}

/// <summary>
/// The on complete listener for the progress bar.
/// </summary>
public class OnComplete : UnityEvent {}

/// <summary>
/// The on change listener for the progress bar.
/// Feeds back the progress.
/// </summary>
public class OnChange : UnityEvent <float> {}