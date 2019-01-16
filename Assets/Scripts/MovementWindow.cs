using System.Collections.Generic;
using UnityEngine;

public class MovementWindow
{
    public GameObject go;

    private RectTransform rt;
    public Vector3[] cornerPoints;
    private Vector3 pivot;
    private Vector3 previousPivot;
    private List<Transform> selectedTransforms;
    private Vector3 prevLP;

    public MovementWindow(GameObject parent)
    {
        go = UnityEngine.Object.Instantiate(Resources.Load("MovementWindow")) as GameObject;
        go.transform.SetParent(parent.transform);
        rt = go.GetComponent<RectTransform>();
        go.transform.rotation = Quaternion.LookRotation(parent.transform.forward);
        go.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        go.transform.localScale = parent.transform.localScale;
        pivot = rt.pivot;
        selectedTransforms = new List<Transform>();
    }

    public void ChangeTransformParentBySelectedTransforms(Transform newParent)
    {
        if (selectedTransforms.Count > 0)
        {
            for (int i = 0; i < selectedTransforms.Count; i++)
            {
                selectedTransforms[i].SetParent(newParent);
            }

            selectedTransforms.Clear();
        }
    }

    public void MoveWindowByPoint(Vector3 localPoint)
    {
        rt.localPosition = localPoint;
    }

