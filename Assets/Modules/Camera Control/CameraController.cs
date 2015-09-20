using UnityEngine;
using System.Collections;

/// <summary>
/// Provides full control over a camera in a scene.
/// </summary>
public class CameraController : MonoBehaviour {
	
	/// <summary>
	/// All the possible camera movement actions.
	/// </summary>
	public enum CameraMovementAction {
		None, Zoom, Pan, Rotate
	}

	/// <summary>
	/// Multiply by this to convert from radians to degrees.
	/// Divide by this to convert from degrees to radians.
	/// </summary>
	public const float RadiansToDegrees = 180f / Mathf.PI;
	
	/// <summary>
	/// The zoom speed multiplier for multitouch pinch gestures.
	/// </summary>
	public const float ZoomMultiplierPinch = 0.01f;
	
	/// <summary>
	/// The zoom multiplier for mouse scrolling.
	/// </summary>
	public const float ZoomMultiplierScroll = 0.1f;
	
	/// <summary>
	/// The zoom multiplier for dragging.
	/// </summary>
	public const float ZoomMultiplierDrag = 0.1f;

	/// <summary>
	/// The zoom multiplier for keyboard input.
	/// </summary>
	public const float ZoomMultiplierKeyboard = 0.1f;

	/// <summary>
	/// The pan multiplier for keyboard input.
	/// </summary>
	public const float PanMultiplierKeyboard = 0.05f;

	/// <summary>
	/// The cutoff value for the rotation on the Y-Z plane
	/// </summary>
	public const float RotationYZCutoff = 0.01f;

	/// <summary>
	/// The rotation multiplier.
	/// </summary>
	public const float RotationMultiplier = 0.01f; // TODO maybe base on screen resolution

	/// <summary>
	/// The camera to control.
	/// </summary>
	public Camera CameraToControl;

	/// <summary>
	/// The minimum zoom radius (in game space units).
	/// </summary>
	public float MinimumZoomRadius = 2;
	
	/// <summary>
	/// The maximum zoom radius (in game space).
	/// </summary>
	public float MaximumZoomRadius = 200;

	/// <summary>
	/// The normal vector to the movement plane.
	/// </summary>
	public Vector3 MovementPlaneNormal = new Vector3 (0, 1, 0);

	/// <summary>
	/// Whether or not to render a grid showing the movement plane.
	/// </summary>
	public bool VisualizeMovementPlane = false;

	/// <summary>
	/// The initial focus position.
	/// </summary>
	public Vector3 InitialFocusPosition = Vector3.zero;

	/// <summary>
	/// The game object to follow
	/// </summary>
	public GameObject FollowedGameObject;

	/// <summary>
	/// The settings for the camera controller.
	/// </summary>
	public CameraControllerSettings Settings;

	/// <summary>
	/// The focal point of the camera
	/// </summary>
	public Vector3 Focus;

	/// <summary>
	/// The distance of the camera from the focal point.
	/// </summary>
	private float radius;

	/// <summary>
	/// The angle of the position of the camera on the Z-Y plane (starting from the top)
	/// </summary>
	private float theta;

	/// <summary>
	/// The angle of the position of the camera on the X-Z plane (starting from the back).
	/// </summary>
	private float phi;

	/// <summary>
	/// The plane in which all movement takes place.
	/// </summary>
	private Plane movementPlane;

	/// <summary>
	/// The dynamic movement plane normal.
	/// </summary>
	public Vector3 DynamicMovementPlaneNormal;

	/// <summary>
	/// The original phi.
	/// </summary>
	private float originalPhi;

	/// <summary>
	/// The original theta.
	/// </summary>
	private float originalTheta;
	
	/// <summary>
	/// The last world coordinates used by the controller.
	/// </summary>
	private Vector3 lastWorldCoordinates;
	
	/// <summary>
	/// The last screen coordinates used by the controller.
	/// </summary>
	private Vector2 lastScreenCoordinates;

	/// <summary>
	/// The current camera movement action.
	/// </summary>
	private CameraMovementAction currentAction;

	/// <summary>
	/// The grid renderer used to render the movement plane.
	/// </summary>
	private MovementPlaneRenderer gridRenderer;

	/// <summary>
	/// The movement plane game object.
	/// </summary>
	public GameObject MovementPlaneGameObject;

	/// <summary>
	/// The follow delta time.
	/// </summary>
	private float followDeltaTime;

	/// <summary>
	/// The time since pan.
	/// </summary>
	private float timeSincePan;

	/// <summary>
	/// The screen size in inches.
	/// </summary>
	public static float ScreenSize;

    /// <summary>
    /// The grid increment
    /// </summary>
    private float gridMultiplier;

