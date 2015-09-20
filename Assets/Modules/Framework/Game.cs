using UnityEngine;
using System.Collections;

/// <summary>
/// The game controller.
/// </summary>
public class Game : MonoBehaviour {

    /// <summary>
    /// The in-game delta time.
    /// </summary>
    public static float DeltaTime;

    /// <summary>
    /// Whether the game is paused or not.
    /// </summary>
    public static bool Paused;

    /// <summary>
    /// The audio controller.
    /// </summary>
    public AudioController AudioController;

    /// <summary>
    /// The scene controller.
    /// </summary>
    public SceneController SceneController;

    /// <summary>
    /// Starts the instance
    /// </summary>
	void Start () {
        Paused = false;
	}
	
    /// <summary>
    /// Updates the instance
    /// </summary>
	void Update () {
        if (Paused)
            DeltaTime = 0;
        else
            DeltaTime = Time.deltaTime;
	}
}
