using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class TextPanel
{
    public GameObject go;
    public TMP_InputField inputField;

    private BoxCollider boxCollider;
    private float targetFontSize;
    private Vector3[] cornerPoints;
    private RectTransform rtBorders;
    private TMP_Text proxyText;

    private string mark = "<mark=#ffff09aa></mark>";
    private string selectedText = "";
    private string newText;

    public void Create(GameObject parent)
    {
        go = UnityEngine.Object.Instantiate(Resources.Load("TextPanel")) as GameObject;
        go.transform.SetParent(parent.transform);
        go.transform.rotation = Quaternion.LookRotation(parent.transform.forward);
        go.transform.localScale = parent.transform.localScale / 3000;

        proxyText = go.GetComponentInChildren<TMP_Text>();
        inputField = go.GetComponentInChildren<TMP_InputField>();
        rtBorders = go.transform.GetChild(0).GetComponent<RectTransform>().GetChild(1).GetComponent<RectTransform>();

        rtBorders.Find("TopSide").GetComponent<RectTransform>().sizeDelta = new Vector2(0, parent.transform.localScale.y * 25);
        rtBorders.Find("BottomSide").GetComponent<RectTransform>().sizeDelta = new Vector2(0, parent.transform.localScale.y * 5);
        rtBorders.Find("RightSide").GetComponent<RectTransform>().sizeDelta = new Vector2(parent.transform.localScale.x * 5, 0);
        rtBorders.Find("LeftSide").GetComponent<RectTransform>().sizeDelta = new Vector2(parent.transform.localScale.x * 5, 0);

        //proxyText.fontSize = inputField.pointSize = parent.transform.localScale.x / 40;
        //rtBorders.Find("TopSide").GetComponent<RectTransform>().sizeDelta = new Vector2(0, parent.transform.localScale.y / 100);
        //rtBorders.Find("BottomSide").GetComponent<RectTransform>().sizeDelta = new Vector2(0, parent.transform.localScale.y / 400);
        //rtBorders.Find("RightSide").GetComponent<RectTransform>().sizeDelta = new Vector2(parent.transform.localScale.x / 400, 0);
        //rtBorders.Find("LeftSide").GetComponent<RectTransform>().sizeDelta = new Vector2(parent.transform.localScale.x / 400, 0);

        boxCollider = go.AddComponent<BoxCollider>();
        inputField.onFocusSelectAll = false;
        proxyText.characterSpacing = 1;
        go.layer = LayerMasksList.TextPanelMask;
        Focus();
        inputField.onValueChanged.AddListener(delegate { OnAnyChange(); });

        cornerPoints = new Vector3[4];
    }

    public void Focus()
    {
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void OnAnyChange()
    {
        RecalculateColliderSize();
        UpdateProxyText();
    }

    void RecalculateColliderSize()
    {
        boxCollider.size = new Vector3(proxyText.rectTransform.sizeDelta.x, proxyText.rectTransform.sizeDelta.y, 1f);
        boxCollider.center = proxyText.bounds.center;
    }

    public void Move(Vector2 localPosition)
    {
        go.transform.localPosition = (Vector3)localPosition + Vector3.forward * (Sizes.BuildingParameters.blockWidth / 2 + Sizes.Notebook.offsetFromWall);
    }

    void UpdateProxyText()
    {
        proxyText.text = inputField.text;

        if (inputField.isFocused)
        {
            //char fakeSpace = ',';

            if (inputField.text.Length > 0)
            {
                //proxyText.text = inputField.text.Replace(' ', fakeSpace);
                string newLine = "\n";

                if (inputField.text.Substring(inputField.text.Length - newLine.Length) == newLine)
                {
                    proxyText.text += newLine;
                }
            }
        }
    }

    public string HighlightText(string text, int beginningOfSelection, int endOfSelection)
    {
        string markedText = "";
        string halfCleanedText = text.Replace("<mark=#ffff09aa>", "");
        string cleanedText = halfCleanedText.Replace("</mark>", "");

        if ((beginningOfSelection == 0 && endOfSelection == text.Length) || (beginningOfSelection == text.Length && endOfSelection == 0))
        {
            if (endOfSelection > beginningOfSelection)
            {
                selectedText = cleanedText.Substring(beginningOfSelection, endOfSelection - beginningOfSelection);
            }

            else if (endOfSelection < beginningOfSelection)
            {
                selectedText = cleanedText.Substring(endOfSelection, beginningOfSelection - endOfSelection);
            }

            markedText = mark.Insert(16, selectedText);

            /*if (text.Contains(markedText))
            {
                markedText = selectedText;
            }*/
        }

        else
        {
            markedText = text;
        }

        if (text.Length.Equals(markedText.Length) && ((beginningOfSelection == 0 && endOfSelection == cleanedText.Length) || (beginningOfSelection == cleanedText.Length && endOfSelection == 0)))
        {
            text = cleanedText;
            markedText = text;
        }

        return markedText;
    }

    public void SetCaretPosition(TMP_InputField inputField, int caretPos)
    {
        string tempText = inputField.text;
        inputField.text = inputField.text.Substring(0, caretPos);
        inputField.MoveTextEnd(false);
        inputField.text = tempText;
    }

    //public void RecalculateCornerPoints(GameObject parent)
    //{
    //    Rect rect = rtBorders.rect;
    //    rect.size = proxyText.rectTransform.sizeDelta;

    //    Vector3 a = new Vector3(rect.xMin, rect.yMax) / 1000;
    //    Vector3 b = new Vector3(rect.xMin, rect.yMin) / 1000;
    //    Vector3 c = new Vector3(rect.xMax, rect.yMin) / 1000;
    //    Vector3 d = new Vector3(rect.xMax, rect.yMax) / 1000;

    //    Vector3 leftUpCorner = new Vector3(a.x, a.y, parent.transform.localPosition.z) + rtBorders.position;
    //    Vector3 leftDownCorner = new Vector3(b.x, b.y, parent.transform.localPosition.z) + rtBorders.position;
    //    Vector3 rightDownCorner = new Vector3(c.x, c.y, parent.transform.localPosition.z) + rtBorders.position;
    //    Vector3 rightUpCorner = new Vector3(d.x, d.y, parent.transform.localPosition.z) + rtBorders.position;

    //    Vector3 projectedLUCorner = new Vector3(ProjectPointOnTransformAxes(leftUpCorner, parent.transform).x, ProjectPointOnTransformAxes(leftUpCorner, parent.transform).y, leftUpCorner.z);
    //    Vector3 projectedLDCorner = new Vector3(ProjectPointOnTransformAxes(leftDownCorner, parent.transform).x, ProjectPointOnTransformAxes(leftDownCorner, parent.transform).y, leftDownCorner.z);
    //    Vector3 projectedRDCorner = new Vector3(ProjectPointOnTransformAxes(rightDownCorner, parent.transform).x, ProjectPointOnTransformAxes(rightDownCorner, parent.transform).y, rightDownCorner.z);
    //    Vector3 projectedRUCorner = new Vector3(ProjectPointOnTransformAxes(rightUpCorner, parent.transform).x, ProjectPointOnTransformAxes(rightUpCorner, parent.transform).y, rightUpCorner.z);

    //    /*Debug.DrawLine(projectedLUCorner, projectedLDCorner, Color.red, 20);
    //    Debug.DrawLine(projectedLDCorner, projectedRDCorner, Color.red, 20);
    //    Debug.DrawLine(projectedRDCorner, projectedRUCorner, Color.red, 20);
    //    Debug.DrawLine(projectedRUCorner, projectedLUCorner, Color.red, 20);*/

    //    cornerPoints[0] = projectedLUCorner;
    //    cornerPoints[1] = projectedLDCorner;
    //    cornerPoints[2] = projectedRDCorner;
    //    cornerPoints[3] = projectedRUCorner;
    //}

    public Vector2 ProjectPointOnTransformAxes(Vector3 localPoint, Transform transform)
    {
        Vector3 rightAxisVector = Vector3.Project(localPoint, transform.right) + new Vector3(0, 0, localPoint.z / 2);
        Vector3 upAxisVector = Vector3.Project(localPoint, transform.up) + new Vector3(0, 0, localPoint.z / 2);
        Vector2 projectedPoint = new Vector2(rightAxisVector.x, upAxisVector.y);

        return projectedPoint;
    }
}



