using UnityEngine;
using System.Collections;

/// <summary>
/// Grid renderer.
/// Renders a grid to the camera to which this script is attached.
/// </summary>
public class MovementPlaneRenderer : MonoBehaviour {
	
	/// <summary>
	/// The transformation matrix of the grid.
	/// </summary>
	public Transform TransformationMatrix;
	
	/// <summary>
	/// The grid size along the x axis before transformation (in world space units).
	/// </summary>
	public int GridWidth;
	
	/// <summary>
	/// The grid size along the y axis before transformation (in world space units).
	/// </summary>
	public int GridHeight;
	
	/// <summary>
	/// The grid step (in world space units).
	/// </summary>
	public float Step;
	
	/// <summary>
	/// The line material to use.
	/// </summary>
    private Material lineMaterial;
	
	/// <summary>
	/// The color of the larger grid lines.
	/// </summary>
	private Color color = new Color(1f,1f,1f,0.3f);
	
	/// <summary>
	/// Temporary variables.
	/// </summary>
	private float startXsm, startZ, startYsm, startOffsetX, startOffsetY, startXlg, startYlg, bigStep, bigGridHeight, bigGridWidth, bigStartOffsetX, bigStartOffsetY;

	/// <summary>
	/// The pan x and y values.
	/// </summary>
	public float PanX, PanY;
	
	/// <summary>
	/// Creates the line material.
	/// </summary>
	void CreateLineMaterial() 
	{
		// Construct the shader for the line
		if( !lineMaterial ) {
            lineMaterial = new Material(Shader.Find("Unlit/ColorOnly"));
            lineMaterial.SetColor("_Color", color);
		}
	}
	
	/// <summary>
	/// Renders the grid.
	/// </summary>
	void OnPostRender() 
	{
		if (!enabled)
			return;
		
		// Initialize grid starting positions and variables
		startXsm = - GridWidth / 2f;
		startYsm = - GridHeight / 2f;
        startZ = 0;

        bigGridHeight = GridHeight * 5f;
        bigGridWidth = GridWidth * 5f;
        bigStep = Step * 5f;
        startXlg = -bigGridHeight / 2f;
        startYlg = -bigGridWidth / 2f;

        // Initialize the offsets
        startOffsetX = (PanX % bigStep);
        startOffsetY = (PanY % bigStep);

        // Add the offsets
        startXsm += startOffsetX;
        startYsm += startOffsetY;
		
		// Create the line material if not already created
		CreateLineMaterial();
		
		// Set the line material
        lineMaterial.SetPass(0);
		
		GL.PushMatrix ();

		GL.MultMatrix (TransformationMatrix.localToWorldMatrix);
		
		GL.Begin( GL.LINES );
		
		// Set color for grid lines
		GL.Color(color);

        // Initialize the offsets
        bigStartOffsetX = (PanX % bigStep);
        bigStartOffsetY = (PanY % bigStep);

        // Add the offsets
        startXlg += bigStartOffsetX;
        startYlg += bigStartOffsetY;

        // Draw big grid lines
        for (float i = 0; i <= bigGridHeight + 0.1f; i += bigStep) {
            GL.Vertex3(startXlg, startYlg + i, startZ);
            GL.Vertex3(startXlg + bigGridWidth, startYlg + i, startZ);

            GL.Vertex3(startXlg + i, startYlg, startZ);
            GL.Vertex3(startXlg + i, startYlg + bigGridHeight, startZ);
        }

        // Draw grid lines
        for (float i = 0; i <= GridHeight + 0.1f; i += Step)
        {
            GL.Vertex3(startXsm, startYsm + i, startZ);
            GL.Vertex3(startXsm + GridWidth, startYsm + i, startZ);

            GL.Vertex3(startXsm + i, startYsm, startZ);
            GL.Vertex3(startXsm + i, startYsm + GridHeight, startZ);
        }
		
		GL.End();
		
		GL.PopMatrix ();
	}
}

