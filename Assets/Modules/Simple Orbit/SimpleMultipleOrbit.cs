using UnityEngine;

/// <summary>
///  Makes the selected game objects orbit another game object at a set speed (rpm).
///  The orbital inclination is calculated from the original positions of the objects.
///  Does not take physics in to account. Fixed orbit only.
/// </summary>
public class SimpleMultipleOrbit : MonoBehaviour {
	
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
	/// The array of orbiting objects along with their rotation speeds (in rpm) stored in OrbitInfo structs. 
	/// An orbiting object is the object to be moved around the central object.
	/// </summary>
	public OrbitInfo[] OrbitingObjects;

	/// <summary>
	/// The array of triples containing the information needed to calculate the dynamic positions of the orbiting objects.
	/// </summary>
	private Orbit[] orbits;

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

		orbits = new Orbit[OrbitingObjects.Length];

		// for each orbiting object, calculate the normal vector to its orbital plane
		for (int i=0; i<OrbitingObjects.Length; i++) {

			// Get the normalized direction vector between the two objects
			Vector3 direction = (OrbitingObjects[i].orbitingObject.transform.position - CentralObject.transform.position).normalized;

			// Calculate a perpendicular vector to the orbital plane
			Vector3 normal = Vector3.Cross (direction, Up);

			normal = Vector3.Cross (normal, direction);

			// Add a new Orbit containing all the information needed for the orbit to the array of orbits
			orbits[i] = new Orbit (OrbitingObjects[i].orbitingObject, OrbitingObjects[i].revolutionsPerMinute, normal);
		}
	}

	/// <summary>
	/// Updates the script.
	/// Called once per frame.
	/// </summary>
	void Update () {
		// for each orbiting object, progress its orbit by its rpm over the delta time
		foreach (Orbit orbit in orbits) {

			// Set the orbiting object to its original position
			orbit.orbitingObject.transform.position = orbit.originalPosition;

			// Calculate the amount to rotate
			float rotation = PI2 * Time.deltaTime * orbit.rpm;

			// Add the rotation to the accumulator in the orbit
			orbit.accumulator += rotation * TimeStep;

			Quaternion localRotation = orbit.orbitingObject.transform.localRotation;

			// Rotate the orbiting object around the central bodys original position
			orbit.orbitingObject.transform.RotateAround (originalCentralBodyPosition, orbit.axis, orbit.accumulator);

			orbit.orbitingObject.transform.localRotation = localRotation;
			
			orbit.orbitingObject.transform.position = Vector3.Lerp (orbit.orbitingObject.transform.position, orbit.orbitingObject.transform.position + (CentralObject.transform.position - originalCentralBodyPosition), Time.deltaTime);
		}
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
		public GameObject orbitingObject;

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
		public GameObject orbitingObject;
		
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
		/// <param name="orbitingObject">The orbiting object. An orbiting object is the object to be moved around the central object.</param>
		/// <param name="axis">The orbital axis is the normal vector to the orbital plane. </param>
		public Orbit (GameObject orbitingObject, float rpm, Vector3 axis) {
			this.orbitingObject = orbitingObject;
			this.rpm = rpm;
			this.originalPosition = orbitingObject.transform.position;
			this.axis = axis;
			accumulator = 0f;
		}
		
	}
}