    public void ChangeLocalPositionOfTransformChildsRelativelyPivot(Transform parent, Vector3 localPos)
    {
        //Debug.Log(previousPivot);
        //Debug.Log(pivot);
        Vector3 worldPos = rt.position;

        for (int i = 0; i < selectedTransforms.Count; i++)
        {
            //Vector3 localCenter = new Vector3(ProjectPointOnTransformAxes(selectedTransforms[i].localPosition, rt.gameObject.transform).x, ProjectPointOnTransformAxes(selectedTransforms[i].localPosition, rt.gameObject.transform).y, rt.localPosition.z);
            //Debug.Log(localCenter);

            float rectDistanceBetweenCornersOfXSide = Vector2.Distance(cornerPoints[1], cornerPoints[2]);
            float rectDistanceBetweenCornersOfYSide = Vector2.Distance(cornerPoints[0], cornerPoints[1]);
            //Debug.Log(rectDistanceBetweenCornersOfXSide);
            //Debug.Log(rectDistanceBetweenCornersOfYSide);

            //float distanceBetweenCornerAndLocalCenterByXSide = Vector2.Distance(cornerPoints[1], new Vector2(selectedTransforms[i].localPosition.x, cornerPoints[1].y));
            //float distanceBetweenCornerAndLocalCenterByYSide = Vector2.Distance(cornerPoints[1], new Vector2(cornerPoints[1].x, selectedTransforms[i].localPosition.y));

            float x = 0;
            float y = 0;

            /*if (previousPivot == new Vector3(0,1))
            {
                //Debug.DrawLine(cornerPoints[0], new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[0].y) + new Vector3(selectedTransforms[i].position.x, 0, localPos.z), Color.red, 20);
                //Debug.DrawLine(cornerPoints[0], new Vector3(cornerPoints[0].x, selectedTransforms[i].localPosition.y) + new Vector3(0, selectedTransforms[i].position.y, localPos.z), Color.red, 20);

                //Vector3 a = new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[0].y);// + new Vector3(selectedTransforms[i].position.x, 0, localPos.z);
                //Vector3 b = new Vector3(cornerPoints[0].x, selectedTransforms[i].localPosition.y);// + new Vector3(0, selectedTransforms[i].position.y, localPos.z);

                x = ((prevLP.x) * selectedTransforms[i].localPosition.x) / (cornerPoints[0].x - worldPos.x);
                y = ((prevLP.y) * selectedTransforms[i].localPosition.y) / (cornerPoints[0].y - worldPos.y);
                Debug.Log(cornerPoints[0]);

                //Debug.DrawLine(cornerPoints[0], newLocalPos, Color.red, 20);
                //Debug.DrawLine(cornerPoints[0], (new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[0].y) + new Vector3(cornerPoints[0].x, selectedTransforms[i].localPosition.y)), Color.red, 20);

                //x = Vector2.Distance(cornerPoints[0], new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[0].y) + new Vector3(selectedTransforms[i].position.x, 0, localPos.z));
                //y = Vector2.Distance(cornerPoints[0], new Vector3(cornerPoints[0].x, selectedTransforms[i].localPosition.y) + new Vector3(0, selectedTransforms[i].position.y, localPos.z));
            }

            else if (previousPivot == new Vector3(0,0))
            {
                Debug.DrawLine(cornerPoints[1], new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[1].y) + new Vector3(selectedTransforms[i].position.x, 0, localPos.z), Color.red, 20);
                Debug.DrawLine(cornerPoints[1], new Vector3(cornerPoints[1].x, selectedTransforms[i].localPosition.y) + new Vector3(0, selectedTransforms[i].position.y, localPos.z), Color.red, 20);
                Debug.DrawLine(cornerPoints[1], (new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[1].y) + new Vector3(cornerPoints[1].x, selectedTransforms[i].localPosition.y)), Color.red, 20);

                x = Vector2.Distance(cornerPoints[1], new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[1].y) + new Vector3(selectedTransforms[i].position.x, 0, localPos.z));
                y = Vector2.Distance(cornerPoints[1], new Vector3(cornerPoints[1].x, selectedTransforms[i].localPosition.y) + new Vector3(0, selectedTransforms[i].position.y, localPos.z));
            }

            else if (previousPivot == new Vector3(1, 0))
            {
                Debug.DrawLine(cornerPoints[2], new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[2].y) + new Vector3(selectedTransforms[i].position.x, 0, localPos.z), Color.red, 20);
                Debug.DrawLine(cornerPoints[2], new Vector3(cornerPoints[2].x, selectedTransforms[i].localPosition.y) + new Vector3(0, selectedTransforms[i].position.y, localPos.z), Color.red, 20);
                Debug.DrawLine(cornerPoints[2], (new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[2].y) + new Vector3(cornerPoints[2].x, selectedTransforms[i].localPosition.y)), Color.red, 20);

                x = Vector2.Distance(cornerPoints[2], new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[2].y) + new Vector3(selectedTransforms[i].position.x, 0, localPos.z));
                y = Vector2.Distance(cornerPoints[2], new Vector3(cornerPoints[2].x, selectedTransforms[i].localPosition.y) + new Vector3(0, selectedTransforms[i].position.y, localPos.z));
            }

            else if (previousPivot == new Vector3(1, 1))
            {
                Debug.DrawLine(cornerPoints[3], new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[3].y) + new Vector3(selectedTransforms[i].position.x, 0, localPos.z), Color.red, 20);
                Debug.DrawLine(cornerPoints[3], new Vector3(cornerPoints[3].x, selectedTransforms[i].localPosition.y) + new Vector3(0, selectedTransforms[i].position.y, localPos.z), Color.red, 20);
                Debug.DrawLine(cornerPoints[3], (new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[3].y) + new Vector3(cornerPoints[3].x, selectedTransforms[i].localPosition.y)), Color.red, 20);

                x = Vector2.Distance(cornerPoints[3], new Vector3(selectedTransforms[i].localPosition.x, cornerPoints[3].y) + new Vector3(selectedTransforms[i].position.x, 0, localPos.z));
                y = Vector2.Distance(cornerPoints[3], new Vector3(cornerPoints[3].x, selectedTransforms[i].localPosition.y) + new Vector3(0, selectedTransforms[i].position.y, localPos.z));
            }*/

            Debug.DrawLine(cornerPoints[0], new Vector3(Math.ProjectPointOnTransformAxes(selectedTransforms[i].transform.position, parent).x, Math.ProjectPointOnTransformAxes(selectedTransforms[i].transform.position, parent).y, localPos.z), Color.red, 200);
            Vector3 a = new Vector3(Math.ProjectPointOnTransformAxes(selectedTransforms[i].transform.position, parent).x, Math.ProjectPointOnTransformAxes(selectedTransforms[i].transform.position, parent).y, localPos.z);
            //x = Vector2.Distance(cornerPoints[0], new Vector2(a.x, cornerPoints[0].y));
            //y = Vector2.Distance(cornerPoints[0], new Vector2(cornerPoints[0].x, a.y));

            Debug.DrawLine(cornerPoints[0], prevLP, Color.red, 20);

            x = (prevLP.x * selectedTransforms[i].localPosition.x) / cornerPoints[0].x;
            y = (prevLP.y * selectedTransforms[i].localPosition.y) / cornerPoints[0].y;
            Vector3 newLocalPos = new Vector3(-x, y);
            selectedTransforms[i].localPosition = newLocalPos;

            //selectedTransforms[i].localPosition = new Vector3(ProjectPointOnTransformAxes(newLocalPos, parent).x, ProjectPointOnTransformAxes(newLocalPos, parent).y);
            //Debug.Log(new Vector3(pivotX, pivotY, localCenter.z));
        }
    }

