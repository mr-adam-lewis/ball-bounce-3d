using UnityEngine;
using System.Collections;

/// <summary>
/// This is the script attached to the ball and controls the ball.
/// </summary>
public class Ball : MonoBehaviour {

    /// <summary>
    /// This is the game controller.
    /// </summary>
    public GameController Game;

    /// <summary>
    /// Raises the on mouse over event
    /// </summary>
    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
            Game.OnBallClick();
    }

    /// <summary>
    /// Raises the on collision enter event
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter (Collision collision)
    {
        Game.OnBallCollide(collision.collider.gameObject);
    }



}
