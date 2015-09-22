using UnityEngine;
using System.Collections;

/// <summary>
/// This is the game controller, it handles all the game logic.
/// </summary>
public class GameController : MonoBehaviour {

    /// <summary>
    /// This is the Popup controlller.
    /// </summary>
    public PopupController Popups;

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
    /// <summary>
    /// This Method is called when the ball collides with an object
    /// </summary>
    /// <param name="obj"></param>
    public void OnBallCollide(GameObject obj)
    {
        if (obj == Floor)
        {
            // This is end of game
            Popups.ShowPopup(2);
        }

    }
    /// <summary>
    /// This is the called when the ball is clicked
    /// </summary>
    public void OnBallClick()
    {

        // This is what will happen when the ball is clicked
    }

    
}
