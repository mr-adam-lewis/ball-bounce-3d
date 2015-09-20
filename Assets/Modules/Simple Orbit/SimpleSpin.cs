using UnityEngine;

/// <summary>
/// Spins the given spinning object around a given axis at a given revolutions per minute.
/// If the spinning object is not set, the game object to which this script is attached will spin.
/// If the axis is set to (0, 0, 0), the spinning object will spin around its up vector.
/// </summary>
public class SimpleSpin : MonoBehaviour {

	/// <summary>
	/// The time step of the rotation.
	/// </summary>
	public static float TimeStep = 1f;
	
	/// <summary>
	/// The constant 2 Pi.
	/// </summary>
	private const float PI2 = 2f * Mathf.PI;

	/// <summary>
	/// The game object to spin
	/// </summary>
	public GameObject SpinningObject;

	/// <summary>
	/// The revolutions per minute at which to spin the object;
	/// </summary>
	public float RevolutionsPerMinute;

	/// <summary>
	/// The axis in which to spin.
	/// </summary>
	public Vector3 Axis;

	/// <summary>
	/// The rotation space.
	/// </summary>
	public Space RotationSpace = Space.World;

	/// <summary>
	/// Initializes the script.
	/// </summary>
	void Start () {
		// Set the spinning object to the game object to which this script is attached if it is not set prior
		if (SpinningObject == null)
			SpinningObject = gameObject;

		// Set axis as the up vector of the spinning object if no axis is set prior
		if (Axis == Vector3.zero)
			Axis = SpinningObject.transform.up;
	}

	/// <summary>
	/// Updates the script.
	/// Called once per frame.
	/// </summary>
	void Update () {
		// Calculate the amount to rotate on this iteration
		float rotation = PI2 * Time.deltaTime * RevolutionsPerMinute * TimeStep;

		Vector3 axis = Axis * rotation;

		// Rotate the spinning object
		SpinningObject.transform.Rotate (axis.x, axis.y, axis.z, RotationSpace);
		//SpinningObject.transform.localRotation *= Quaternion.Euler (axis * rotation);
	}
}
