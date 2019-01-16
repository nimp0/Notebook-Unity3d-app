using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Linq;
using System.Text;

public class TextPanel
{
    public GameObject go;
    public TMP_InputField inputField;

    private TMP_FontAsset fontAsset;
    private BoxCollider boxCollider;
    private float targetFontSize;
    private Vector3[] cornerPoints;
    private RectTransform rtBorders;
    private TMP_Text proxyText;

    public void Create(GameObject parent)
    {
        go = UnityEngine.Object.Instantiate(Resources.Load("TextPanel")) as GameObject;
        proxyText = go.GetComponentInChildren<TMP_Text>();
        inputField = go.GetComponentInChildren<TMP_InputField>();
        rtBorders = go.transform.GetChild(0).GetComponent<RectTransform>().GetChild(1).GetComponent<RectTransform>();
        fontAsset = UnityEngine.Object.Instantiate(Resources.Load("LatinCyrilic UI SDF")) as TMP_FontAsset;
        inputField.fontAsset = proxyText.font = fontAsset;

        go.transform.SetParent(parent.transform);
        go.transform.rotation = Quaternion.LookRotation(parent.transform.forward);
        go.transform.localScale = parent.transform.localScale / 3000;

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
        proxyText.characterSpacing = 2;
        //go.layer = LayerMasksList.TextPanelMask;
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
        TMP_Text text = inputField.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>();

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
                    //text.text += newLine;
                    text.GetComponent<RectTransform>().localPosition = proxyText.GetComponent<RectTransform>().localPosition;
                    
                    //inputField.caretPosition = text.text.Length;
                    Debug.Log(inputField.text);
                    Debug.Log(proxyText.text);
                    Debug.Log(text.text);
                    //Debug.Log(inputField.caretPosition);
                }
            }
        }
    }

    public string HighlightText(string text, int beginningOfSelection, int endOfSelection)
    {
        string mark = "<mark=#ffff1055></mark>";
        string selectedText = "";
        string cleanedText = text.Replace("<mark=#ffff1055>", "").Replace("</mark>", "");
        int startIndexOfSelectedCleanText = 0;

        if (endOfSelection > beginningOfSelection)
        {
            selectedText = cleanedText.Substring(beginningOfSelection, endOfSelection - beginningOfSelection);
            startIndexOfSelectedCleanText = beginningOfSelection;
        }

        else if (endOfSelection < beginningOfSelection)
        {
            selectedText = cleanedText.Substring(endOfSelection, beginningOfSelection - endOfSelection);
            startIndexOfSelectedCleanText = endOfSelection;
        }

        string markedText = mark.Insert(16, selectedText);
        int startIndexOfSelectedText = text.IndexOf(selectedText, startIndexOfSelectedCleanText);
        string highlightedText = text.Remove(startIndexOfSelectedText, selectedText.Length).Insert(startIndexOfSelectedText, markedText);

        return highlightedText;
    }

    //public string HighlightText(string text, int beginningOfSelection, int endOfSelection)
    //{
    //    string highlightedText = "";
    //    string markedText = "";
    //    //string cleanedText = text.Replace("<mark=#ffff1055>", "").Replace("</mark>", "");
    //    string halfCleanedText = text.Replace("<mark=#ffff1055>", "");
    //    string cleanedText = halfCleanedText.Replace("</mark>", "");
    //    int startIndexOfSelectedCleanText = 0;
    //    string selectedHighlightedText = "";
    //    int startIndexOfSelectedText = 0;
    //    int fakeIndex = 0;

    //    if (endOfSelection > beginningOfSelection)
    //    {
    //        selectedText = cleanedText.Substring(beginningOfSelection, endOfSelection - beginningOfSelection);
    //        startIndexOfSelectedCleanText = beginningOfSelection;
    //        int a = beginningOfSelection;
    //        int b = endOfSelection;
    //        string fakeSelectedText = text.Substring(a, b - a);
    //        fakeIndex = a;
    //        Debug.Log(fakeSelectedText);
    //        Debug.Log(fakeIndex);

    //        //if (text.Contains("<mark=#ffff1055>") && text.Contains("</mark>"))
    //        //{
    //        //    selectedHighlightedText = text.Substring(beginningOfSelection, (endOfSelection + mark.Length) - beginningOfSelection);
    //        //}
    //    }

    //    else if (endOfSelection < beginningOfSelection)
    //    {
    //        selectedText = cleanedText.Substring(endOfSelection, beginningOfSelection - endOfSelection);
    //        startIndexOfSelectedCleanText = endOfSelection;

    //        //if (text.Contains("<mark=#ffff1055>") && text.Contains("</mark>"))
    //        //{
    //        //    selectedHighlightedText = text.Substring(endOfSelection, (beginningOfSelection + mark.Length) - endOfSelection);
    //        //}
    //    }

    //    markedText = mark.Insert(16, selectedText);

    //    //Debug.Log(selectedHighlightedText);

    //    //if (selectedHighlightedText.Contains("<mark=#ffff1055>") && selectedHighlightedText.Contains("</mark>"))
    //    //{
    //    //    markedText = selectedHighlightedText.Replace("<mark=#ffff1055>", "").Replace("</mark>", "");
    //    //    selectedText = selectedHighlightedText;
    //    //}

    //    startIndexOfSelectedText = text.IndexOf(selectedText, fakeIndex);
    //    string s = text.Substring(startIndexOfSelectedText, selectedText.Length);


    //    highlightedText = text.Remove(startIndexOfSelectedText, selectedText.Length).Insert(startIndexOfSelectedText, markedText);

    //    return highlightedText;
    //}

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
}



