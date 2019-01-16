using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Math
{
    public static Vector2 ProjectPointOnTransformAxes(Vector3 localPoint, Transform parent)
    {
        Vector3 rightAxisVector = Vector3.Project(localPoint, parent.right) + new Vector3(0, 0, localPoint.z / 2);
        Vector3 upAxisVector = Vector3.Project(localPoint, parent.up) + new Vector3(0, 0, localPoint.z / 2);
        Vector2 projectedPoint = new Vector2(rightAxisVector.x, upAxisVector.y);

        return projectedPoint;
    }

    public static Vector2[] ProjectPointsOnTransformAxes(Vector3[] localPoints, Transform instance)
    {
        Vector2[] localPoints2d = new Vector2[localPoints.Length];

        for (int i = 0; i < localPoints.Length; i++)
        {
            Vector3 right = Vector3.Project(localPoints[i], instance.transform.right) + Vector3.forward / 2;
            Vector3 up = Vector3.Project(localPoints[i], instance.transform.up) + Vector3.forward / 2;
            Vector2 twoDPoint = new Vector2(right.x, up.y);
            localPoints2d[i] = twoDPoint;
        }

        return localPoints2d;
    }

    public static bool IsLeftUp(Vector2 A, Vector2 B)
    {
        return B.x < A.x && B.y > A.y;
    }

    public static bool IsRightUp(Vector2 A, Vector2 B)
    {
        return B.x > A.x && B.y > A.y;
    }

    public static bool IsLeftDown(Vector2 A, Vector2 B)
    {
        return B.x < A.x && B.y < A.y;
    }

    public static bool IsRightDown(Vector2 A, Vector2 B)
    {
        return B.x > A.x && B.y < A.y;
    }
}




