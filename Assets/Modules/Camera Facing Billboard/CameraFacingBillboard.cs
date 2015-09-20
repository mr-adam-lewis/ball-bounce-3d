using UnityEngine;

/// <summary>
/// Camera facing billboard.
/// </summary>
public class CameraFacingBillboard : MonoBehaviour
{
	/// <summary>
	/// The camera to face.
	/// </summary>
	public Camera CameraToFace;
	
	/// <summary>
	/// The rotation plane normal.
	/// </summary>
	public Vector3 RotationPlaneNormal = Vector3.zero;
	
	/// <summary>
	/// What to do after the update.
	/// </summary>
	void LateUpdate()
	{
		// Set as main camera if null
		if (CameraToFace == null)
			CameraToFace = Camera.main;

		if (CameraToFace == null)
			return;
		
		// Set to look at orthographic camera
		if (CameraToFace.orthographic) {
			transform.rotation = CameraToFace.transform.rotation;
			transform.Rotate (0, 180, 0, Space.Self);
		} else {
			Vector3 position = CameraToFace.transform.position;
			Vector3 up = CameraToFace.transform.up;
			
			// Calculate position for billboards locked to a rotation plane
			if (RotationPlaneNormal != Vector3.zero) {
				position = Vector3.Cross (RotationPlaneNormal, Vector3.Cross (RotationPlaneNormal, (CameraToFace.transform.position - transform.position)));
				up = RotationPlaneNormal;
			}
			
			// Set to look at perspective camera
			transform.LookAt (position, up);
		}
	}
}