	/// <summary>
	/// Initializes the camera controller.
	/// </summary>
	void Start () {
		// Get screen inches
		ScreenSize = Mathf.Sqrt ((Screen.width * Screen.width) + (Screen.height * Screen.height));
		ScreenSize /= Screen.dpi;

		if (CameraToControl == null)
			CameraToControl = Camera.main;

		// Apply default settings if no settings selected
		if (Settings == null)
			Settings = new CameraControllerSettings ();

		// Set initial focus position
		Focus = InitialFocusPosition;
		
		MovementPlaneGameObject = new GameObject ();
		MovementPlaneGameObject.name = "Camera Controller Movement Plane";

		// Construct movement plane
		DynamicMovementPlaneNormal = MovementPlaneNormal;
		movementPlane = new Plane (DynamicMovementPlaneNormal, Focus);

		// If followed object input, keep camera position where it was and look at object
		if (FollowedGameObject != null)
			// Set the focus
			Focus = FollowedGameObject.transform.position;
		
		CalculateInitialThetaPhiRadius ();
	}

	/// <summary>
	/// Calculates the initial theta, phi, and radius.
	/// </summary>
	public void CalculateInitialThetaPhiRadius () {
		// Calculate the vector between the camera and the focus
		Vector3 focusToCamera = CameraToControl.transform.position - Focus;
		
		// Set the radius as the distance between the focus and the camera
		radius = focusToCamera.magnitude;
		
		// Calculate rotation angles
		phi = Mathf.Asin (focusToCamera.y / new Vector2 (focusToCamera.y, focusToCamera.z).magnitude);
		theta = Mathf.Asin (-focusToCamera.z / new Vector2 (focusToCamera.x, focusToCamera.z).magnitude);
		
		// Correct NaN errors
		if (float.IsNaN (theta))
			theta = 0f;
		if (float.IsNaN (phi))
			phi = 0f;
		
		// Set the original values for handling panning on the movement plane
		originalPhi = - RadiansToDegrees * phi;
		originalTheta = 90 - RadiansToDegrees * theta;
		
		OnRotate (0, 0);
	}

	/// <summary>
	/// Creates the grid renderer.
	/// </summary>
	private void CreateGridRenderer () {
		if (gridRenderer != null)
			return;

		// Construct the grid renderer
		gridRenderer = CameraToControl.gameObject.AddComponent <MovementPlaneRenderer> ();
		gridRenderer.TransformationMatrix = MovementPlaneGameObject.transform;
		gridRenderer.GridWidth = (int) (20f * MaximumZoomRadius);
		gridRenderer.GridHeight = (int) (20f * MaximumZoomRadius);
		gridRenderer.Step = gridRenderer.GridWidth / 100f;
	}

	/// <summary>
	/// Gets the radius.
	/// </summary>
	/// <returns>The radius.</returns>
	public float GetRadius () {
		return radius;
	}

	/// <summary>
	/// Gets the theta angle.
	/// </summary>
	/// <returns>The theta.</returns>
	public float GetTheta () {
		return theta;
	}

	/// <summary>
	/// Gets the phi angle.
	/// </summary>
	/// <returns>The phi.</returns>
	public float GetPhi () {
		return phi;
	}

	/// <summary>
	/// Makes the camera follow the specified gameObject.
	/// </summary>
	/// <param name="gameObject">The game object to follow.</param>
	public void FollowGameObject (GameObject gameObject) {
		this.FollowedGameObject = gameObject;
		followDeltaTime = 0f;
	}

	/// <summary>
	/// Stops the camera following the followed game object.
	/// If the followed game object has not been set, this method will have no effect.
	/// </summary>
	public void UnfollowGameObject () {
		this.FollowedGameObject = null;
	}
	
	/// <summary>
	/// Zooms the camera by the given amount.
	/// </summary>
	/// <param name="amount">The amount to zoom by.</param>
	public void ZoomBy (float amount) {
		ZoomTo (radius + amount);
	}
	
	/// <summary>
	/// Zooms the camera to the given radius from the focal point.
	/// </summary>
	/// <param name="dist">The distance from the focal point to set.</param>
	public void ZoomTo (float dist) {
		if (!Settings.ZoomEnabled)
			return;

		radius = dist;
		
		if (radius < MinimumZoomRadius)
			radius = MinimumZoomRadius;
		else if (radius > MaximumZoomRadius)
			radius = MaximumZoomRadius;
	}

	/// <summary>
	/// Moves the camera focal point by the given amount.
	/// </summary>
	/// <param name="amount">The amount to move by</param>
	public void MoveBy (Vector3 amount) {
		MoveTo (Focus + amount);
	}

	/// <summary>
	/// Moves the camera focal point to the given position.
	/// </summary>
	/// <param name="position">The position to move the focal point to.</param>
	public void MoveTo (Vector3 position) {
		if (!Settings.PanEnabled)
			return;

		Focus = position;
	}

