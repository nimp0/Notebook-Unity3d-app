using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PaintLine
{
    public GameObject go;
    public LineRenderer lineRenderer;
    
    private Vector3[] pointsVectors;
    public float[] pointsRadii;
    private float boundsRadius;

    public Color Color
    {
        get
        {
            return Color.yellow;
        }
    }

    public float Width
    {
        get
        {
            return 0.3f;
        }
    }

    public PaintLine()
    {

    }

    public void Create(GameObject parent)
    {
        go = new GameObject("PaintLine(Clone)");
        go.transform.SetParent(parent.transform);

        lineRenderer = go.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startColor = lineRenderer.endColor = Color;
        lineRenderer.startWidth = lineRenderer.endWidth = Width/(parent.transform.localScale.x * 20);
        lineRenderer.material = new Material(Shader.Find("UI/Default"));
        lineRenderer.material.SetFloat("_InvFade", 3);
        lineRenderer.material.SetColor("_TintColor", Color);
        lineRenderer.useWorldSpace = false;
        //line.numCornerVertices = 30;
        lineRenderer.numCapVertices = 30;
        lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer.generateLightingData = false;

        go.transform.localPosition = parent.transform.localPosition; //Vector3.forward * Sizes.BuildingParameters.blockWidth;

        //var g = go.GetComponent<MeshRenderer>()?.gameObject;
        //GameObject.Destroy(g);
    }

    public void Move(Vector3 localPosition)
    {

    }

    public bool OverlapPoint(Vector3 localPoint)
    {
        Vector3 localBoundsCenter = go.transform.InverseTransformPoint(lineRenderer.bounds.center);
        //localPoint = localPoint + Vector3.forward * Sizes.Notebook.offsetFromWall;

        if (Vector3.Distance(localPoint, localBoundsCenter) > boundsRadius)
        {
            return false;
        }

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            //Debug.DrawLine(localPoint + go.transform.position, lineRenderer.GetPosition(i) + go.transform.position, Color.red, 10);
            //Debug.DrawLine(lineRenderer.GetPosition(i) + go.transform.position, lineRenderer.GetPosition(i) + Vector3.up * pointsRadii[i] + go.transform.position, Color.blue, 10);

            if (Vector3.Distance(localPoint, lineRenderer.GetPosition(i)) < pointsRadii[i])
            {
                return true;
            }
        }

        return false;
    }

    public void AddPoint(Vector3 localPoint)
    {
        //localPoint = localPoint + Vector3.forward * Sizes.Notebook.offsetFromWall;

        if (lineRenderer.positionCount == 0)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, localPoint);
        }

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, localPoint);
    }

    public void Finilize()
    {
        lineRenderer.Simplify(Sizes.Notebook.minPointLineThreshold / 10);
        CalculateBoundsRadius();
        CalculatePointCharacteristics();
        FillGaps();
        CalculatePointCharacteristics();
    }

    public List<Vector3> GetLinePointList()
    {
        List<Vector3> l = new List<Vector3>();
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            l.Add(lineRenderer.GetPosition(i));
        }

        return l;
    }

    public void CalculateBoundsRadius()
    {
        boundsRadius = lineRenderer.bounds.extents.magnitude;
    }

    public void CalculatePointCharacteristics()
    {
        pointsVectors = new Vector3[lineRenderer.positionCount];
        pointsRadii = new float[lineRenderer.positionCount];

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            if (lineRenderer.positionCount == 2 && Vector3.Equals(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)))
            {
                pointsVectors[i] = Vector3.up * Sizes.Notebook.maxPointLineTreshold;
                //Debug.Log(pointsVectors[i]);
            }
            else
            {
                if (i == lineRenderer.positionCount - 1)
                {
                    pointsVectors[i] = lineRenderer.GetPosition(i) - lineRenderer.GetPosition(i - 1);
                }
                else
                {
                    pointsVectors[i] = lineRenderer.GetPosition(i + 1) - lineRenderer.GetPosition(i);
                }
            }

            pointsRadii[i] = pointsVectors[i].magnitude;
        }
    }

    public void FillGaps()
    {
        for (int i = 0, c = 0; i < pointsRadii.Length - 1; i++, c++)
        {
            if (pointsRadii[i] > Sizes.Notebook.maxPointLineTreshold)
            {
                int amountOfPointsToFill = Mathf.CeilToInt(pointsRadii[i] / Sizes.Notebook.maxPointLineTreshold);
                Vector3 start = lineRenderer.GetPosition(c);
                //Debug.Log("we have threshold " + maxThreshold + " and we have radius " + pointsRadii[i] + " and we are going to divide it by " + amountOfPointsToFill);
                for (int j = 1; j < amountOfPointsToFill; j++)
                {
                    float l = (float)j / (amountOfPointsToFill - 1);
                    Vector3 pointToFill = pointsVectors[i] * l + start;
                    //Debug.DrawLine(start, pointToFill, Color.green, 10);
                    InsertPoint(c + 1, pointToFill);
                    c++;
                }
            }
        }
    }

    void InsertPoint(int i, Vector3 point)
    {
        Vector3[] oldPoints = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(oldPoints);
        lineRenderer.positionCount++;
        var list = oldPoints.ToList();
        list.Insert(i, point);
        Vector3[] newPoints = list.ToArray();
        lineRenderer.SetPositions(newPoints);
    }
}

