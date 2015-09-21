using UnityEngine;
using System.Collections;

/// <summary>
/// This is the game controller, it handles all the game logic.
/// </summary>
public class GameControler : MonoBehaviour {

    /// <summary>
    /// The floor game object.
    /// </summary>
    public GameObject Floor;

    /// <summary>
    /// This is the ball game object.
    /// </summary>
    public GameObject Ball;

	/// <summary>
    /// Starts the script.
    /// </summary>
	void Start () {
	    


	}
	
    /// <summary>
    /// This updates the script,and is called once per frame.
    /// </summary>
	void Update () {
	
	}
    public void OnBallCollide(GameObject obj)
    {
        if (obj == Ball)
        {
            // This is end of game
        }

    }

    
}