	/// <summary>
	/// Rotates the camera around the Y-Z plane by the given angle.
	/// The camera will not go further than the limits set in the controller (to avoid up vector flip).
	/// </summary>
	/// <param name="angle">The angle in radians.</param>
	public void RotatePhiBy (float angle) {
		RotatePhiTo (phi + angle);
	}

	/// <summary>
	/// Rotates the camera around the Y-Z plane to the given angle.
	/// The camera will not go further than the limits set in the controller (to avoid up vector flip).
	/// </summary>
	/// <param name="angle">The angle in radians.</param>
	public void RotatePhiTo (float angle) {
		if (!Settings.RotationEnabled)
			return;

		phi = angle;
		
		if (phi >= Mathf.PI / 2f)
			phi = Mathf.PI / 2f - RotationYZCutoff;
		else if (phi <= -Mathf.PI / 2f)
			phi = - Mathf.PI / 2f + RotationYZCutoff;
	}

	/// <summary>
	/// Rotates the camera around the X-Z plane by the given angle.
	/// </summary>
	/// <param name="angle">The angle in radians.</param>
	public void RotateThetaBy (float angle) {
		RotateThetaTo (theta + angle);
	}

	/// <summary>
	/// Rotates the camera around the X-Z plane to the given angle.
	/// </summary>
	/// <param name="angle">The angle in radians.</param>
	public void RotateThetaTo (float angle) {
		if (!Settings.RotationEnabled)
			return;

		theta = angle;
	}
	
	/// <summary>
	/// Maneuvers the camera to the position specified by this controller and looks at the focal point of this controller.
	/// </summary>
	private void ManeuverCamera () {
		// Calculate trig values for camera angles
		float ct = Mathf.Cos(theta);
		float st = Mathf.Sin(theta);
		float cp = Mathf.Cos(phi);
		float sp = Mathf.Sin(phi);
		
		// Calculate the position of the camera
		float x = radius * ct * cp;
		float y = radius * sp;
		float z = -radius * st * cp;

		// Calculate camera position
		Vector3 campos = new Vector3 (x, y, z);
		
		// Set the position of the camera
		//CameraToControl.transform.position = Vector3.Lerp (CameraToControl.transform.position, campos + Focus, Time.deltaTime);
		CameraToControl.transform.position = campos + Focus;
		
		// Make the camera look at the focal point
		CameraToControl.transform.LookAt (Focus, Vector3.up);
	}

	/// <summary>
	/// Gets the world coords for a given screen x, y position given as a 2d vector.
	/// </summary>
	/// <returns>The world coordinates.</returns>
	/// <param name="screenCoordinates">The screen coordinates.</param>
	public Vector3 GetWorldCoordinates(Vector2 screenCoordinates) {
		Ray ray = CameraToControl.ScreenPointToRay (screenCoordinates);
		float rayDistance;
		movementPlane.SetNormalAndPosition (DynamicMovementPlaneNormal, Focus);
		movementPlane.Raycast (ray, out rayDistance);
		return ray.GetPoint (rayDistance);
	}

	/// <summary>
	/// Updates the camera controller.
	/// </summary>
	void FixedUpdate () {
		if (CameraToControl == null 
		    || !CameraToControl.enabled 
		    || !enabled)
			return;

		// Prevent mouse simulation with touches
		Input.simulateMouseWithTouches = false;
		
		// Follow game object			
		if (FollowedGameObject != null) {
            Vector3 diff = (FollowedGameObject.transform.position - Focus) * (Time.smoothDeltaTime + followDeltaTime);
            OnPan(diff);
			//Focus = Vector3.Lerp (Focus, FollowedGameObject.transform.position, Time.smoothDeltaTime + followDeltaTime);
			if (followDeltaTime < 1f - Time.smoothDeltaTime)
				followDeltaTime += 0.01f;
		} else
			followDeltaTime = 0f;

		// Handle the different forms of input
		HandleTouchInput ();
		HandleMouseInput ();
		HandleKeyboardInput ();

		// Move the camera to its new position
		ManeuverCamera ();

		// Visualize the movement plane
		if (VisualizeMovementPlane) {
			CreateGridRenderer ();
			gridRenderer.TransformationMatrix.position = Focus;
			gridRenderer.TransformationMatrix.Rotate (0, 180, 0);
			if (Settings.MovementPlaneMode == CameraControllerSettings.MovementPlaneType.Dynamic) {
				if (DynamicMovementPlaneNormal == MovementPlaneNormal)
					OnRotate (0, 0);
				gridRenderer.TransformationMatrix.LookAt (Focus + DynamicMovementPlaneNormal, CameraToControl.transform.up);
			} else {
				DynamicMovementPlaneNormal = MovementPlaneNormal;
				gridRenderer.TransformationMatrix.LookAt (Focus + DynamicMovementPlaneNormal);
			}
            if (radius > 5f * gridMultiplier)
                gridMultiplier *= 5f;
            if (radius < gridMultiplier / 5f)
                gridMultiplier /= 5f;
            if (gridMultiplier <= MinimumZoomRadius)
                gridMultiplier = MinimumZoomRadius;
			gridRenderer.GridWidth = (int) (gridMultiplier * MinimumZoomRadius);
			gridRenderer.GridHeight = (int) (gridMultiplier * MinimumZoomRadius);
			gridRenderer.Step = gridRenderer.GridWidth / 10f;
		} else if (gridRenderer != null)
			Destroy (gridRenderer);
	}