    public void SetPivotByLocalPoint(Vector3 localPoint)
    {
        //if (Math.IsLeftUp(localPoint, cornerPoints[0]) && Math.IsLeftDown(localPoint, cornerPoints[1]) && Math.IsRightDown(localPoint, cornerPoints[2]) && Math.IsRightUp(localPoint, cornerPoints[3]))
        {
            float rectDistanceBetweenCornersOfXSide = Vector2.Distance(cornerPoints[1], cornerPoints[2]);
            float rectDistanceBetweenCornersOfYSide = Vector2.Distance(cornerPoints[0], cornerPoints[1]);
            float distanceBetweenCornerAndLocalCenterByXSide = Vector2.Distance(cornerPoints[1], new Vector2(localPoint.x, cornerPoints[1].y));
            float distanceBetweenCornerAndLocalCenterByYSide = Vector2.Distance(cornerPoints[1], new Vector2(cornerPoints[1].x, localPoint.y));
            //Debug.DrawLine(cornerPoints[1], new Vector3(localCenter.x, cornerPoints[1].y) + new Vector3(0,0,localPoint.z), Color.red, 10);
            //Debug.DrawLine(cornerPoints[1], new Vector3(cornerPoints[1].x, localCenter.y) + new Vector3(0, 0, localPoint.z), Color.red, 10);
            float pivotX = distanceBetweenCornerAndLocalCenterByXSide / rectDistanceBetweenCornersOfXSide;
            float pivotY = distanceBetweenCornerAndLocalCenterByYSide / rectDistanceBetweenCornersOfYSide;

            pivot = new Vector2(pivotX, pivotY);
            rt.pivot = pivot;
            prevLP = localPoint;
        }
    }

    public void DetectForTransformsInMW(Transform potentialChild, Vector3[] transformPoints)
    {
        for (int i = 0; i < transformPoints.Length; i++)
        {
            if (Math.IsLeftUp(transformPoints[i], cornerPoints[0]) && Math.IsLeftDown(transformPoints[i], cornerPoints[1]) && Math.IsRightDown(transformPoints[i], cornerPoints[2]) && Math.IsRightUp(transformPoints[i], cornerPoints[3]))
            {
                selectedTransforms.Add(potentialChild);
                potentialChild.SetParent(go.transform);
                //return true;
            }
        }

        //return false;
    }

    public void FinalizeSelecting(Vector3 localPoint, Transform parent)
    {
        Vector3 worldPos = rt.position;
        Rect rect = rt.rect;
        cornerPoints = new Vector3[4];

        Vector3 a = new Vector3(rect.xMin, rect.yMax);
        Vector3 b = new Vector3(rect.xMin, rect.yMin);
        Vector3 c = new Vector3(rect.xMax, rect.yMin);
        Vector3 d = new Vector3(rect.xMax, rect.yMax);

        /*Debug.DrawLine(a + worldPos, b + worldPos, Color.red, 20);
        Debug.DrawLine(b + worldPos, c + worldPos, Color.red, 20);
        Debug.DrawLine(c + worldPos, d + worldPos, Color.red, 20);
        Debug.DrawLine(d + worldPos, a + worldPos, Color.red, 20);*/

        Vector3 leftUpCorner = new Vector3(a.x, a.y, localPoint.z) + worldPos;
        Vector3 leftDownCorner = new Vector3(b.x, b.y, localPoint.z) + worldPos;
        Vector3 rightDownCorner = new Vector3(c.x, c.y, localPoint.z) + worldPos;
        Vector3 rightUpCorner = new Vector3(d.x, d.y, localPoint.z) + worldPos;

        Vector3 projectedLUCorner = new Vector3(Math.ProjectPointOnTransformAxes(leftUpCorner, parent).x, Math.ProjectPointOnTransformAxes(leftUpCorner, parent).y, localPoint.z);
        Vector3 projectedLDCorner = new Vector3(Math.ProjectPointOnTransformAxes(leftDownCorner, parent).x, Math.ProjectPointOnTransformAxes(leftDownCorner, parent).y, localPoint.z);
        Vector3 projectedRDCorner = new Vector3(Math.ProjectPointOnTransformAxes(rightDownCorner, parent).x, Math.ProjectPointOnTransformAxes(rightDownCorner, parent).y, localPoint.z);
        Vector3 projectedRUCorner = new Vector3(Math.ProjectPointOnTransformAxes(rightUpCorner, parent).x, Math.ProjectPointOnTransformAxes(rightUpCorner, parent).y, localPoint.z);

        //Debug.DrawLine(projectedLUCorner, projectedLDCorner, Color.red, 20);
        //Debug.DrawLine(projectedLDCorner, projectedRDCorner, Color.red, 20);
        //Debug.DrawLine(projectedRDCorner, projectedRUCorner, Color.red, 20);
        //Debug.DrawLine(projectedRUCorner, projectedLUCorner, Color.red, 20);

        cornerPoints[0] = projectedLUCorner;
        cornerPoints[1] = projectedLDCorner;
        cornerPoints[2] = projectedRDCorner;
        cornerPoints[3] = projectedRUCorner;

        //Debug.DrawLine(cornerPoints[0], new Vector3(ProjectPointOnTransformAxes(fuck.transform.position, parent).x, ProjectPointOnTransformAxes(fuck.transform.position, parent).y, localPoint.z), Color.red, 200);
    }

