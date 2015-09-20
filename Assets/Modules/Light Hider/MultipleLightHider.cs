using UnityEngine;
using System.Collections;

/// <summary>
/// Hides the referenced lights from the camera to which this script is attached.
/// </summary>
public class MultipleLightHider : MonoBehaviour {

	/// <summary>
	/// The lights to hide.
	/// </summary>
	public Light[] LightsToHide;

	/// <summary>
	/// The light enabled values.
	/// </summary>
	private bool[] lightEnabledValues;

	/// <summary>
	/// Raises the pre cull event.
	/// </summary>
	void OnPreCull () {
		// Construct light enabled values array
		if (lightEnabledValues == null)
			lightEnabledValues = new bool[LightsToHide.Length];

		// Loop through lights disabling them temporarily but first storing their enabled value
		for (int i=0; i<LightsToHide.Length; i++) {
			lightEnabledValues[i] = LightsToHide[i].enabled;
			LightsToHide[i].enabled = false;
		}
	}

	/// <summary>
	/// Raises the post render event.
	/// </summary>
	void OnPostRender () {
		// Apply old enabled values back to the lights
		for (int i=0; i<LightsToHide.Length; i++)
			LightsToHide [i].enabled = lightEnabledValues [i];
	}
	
}