	//###################################################################################################################################//
	//----------------------------------------------------- Event Handling --------------------------------------------------------------//
	//###################################################################################################################################//


	//------------------------------------------------------- Zoom Events ---------------------------------------------------------------//
	
	/// <summary>
	/// What to do when a zoom event starts.
	/// </summary>
	public void OnZoomStart () {
		if (!Settings.ZoomEnabled)
			return;
		OnZoomStart (Vector2.zero);
	}

	/// <summary>
	/// What to do when a zoom event starts.
	/// </summary>
	/// <param name="screenCoordinates">The screen coordinates of the pointer used to do the zooming (if a pointer was used).</param>
	public void OnZoomStart (Vector2 screenCoordinates) {
		if (currentAction != CameraMovementAction.None || !Settings.ZoomEnabled)
			return;

		// Initialize last screen coordinates
		lastScreenCoordinates = screenCoordinates;
	}
	
	/// <summary>
	/// What to do when the camera controller zooms.
	/// </summary>
	/// <param name="amount">The amount zoomed by.</param>
	public void OnZoom (float amount) {
		if (!Settings.ZoomEnabled)
			return;
		ZoomBy (amount);
	}

	/// <summary>
	/// What to do when the camera controller zooms.
	/// </summary>
	/// <param name="screenCoordinates">The screen coordinates of the pointer used to do the zooming (if a pointer was used).</param>
	public void OnZoom (Vector2 screenCoordinates) {
		if (!Settings.ZoomEnabled)
			return;

		// If initialized
		if (lastScreenCoordinates != Vector2.zero) {
			
			float magnitude = (screenCoordinates.y - lastScreenCoordinates.y) * ZoomMultiplierDrag;
			
			OnZoom (-magnitude);
			
			// Reset the last mouse coordinates to the current ones
			lastScreenCoordinates = screenCoordinates;
			
		} else // Initialize
			lastScreenCoordinates = screenCoordinates;
	}
	
	/// <summary>
	/// What to do when a zoom event ends.
	/// </summary>
	public void OnZoomEnd () {
		if (!Settings.ZoomEnabled)
			return;

		// Set back to zero (uninitialize)
		lastScreenCoordinates = Vector2.zero;

		currentAction = CameraMovementAction.None;
	}

	//------------------------------------------------------- Pan Events --------------------------------------------------------------//
	
	/// <summary>
	/// What to do when a pan event starts.
	/// </summary>
	/// <param name="screenCoordinates">The screen coordinates.</param>
	public void OnPanStart (Vector2 screenCoordinates) {
		if (currentAction != CameraMovementAction.None || !Settings.PanEnabled)
			return;

		lastWorldCoordinates = GetWorldCoordinates (screenCoordinates);
		lastScreenCoordinates = screenCoordinates;
	}
	
	/// <summary>
	/// What to do when the camera pans.
	/// </summary>
	/// <param name="screenCoordinates">The screen coordinates.</param>
	public void OnPan (Vector2 screenCoordinates) {
		if (!Settings.PanEnabled)
			return;

		if (FollowedGameObject != null) {
			return;
		}

		// Check if last world coordinates have been stored
		if (lastWorldCoordinates != Vector3.zero) {
			// Calculate translation
			Vector3 currentWorldCoords = GetWorldCoordinates (screenCoordinates);
			Vector3 translation = lastWorldCoordinates - currentWorldCoords; 

			// Only pan if different screen coordinates
			if (screenCoordinates != lastScreenCoordinates)
				OnPan (translation);

			// Set last screen coordinates
			lastScreenCoordinates = screenCoordinates;
		} else {
			// if not, store them and continue
			lastScreenCoordinates = screenCoordinates;
			lastWorldCoordinates = GetWorldCoordinates (screenCoordinates);
		}
	}
	
	/// <summary>
	/// What to do when the camera is panned.
	/// </summary>
	/// <param name="translation">The translation to apply to the focus.</param>
	public void OnPan (Vector3 translation) {
		if (!Settings.PanEnabled)
			return;

		// Apply translation
		MoveBy (translation);

		// Update grid renderer with movements
		if (gridRenderer != null) {
			float theta = Vector3.Dot(gridRenderer.TransformationMatrix.right, translation.normalized) - Mathf.PI / 2f;
			float x = - translation.magnitude * Mathf.Cos(theta);
			theta = Vector3.Dot (gridRenderer.TransformationMatrix.up, translation.normalized) - Mathf.PI / 2f;
			float y = -translation.magnitude * Mathf.Cos(theta);
			gridRenderer.PanX += x;
			gridRenderer.PanY += y;
		}
	}
	
