using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreationTest : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject currentLine;

    private LineRenderer lineRenderer;
    private List<Vector3> linePositions;

    private Vector3 tempPosition;

    // Start is called before the first frame update
    void Start()
    {
        linePositions = new List<Vector3>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }
        if (Input.GetMouseButton(0))
        {
            tempPosition += new Vector3(0, 0, 1);
            if (Vector3.Distance(tempPosition,linePositions[linePositions.Count-1]) > 0.1f)
            {
                UpdateLine(tempPosition);
            }
        }
    }

    void CreateLine()
    {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();

        linePositions.Clear();
        linePositions.Add(new Vector3(1, 0, 0));
        linePositions.Add(new Vector3(1, 0, 0));

        lineRenderer.SetPosition(0, linePositions[0]);
        lineRenderer.SetPosition(1, linePositions[1]);
    }

    void UpdateLine(Vector3 newPosition)
    {
        linePositions.Add(newPosition);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPosition);

    }
}