    public void RecalculateWindowSizeByLocalPoint(Vector3 localPoint)
    {
        Vector3 localCenter = rt.position;
        Rect rect = rt.rect;
        Vector3 newSize;

        /*Debug.Log(localCenter);
        Debug.Log(localPoint);
        Debug.DrawLine(localCenter, localPoint, Color.red, 10);*/

        if (Math.IsLeftUp(localCenter, localPoint))
        {
            //Debug.Log("leftup true");
            pivot = new Vector2(1, 0);
            rt.pivot = pivot;

            Vector3 leftUpVertixPos = new Vector3(rect.xMin, rect.yMin, localPoint.z);
            leftUpVertixPos = localPoint;

            newSize = new Vector3(-leftUpVertixPos.x, leftUpVertixPos.y, leftUpVertixPos.z);
            rt.sizeDelta = newSize + new Vector3(localCenter.x, -localCenter.y, localCenter.z);
        }

        else if (Math.IsRightUp(localCenter, localPoint))
        {
            //Debug.Log("rightup true");
            pivot = new Vector2(0, 0);
            rt.pivot = pivot;

            Vector3 rightUpVertixPos = new Vector3(rect.xMax, rect.yMin, localPoint.z);
            rightUpVertixPos = localPoint;

            newSize = new Vector3(rightUpVertixPos.x, rightUpVertixPos.y, rightUpVertixPos.z);
            rt.sizeDelta = newSize - localCenter;
        }

        else if (Math.IsLeftDown(localCenter, localPoint))
        {
            //Debug.Log("leftdown true");
            pivot = new Vector2(1, 1);
            rt.pivot = pivot;

            Vector3 leftDownVertixPos = new Vector3(rect.xMin, rect.yMax, localPoint.z);
            leftDownVertixPos = localPoint;

            newSize = new Vector3(-leftDownVertixPos.x, -leftDownVertixPos.y, leftDownVertixPos.z);
            rt.sizeDelta = newSize + localCenter;
        }

        else if (Math.IsRightDown(localCenter, localPoint))
        {
            //Debug.Log("rightdown true");
            pivot = new Vector2(0, 1);
            rt.pivot = pivot;

            Vector3 rightDownVertixPos = new Vector3(rect.xMax, rect.yMax, localPoint.z);
            rightDownVertixPos = localPoint;

            newSize = new Vector3(rightDownVertixPos.x, -rightDownVertixPos.y, rightDownVertixPos.z);
            rt.sizeDelta = newSize + new Vector3(-localCenter.x, localCenter.y, localCenter.z);
        }

        previousPivot = pivot;
    }

    public void Move(Vector2 localPosition)
    {
        localPosition = (Vector3)localPosition + Vector3.forward * (Sizes.BuildingParameters.blockWidth / 2 + Sizes.Notebook.offsetFromWall);
    }
}