	/// <summary>
	/// What to do then a pan event ends
	/// </summary>
	public void OnPanEnd () {
		if (!Settings.PanEnabled)
			return;

		// Set back to zero (uninitialize)
		lastWorldCoordinates = Vector3.zero;

		currentAction = CameraMovementAction.None;
	}

	//------------------------------------------------------ Rotate Events --------------------------------------------------------------//

	/// <summary>
	/// What to do when a rotate event starts.
	/// </summary>
	public void OnRotateStart () {
		if (!Settings.RotationEnabled)
			return;
		OnRotateStart (Vector2.zero);
	}

	/// <summary>
	/// What to do when a rotate event starts.
	/// </summary>
	/// <param name="screenCoordinates">The screen coordinates of the pointer used to do the rotating.</param>
	public void OnRotateStart (Vector2 screenCoordinates) {
		if (currentAction != CameraMovementAction.None || !Settings.RotationEnabled)
			return;

		// Initialize last screen coordinates
		lastScreenCoordinates = screenCoordinates;
	}

	/// <summary>
	/// What to do when the camera is rotated.
	/// </summary>
	/// <param name="screenCoordinates">The screen coordinates of the pointer used to do the rotating.</param>
	public void OnRotate (Vector2 screenCoordinates) {
		if (!Settings.RotationEnabled)
			return;

		// If initialized
		if (lastScreenCoordinates != Vector2.zero) {
			
			// Calculate rotation
			float theta = -(lastScreenCoordinates.x - screenCoordinates.x) * RotationMultiplier;
			float phi = -(screenCoordinates.y - lastScreenCoordinates.y) * RotationMultiplier;
			
			// Rotate by the difference
			OnRotate (theta, phi);
			
			// Reset the last mouse coordinates to the current ones
			lastScreenCoordinates = screenCoordinates;
			
		} else // Initialize
			lastScreenCoordinates = screenCoordinates;
	}
	
	/// <summary>
	/// What to do when the camera is rotated.
	/// </summary>
	/// <param name="theta">The theta angle in radians.</param>
	/// <param name="phi">The phi angle in radians.</param>
	public void OnRotate (float theta, float phi) {
		if (!Settings.RotationEnabled)
			return;
		
		// Perform the rotation
		RotatePhiBy (phi);
		RotateThetaBy (theta);
		
		// If the movement plane mode is set to dynamic, rotate the movement plane normal as well
		if (Settings.MovementPlaneMode == CameraControllerSettings.MovementPlaneType.Dynamic)
			DynamicMovementPlaneNormal = Quaternion.Euler (0, RadiansToDegrees * this.theta + originalTheta, 0) 
				* (Quaternion.Euler (0, 0, RadiansToDegrees * this.phi + originalPhi) 
				   * MovementPlaneNormal);
	}
	
	/// <summary>
	/// What to do when a rotate event ends
	/// </summary>
	public void OnRotateEnd () {
		if (currentAction != CameraMovementAction.Rotate || !Settings.RotationEnabled)
			return;

		// Set back to zero (uninitialize)
		lastScreenCoordinates = Vector2.zero;

		currentAction = CameraMovementAction.None;
	}
	
	//###################################################################################################################################//
	//-------------------------------------------------- Touch Input Handling -----------------------------------------------------------//
	//###################################################################################################################################//

