using UnityEngine;

/// <summary>
/// Contains all settings for the camera controller.
/// </summary>
[System.Serializable]
public class CameraControllerSettings {

	/// <summary>
	/// The movement plane mode.
	/// </summary>
	public MovementPlaneType MovementPlaneMode;

	/// <summary>
	/// Whether or not touch input is enabled for the camera controller
	/// </summary>
	public bool TouchEnabled = false;

	/// <summary>
	/// Whether or not mouse input is enabled for the camera controller
	/// </summary>
	public bool MouseEnabled = false;

	/// <summary>
	/// Whether or not keyboard input is enabled for the camera controller
	/// </summary>
	public bool KeyboardEnabled = false;

	/// <summary>
	/// The zoom enabled.
	/// </summary>
	public bool ZoomEnabled = true;

	/// <summary>
	/// The pan enabled.
	/// </summary>
	public bool PanEnabled = true;

	/// <summary>
	/// The rotation enabled.
	/// </summary>
	public bool RotationEnabled = true;

	/// <summary>
	/// The key code for zooming in.
	/// </summary>
	public KeyCode KeyZoomIn = KeyCode.None;

	/// <summary>
	/// The key code for zooming out.
	/// </summary>
	public KeyCode KeyZoomOut = KeyCode.None;

	/// <summary>
	/// The key code for panning forward.
	/// </summary>
	public KeyCode KeyPanForward = KeyCode.None;
	
	/// <summary>
	/// The key code for panning backward.
	/// </summary>
	public KeyCode KeyPanBackward = KeyCode.None;
	
	/// <summary>
	/// The key code for panning left.
	/// </summary>
	public KeyCode KeyPanLeft = KeyCode.None;
	
	/// <summary>
	/// The key code for panning right.
	/// </summary>
	public KeyCode KeyPanRight = KeyCode.None;

	/// <summary>
	/// The keycode for rotating left.
	/// </summary>
	public KeyCode KeyRotateLeft = KeyCode.None;
	
	/// <summary>
	/// The keycode for rotating right.
	/// </summary>
	public KeyCode KeyRotateRight = KeyCode.None;
	
	/// <summary>
	/// The keycode for rotating up.
	/// </summary>
	public KeyCode KeyRotateUp = KeyCode.None;
	
	/// <summary>
	/// The keycode for rotating down.
	/// </summary>
	public KeyCode KeyRotateDown = KeyCode.None;

	/// <summary>
	/// The touch zoom mode.
	/// </summary>
	public TouchZoomType TouchZoomMode = TouchZoomType.Disabled;

	/// <summary>
	/// The touch pan mode.
	/// </summary>
	public TouchPanType TouchPanMode = TouchPanType.Disabled;

	/// <summary>
	/// The touch rotation mode.
	/// </summary>
	public TouchRotationType TouchRotationMode = TouchRotationType.Disabled;

	/// <summary>
	/// The mouse zoom mode.
	/// </summary>
	public MouseZoomType MouseZoomMode = MouseZoomType.Disabled;

	/// <summary>
	/// The mouse pan mode.
	/// </summary>
	public MousePanType MousePanMode = MousePanType.Disabled;

	/// <summary>
	/// The mouse rotation mode.
	/// </summary>
	public MouseRotateType MouseRotationMode = MouseRotateType.Disabled;
	
	/// <summary>
	/// The enumerations for touch zoom mode
	/// </summary>	
	public enum TouchZoomType {
		Disabled, TwoFingerPinch, OneFingerDrag, TwoFingerDrag, ThreeFingerDrag
	}
	
	/// <summary>
	/// The enumerations for touch pan mode
	/// </summary>	
	public enum TouchPanType {
		Disabled, OneFingerDrag, TwoFingerDrag, ThreeFingerDrag
	}
	
	/// <summary>
	/// The enumerations for touch rotation mode
	/// </summary>	
	public enum TouchRotationType {
		Disabled, OneFingerDrag, TwoFingerDrag, ThreeFingerDrag
	}
	
	/// <summary>
	/// The enumerations for mouse zoom mode
	/// </summary>	
	public enum MouseZoomType {
		Disabled, LeftButtonDrag, RightButtonDrag, MiddleButtonDrag, Scroll
	}
	
	/// <summary>
	/// The enumerations for mouse pan mode
	/// </summary>	
	public enum MousePanType {
		Disabled, LeftButtonDrag, RightButtonDrag, MiddleButtonDrag
	}
	
	/// <summary>
	/// The enumerations for mouse rotation mode
	/// </summary>	
	public enum MouseRotateType {
		Disabled, LeftButtonDrag, RightButtonDrag, MiddleButtonDrag
	}

	/// <summary>
	/// The enumerations for the type of movement plane.
	/// If fixed, the movement plane will remain the same as the original movement plane when rotated.
	/// If dynamic, the movement plane will rotate with the camera.
	/// </summary>
	public enum MovementPlaneType {
		Static, Dynamic
	}

}