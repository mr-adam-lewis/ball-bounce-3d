using UnityEngine;
using System.Collections;

/// <summary>
/// Hides the referenced light from the camera to which this script is attached.
/// </summary>
public class LightHider : MonoBehaviour {

	/// <summary>
	/// The light to hide.
	/// </summary>
	public Light LightToHide;

	/// <summary>
	/// The light enabled value.
	/// </summary>
	private bool lightEnabledValue;

	/// <summary>
	/// Raises the pre cull event.
	/// </summary>
	void OnPreCull () {
		lightEnabledValue = LightToHide.enabled;
		LightToHide.enabled = false;
	}

	/// <summary>
	/// Raises the post render event.
	/// </summary>
	void OnPostRender () {
		LightToHide.enabled = lightEnabledValue;
	}
	
}