	/// <summary>
	/// Handles the touch input to control the camera.
	/// </summary>
	private void HandleTouchInput () {
		if (!Settings.TouchEnabled || Input.touchCount == 0)
			return;
	
		// Handle zooming using touch input
		if (Settings.TouchZoomMode != CameraControllerSettings.TouchZoomType.Disabled) {

			if (Input.touchCount == 1) {
				// Access touches
				Touch touchZero = Input.GetTouch (0);

				if (Settings.TouchZoomMode == CameraControllerSettings.TouchZoomType.OneFingerDrag) {

					// Start zoom
					if (touchZero.phase == TouchPhase.Began)
						OnZoomStart (touchZero.position);
					
					// End zoom
					else if (touchZero.phase == TouchPhase.Ended
					         || touchZero.phase == TouchPhase.Canceled)
						OnZoomEnd ();
					
					// Zoom using drag
					else if (touchZero.phase == TouchPhase.Moved)
						// Zoom by the difference
						OnZoom (touchZero.position);
				}

			} else if (Input.touchCount == 2) {
				// Access touches
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);
				
				// Calculate previous touch positions
				Vector2 touchZeroPreviousPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePreviousPos = touchOne.position - touchOne.deltaPosition;

				// Pinch to zoom
				if (Settings.TouchZoomMode == CameraControllerSettings.TouchZoomType.TwoFingerPinch) {

					// Calculate previous touch distances
					float previousTouchDeltaMag = (touchZeroPreviousPos - touchOnePreviousPos).magnitude;
					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

					// Calculate the difference
					float deltaMag = previousTouchDeltaMag - touchDeltaMag;

					deltaMag *= ScreenSize / 10f;

					// Start zoom
					if (touchZero.phase == TouchPhase.Began 
							|| touchOne.phase == TouchPhase.Began)
						OnZoomStart ();

					// End zoom
					else if (touchZero.phase == TouchPhase.Ended 
						|| touchOne.phase == TouchPhase.Ended 
						|| touchZero.phase == TouchPhase.Canceled 
						|| touchOne.phase == TouchPhase.Canceled)
						OnZoomEnd ();

					// Zoom using pinch
					else if (touchZero.phase == TouchPhase.Moved 
						|| touchOne.phase == TouchPhase.Moved) {
						float amount = deltaMag * radius * ZoomMultiplierPinch;
						// Zoom by the difference if above tolerance
						if (Mathf.Abs(deltaMag) > 0.1f)
							OnZoom (amount);
					}

				} else if (Settings.TouchZoomMode == CameraControllerSettings.TouchZoomType.TwoFingerDrag) {

					// Start zoom
					if (touchZero.phase == TouchPhase.Began 
						|| touchOne.phase == TouchPhase.Began)
						OnZoomStart (touchZero.position);
					
					// End zoom
					else if (touchZero.phase == TouchPhase.Ended 
						|| touchOne.phase == TouchPhase.Ended 
						|| touchZero.phase == TouchPhase.Canceled 
						|| touchOne.phase == TouchPhase.Canceled)
						OnZoomEnd ();
					
					// Zoom using drag
					else if (touchZero.phase == TouchPhase.Moved 
						|| touchOne.phase == TouchPhase.Moved)
						// Zoom by the difference
						OnZoom (touchZero.position);
				}

			} else if (Input.touchCount == 3) {
				// Access touches
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);
				Touch touchTwo = Input.GetTouch (2);

				// Zoom using 3 finger drag
				if (Settings.TouchZoomMode == CameraControllerSettings.TouchZoomType.ThreeFingerDrag) {

					// Start zoom
					if (touchZero.phase == TouchPhase.Began 
					    || touchOne.phase == TouchPhase.Began
					    || touchTwo.phase == TouchPhase.Began)
						OnZoomStart (touchZero.position);
					
					// End zoom
					else if (touchZero.phase == TouchPhase.Ended 
						     || touchOne.phase == TouchPhase.Ended 
						     || touchTwo.phase == TouchPhase.Ended 
							 || touchZero.phase == TouchPhase.Canceled 
					         || touchOne.phase == TouchPhase.Canceled
					         || touchTwo.phase == TouchPhase.Canceled)
						OnZoomEnd ();
					
					// Zoom using drag
					else if (touchZero.phase == TouchPhase.Moved 
					         || touchOne.phase == TouchPhase.Moved
							 || touchTwo.phase == TouchPhase.Moved)
						// Zoom by the difference
						OnZoom (touchZero.position);
				}
			}
		}

		// Handle panning using touch input
		if (Settings.TouchPanMode != CameraControllerSettings.TouchPanType.Disabled) {
			
			if (Input.touchCount == 1) {
				// Access touches
				Touch touchZero = Input.GetTouch (0);

				if (Settings.TouchPanMode == CameraControllerSettings.TouchPanType.OneFingerDrag) {

					// Start pan
					if (touchZero.phase == TouchPhase.Began)
						OnPanStart (touchZero.position);
					
					// End pan
					else if (touchZero.phase == TouchPhase.Ended 
					         || touchZero.phase == TouchPhase.Canceled)
						OnPanEnd ();
					
					// Pan using touch
					else if (touchZero.phase == TouchPhase.Moved )
						// Pan using difference in world positions
						OnPan (touchZero.position);
				}
				
			} else if (Input.touchCount == 2) {
				// Access touches
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);

				// Calculate mid point
				Vector2 midPoint = (touchOne.position + touchZero.position) / 2f;
				
				if (Settings.TouchPanMode == CameraControllerSettings.TouchPanType.TwoFingerDrag) {
					
					// Start pan
					if (touchZero.phase == TouchPhase.Began 
					    || touchOne.phase == TouchPhase.Began)
						OnPanStart (midPoint);
					
					// End pan
					else if (touchZero.phase == TouchPhase.Ended 
					         || touchOne.phase == TouchPhase.Ended 
					         || touchZero.phase == TouchPhase.Canceled 
					         || touchOne.phase == TouchPhase.Canceled)
						OnPanEnd ();
					
					// Pan using touch
					else if (touchZero.phase == TouchPhase.Moved 
					         || touchOne.phase == TouchPhase.Moved)
						// Pan using difference in world positions
						OnPan (midPoint);
				}				
				
			} else if (Input.touchCount == 3) {
				// Access touches
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);
				Touch touchTwo = Input.GetTouch (2);
				
				if (Settings.TouchPanMode == CameraControllerSettings.TouchPanType.ThreeFingerDrag) {

					// Start pan
					if (touchZero.phase == TouchPhase.Began 
					    || touchOne.phase == TouchPhase.Began
					    || touchTwo.phase == TouchPhase.Began)
						OnPanStart (touchZero.position);
					
					// End pan
					else if (touchZero.phase == TouchPhase.Ended 
					         || touchOne.phase == TouchPhase.Ended 
					         || touchTwo.phase == TouchPhase.Ended 
					         || touchZero.phase == TouchPhase.Canceled 
					         || touchOne.phase == TouchPhase.Canceled
					         || touchTwo.phase == TouchPhase.Canceled)
						OnPanEnd ();
					
					// Pan using touch
					else if (touchZero.phase == TouchPhase.Moved 
					         || touchOne.phase == TouchPhase.Moved
					         || touchTwo.phase == TouchPhase.Moved)
						// Pan using difference in world positions
						OnPan (touchZero.position);
				}

			}
		}

		// Handle rotation using touch input
		if (Settings.TouchRotationMode != CameraControllerSettings.TouchRotationType.Disabled) {
			
			if (Input.touchCount == 1) {
				// Access touches
				Touch touchZero = Input.GetTouch (0);
				
				if (Settings.TouchRotationMode == CameraControllerSettings.TouchRotationType.OneFingerDrag) {
					
					// Start rotation
					if (touchZero.phase == TouchPhase.Began)
						OnRotateStart (touchZero.position);
					
					// End rotation
					else if (touchZero.phase == TouchPhase.Ended 
					         || touchZero.phase == TouchPhase.Canceled)
						OnRotateEnd ();
					
					// Rotate using touch
					else if (touchZero.phase == TouchPhase.Moved)
						// Rotate using difference in screen positions
						OnRotate (touchZero.position);

				}
				
			} else if (Input.touchCount == 2) {
				// Access touches
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);
				
				// Calculate mid point
				Vector2 midPoint = (touchOne.position + touchZero.position) / 2f;
				
				if (Settings.TouchRotationMode == CameraControllerSettings.TouchRotationType.TwoFingerDrag) {
					
					// Start rotation
					if (touchZero.phase == TouchPhase.Began 
					    || touchOne.phase == TouchPhase.Began)
						OnRotateStart (midPoint);
					
					// End rotation
					else if (touchZero.phase == TouchPhase.Ended 
					         || touchOne.phase == TouchPhase.Ended 
					         || touchZero.phase == TouchPhase.Canceled 
					         || touchOne.phase == TouchPhase.Canceled)
						OnRotateEnd ();
					
					// Rotate using touch
					else if (touchZero.phase == TouchPhase.Moved 
					         || touchOne.phase == TouchPhase.Moved)
						// Rotate using difference in screen positions
						OnRotate (midPoint);

				}
				
			} else if (Input.touchCount == 3) {
				// Access touches
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);
				Touch touchTwo = Input.GetTouch (2);
				
				if (Settings.TouchRotationMode == CameraControllerSettings.TouchRotationType.ThreeFingerDrag) {
					
					// Start rotation
					if (touchZero.phase == TouchPhase.Began 
					    || touchOne.phase == TouchPhase.Began
					    || touchTwo.phase == TouchPhase.Began)
						OnRotateStart (touchZero.position);
					
					// End rotation
					else if (touchZero.phase == TouchPhase.Ended 
					         || touchOne.phase == TouchPhase.Ended 
					         || touchTwo.phase == TouchPhase.Ended 
					         || touchZero.phase == TouchPhase.Canceled 
					         || touchOne.phase == TouchPhase.Canceled
					         || touchTwo.phase == TouchPhase.Canceled)
						OnRotateEnd ();
					
					// Rotate using touch
					else if (touchZero.phase == TouchPhase.Moved 
					         || touchOne.phase == TouchPhase.Moved
					         || touchTwo.phase == TouchPhase.Moved)
						// Rotate using difference in screen positions
						OnRotate (touchZero.position);
				}
				
			}
		}

	}
	
	//###################################################################################################################################//
	//-------------------------------------------------- Mouse Input Handling -----------------------------------------------------------//
	//###################################################################################################################################//

	/// <summary>
	/// Handles the mouse input to control the camera.
	/// </summary>
	private void HandleMouseInput () {
		if (!Settings.MouseEnabled)
			return;

		Vector2 mousePosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

		// Zoom using settings
		if (Settings.MouseZoomMode != CameraControllerSettings.MouseZoomType.Disabled) {
			
			// Zoom using mouse scroll
			if (Settings.MouseZoomMode == CameraControllerSettings.MouseZoomType.Scroll) {

				// Zoom by mouse scroll on y axis
				if (Input.mouseScrollDelta.y != 0)
					OnZoom (-Input.mouseScrollDelta.y * ZoomMultiplierScroll * radius);

			} else {
				
				// Get the correct mouse button from settings
				int mouseButton = 0;
					switch (Settings.MouseZoomMode) {
					case CameraControllerSettings.MouseZoomType.LeftButtonDrag : mouseButton = 0;
						break;
					case CameraControllerSettings.MouseZoomType.RightButtonDrag : mouseButton = 1;
						break;
					case CameraControllerSettings.MouseZoomType.MiddleButtonDrag : mouseButton = 2;
						break;
				}

				// Initialize the zoom
				if (Input.GetMouseButtonDown (mouseButton))
					OnZoomStart (mousePosition);

				// End the zoom
				else if (Input.GetMouseButtonUp (mouseButton))
					OnZoomEnd ();

				// Zoom dragging mouse button from settings
				else if (Input.GetMouseButton (mouseButton))
					OnZoom (mousePosition);
			}

		}

		// Pan using mouse drag on button from settings
		if (Settings.MousePanMode != CameraControllerSettings.MousePanType.Disabled) {
			
			// Get the correct mouse button from settings
			int mouseButton = 0;
			switch (Settings.MousePanMode) {
			case CameraControllerSettings.MousePanType.LeftButtonDrag : mouseButton = 0;
				break;
			case CameraControllerSettings.MousePanType.RightButtonDrag : mouseButton = 1;
				break;
			case CameraControllerSettings.MousePanType.MiddleButtonDrag : mouseButton = 2;
				break;
			}

			// Initialize the pan
			if (Input.GetMouseButtonDown (mouseButton))
				OnPanStart (mousePosition);

			// End the pan
			else if (Input.GetMouseButtonUp (mouseButton))
				OnPanEnd ();
			
			// Mouse button panning from settings
			else if (Input.GetMouseButton (mouseButton))
				OnPan (mousePosition);

		}
		
		// Rotate using mouse button declared in settings
		if (Settings.MouseRotationMode != CameraControllerSettings.MouseRotateType.Disabled) {

			// Get the correct mouse button from settings
			int mouseButton = 0;
			switch (Settings.MouseRotationMode) {
				case CameraControllerSettings.MouseRotateType.LeftButtonDrag : mouseButton = 0;
					break;
				case CameraControllerSettings.MouseRotateType.RightButtonDrag : mouseButton = 1;
					break;
				case CameraControllerSettings.MouseRotateType.MiddleButtonDrag : mouseButton = 2;
					break;
			}

			// Initialize the rotation
			if (Input.GetMouseButtonDown (mouseButton))
				OnRotateStart (mousePosition);
			
			// End the rotation
			else if (Input.GetMouseButtonUp (mouseButton))
				OnRotateEnd ();

			// Mouse button rotation from settings
			else if (Input.GetMouseButton (mouseButton))
				OnRotate (mousePosition);
		}
	}
	
	//###################################################################################################################################//
	//------------------------------------------------- Keyboard Input Handling ---------------------------------------------------------//
	//###################################################################################################################################//

	/// <summary>
	/// Handles the keyboard input to control the camera.
	/// </summary>
	private void HandleKeyboardInput () {
		if (!Settings.KeyboardEnabled)
			return;

		// Zoom using keycodes from settings
		if (Input.GetKey (Settings.KeyZoomIn))
			OnZoom (-ZoomMultiplierKeyboard);
		else if (Input.GetKey (Settings.KeyZoomOut))
			OnZoom (ZoomMultiplierKeyboard);

		// Get planar forwards and right vectors
		Vector3 forward = MovementPlaneGameObject.transform.up;
		Vector3 right = -MovementPlaneGameObject.transform.right;

		// Pan using keycodes from settings
		if (Input.GetKey (Settings.KeyPanForward))
			OnPan (forward * radius * PanMultiplierKeyboard);

		if (Input.GetKey (Settings.KeyPanBackward))
			OnPan (-forward * radius * PanMultiplierKeyboard);

		if (Input.GetKey (Settings.KeyPanLeft))
			OnPan (-right * radius * PanMultiplierKeyboard);

		if (Input.GetKey (Settings.KeyPanRight))
			OnPan (right * radius * PanMultiplierKeyboard);

		// Rotate X-Z
		if (Input.GetKey (Settings.KeyRotateLeft))
		    OnRotate (RotationMultiplier, 0);
		else if (Input.GetKey (Settings.KeyRotateRight))
			OnRotate (-RotationMultiplier, 0);

		// Rotate Y-Z
		if (Input.GetKey (Settings.KeyRotateUp))
			OnRotate (0, RotationMultiplier);
		else if (Input.GetKey (Settings.KeyRotateDown))
			OnRotate (0, -RotationMultiplier);
	}
}