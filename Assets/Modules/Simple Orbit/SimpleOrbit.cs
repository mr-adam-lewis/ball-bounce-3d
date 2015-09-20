using UnityEngine;

/// <summary>
///  Makes the selected game object orbit another game object at a set speed (rpm).
///  The orbital inclination is calculated from the original positions of the objects.
///  Does not take physics in to account. Fixed orbit only.
/// </summary>
public class SimpleOrbit : MonoBehaviour {
	
	/// <summary>
	/// The time step of the rotation.
	/// </summary>
	public static float TimeStep = 1f;
	
	/// <summary>
	/// The constant 2 Pi.
	/// </summary>
	private const float PI2 = 2f * Mathf.PI;
	
	/// <summary>
	/// The central object. A.k.a the object to have things moving around it.
	/// </summary>
	public GameObject CentralObject;
	
	/// <summary>
	/// The normal vector to the orbital plane. E.g. (0, 1, 0) would have things orbiting on the x-z plane.
	/// If set to (0, 0, 0), the up vector will be CentralObject.transform.up.
	/// </summary>
	public Vector3 Up;
	
	/// <summary>
	/// The orbiting object along with its rotation speed (in rpm) stored in an OrbitInfo struct. 
	/// An orbiting object is the object to be moved around the central object.
	/// </summary>
	public OrbitInfo OrbitingObject;
	
	/// <summary>
	/// The triple containing the information needed to calculate the dynamic position of the orbiting object.
	/// </summary>
	private Orbit orbit;
	
	private Vector3 originalCentralBodyPosition;
	
	/// <summary>
	/// Initializes the script.
	/// </summary>
	void Start () {
		// Store the central object original position
		originalCentralBodyPosition = CentralObject.transform.position;
		
		// Calculate the up vector if not entered
		if (Up == Vector3.zero)
			Up = CentralObject.transform.up;

		// Get the normalized direction vector between the two objects
		Vector3 direction = (OrbitingObject.OrbitingObject.transform.position - CentralObject.transform.position).normalized;
		
		// Calculate a perpendicular vector to the orbital plane
		Vector3 normal = Vector3.Cross (direction, Up);
		
		normal = Vector3.Cross (normal, direction);
		
		// Add a new Orbit containing all the information needed for the orbit to the array of orbits
		orbit = new Orbit (OrbitingObject.OrbitingObject, OrbitingObject.revolutionsPerMinute, normal);
	}

	/// <summary>
	/// Updates the script.
	/// Called once per frame.
	/// </summary>
	void Update () {
		// for each orbiting object, progress its orbit by its rpm over the delta time

		// Set the orbiting object to its original position
		orbit.OrbitingObject.transform.position = orbit.originalPosition;
		
		// Calculate the amount to rotate
		float rotation = PI2 * Time.deltaTime * orbit.rpm;
		
		// Add the rotation to the accumulator in the orbit
		orbit.accumulator += rotation * TimeStep;
		
		Quaternion localRotation = orbit.OrbitingObject.transform.localRotation;
		
		// Rotate the orbiting object around the central bodys original position
		orbit.OrbitingObject.transform.RotateAround (originalCentralBodyPosition, orbit.axis, orbit.accumulator);
		
		orbit.OrbitingObject.transform.localRotation = localRotation;
		
		orbit.OrbitingObject.transform.position = Vector3.Lerp (orbit.OrbitingObject.transform.position, orbit.OrbitingObject.transform.position + (CentralObject.transform.position - originalCentralBodyPosition), Time.deltaTime);
	}
	
	/// <summary>
	/// A pair containing the orbiting object and the revolutions per minute at which the object should orbit.
	/// Used to feed in information the the SimpleOrbit class.
	/// </summary>
	[System.Serializable]
	public struct OrbitInfo {
		
		/// <summary>
		/// The orbiting object. A.k.a the object to be moved around the central object.
		/// </summary>
		public GameObject OrbitingObject;
		
		/// <summary>
		/// The revolutions per minute at which the orbiting object should orbit.
		/// </summary>
		public float revolutionsPerMinute;
		
	}
	
	/// <summary>
	/// A triple containing the orbiting object, the orbiting objects original position, and the axis to orbit around.
	/// Used for storing the information needed to calculate the dynamic positions of the orbiting objects.
	/// </summary>
	private class Orbit {
		
		/// <summary>
		/// The orbiting object. A.k.a the object to be moved around the central object.
		/// </summary>
		public GameObject OrbitingObject;
		
		/// <summary>
		/// The revolutions per minute at which the orbiting object should orbit.
		/// </summary>
		public float rpm;
		
		/// <summary>
		/// The original position of the orbiting object.
		/// </summary>
		public Vector3 originalPosition;
		
		/// <summary>
		/// The orbital axis is the normal vector to the orbital plane.
		/// </summary>
		public Vector3 axis;
		
		/// <summary>
		/// The accumulator should be increased over time up to 2Pi at which point it should be reset to zero
		/// </summary>
		public float accumulator;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleOrbit+Orbit"/> struct.
		/// </summary>
		/// <param name="OrbitingObject">The orbiting object. An orbiting object is the object to be moved around the central object.</param>
		/// <param name="axis">The orbital axis is the normal vector to the orbital plane. </param>
		public Orbit (GameObject OrbitingObject, float rpm, Vector3 axis) {
			this.OrbitingObject = OrbitingObject;
			this.rpm = rpm;
			this.originalPosition = OrbitingObject.transform.position;
			this.axis = axis;
			accumulator = 0f;
		}
		
	}
}