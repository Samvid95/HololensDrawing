using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Created this class to make sure we have all the events stored and not just the line.
/// 
/// This was created as a goal for having a finer undo-redo functionality. IT IS INCOMPLETE! USE IT WITH DISCRETION.
/// </summary>
public class AllEvents
{
    public GameObject line;
    public Color color;


}
public class InputManager : MonoBehaviour
{
    public GameObject lineObject;
    private GameObject currentLine;

    public int colorNum = -1;
    public Color linecolor;

    private LineRenderer lineRenderer;
    private List<Vector3> linePositions;

    private List<GameObject> lines;
    private List<GameObject> lines_redo;

    private List<AllEvents> events;

    /// <summary>
    /// It take the input as a number from button(Red, Green, Blue, White) and assigns to the lineColor variable/
    /// </summary>
    /// <param name="num"> Int value. Used to setup lineColor</param>
    public void ColorSelection(int num)
    {
        Debug.Log("Color is: " + num);
        colorNum = num;
        if (colorNum == 0)
        {
            linecolor = Color.red;
        }
        else if (colorNum == 1)
        {
            linecolor = Color.green;
        }
        else if (colorNum == 2)
        {
            linecolor = Color.blue;
        }
        else
        {
            linecolor = Color.white;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        colorNum = -1;
        linePositions = new List<Vector3>();
        lines = new List<GameObject>();
        lines_redo = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get all the detected controllers
        foreach (var controller in CoreServices.InputSystem.DetectedControllers)
        {
            if(controller.InputSource.SourceName == "Right Hand")
            {
                //Only get select input and hand position input from the right hand.
                MixedRealityInteractionMapping[] inputMappings = controller.Interactions;
                MixedRealityInteractionMapping spatialInput = inputMappings[0];
                MixedRealityInteractionMapping selectInput = inputMappings[2];
                
                //Draw only if you are selecting something! 
                if (selectInput.BoolData)
                {
                    if (!currentLine)
                    {
                        CreateLine(spatialInput.PositionData);
                        lines.Add(currentLine);
                    }
                    if(Vector3.Distance(spatialInput.PositionData, linePositions[linePositions.Count - 1]) > 0.05f)
                    {
                        UpdateLine(spatialInput.PositionData);
                    }

                }
                else
                {
                    currentLine = null;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            undo();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            redo();
        }

    }
    /// <summary>
    /// Whenever the user clics ones I use this to initialize a new line renderer and create a list to keep track of all the input 
    /// </summary>
    /// <param name="initPosition">vector3; initial position of the click</param>
    void CreateLine(Vector3 initPosition)
    {
        currentLine = Instantiate(lineObject, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        Debug.Log("Currently drawing: " + linecolor.ToString());
        /*AllEvents event = new AllEvents();
        event.color = linecolor;
        */
        lineRenderer.material.color = linecolor;
        linePositions.Clear();
        linePositions.Add(initPosition);
        linePositions.Add(initPosition);

        lineRenderer.SetPosition(0, linePositions[0]);
        lineRenderer.SetPosition(1, linePositions[1]);
    }

    /// <summary>
    /// After initialization this function addes new position to the line renderer. 
    /// </summary>
    /// <param name="updatePosition">vector3; update the position of the lines</param>
    void UpdateLine(Vector3 updatePosition)
    {
        linePositions.Add(updatePosition);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, updatePosition);
    }

    /// <summary>
    /// When you press 'Z' undo happens and this is the sequence of events
    /// - Get the top line element
    /// - Remove that element
    /// - Make it disappear
    /// - Add it in another list so we can use it for redo later
    /// </summary>
    void undo()
    {
        GameObject top_line = lines[lines.Count - 1];
        lines.RemoveAt(lines.Count - 1);
        top_line.SetActive(false);
        lines_redo.Add(top_line);
    }

    /// <summary>
    /// Same as the Undo just in reverse. Straight forward code.
    /// </summary>
    void redo()
    {
        GameObject top_line = lines_redo[lines_redo.Count - 1];
        lines_redo.RemoveAt(lines_redo.Count - 1);
        top_line.SetActive(true);
        lines.Add(top_line);
    }




}
