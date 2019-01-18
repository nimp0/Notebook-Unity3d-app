using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine.UI;

class TextPanel
{
    public GameObject go;
    public TMP_InputField inputField;

    private TMP_FontAsset fontAsset;
    private BoxCollider boxCollider;
    private float targetFontSize;
    public Vector3[] cornerPoints;
    private RectTransform rtBorders;
    private TMP_Text proxyText;

    public void Create(GameObject parent)
    {
        go = UnityEngine.Object.Instantiate(Resources.Load("TextPanel")) as GameObject;
        go.transform.SetParent(parent.transform);
        go.transform.rotation = Quaternion.LookRotation(parent.transform.forward);
        go.transform.localScale = parent.transform.localScale / 3000;

        proxyText = go.GetComponentInChildren<TMP_Text>();
        inputField = go.GetComponentInChildren<TMP_InputField>();
        rtBorders = go.transform.GetChild(0).GetComponent<RectTransform>().GetChild(1).GetComponent<RectTransform>();
        fontAsset = UnityEngine.Object.Instantiate(Resources.Load("LatinCyrilic UI SDF")) as TMP_FontAsset;
        inputField.fontAsset = proxyText.font = fontAsset;

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
        inputField.onValueChanged.AddListener(delegate { OnAnyChange(parent.transform); });

        cornerPoints = new Vector3[4];
    }

    public void Focus()
    {
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void OnAnyChange(Transform parent)
    {
        RecalculateCornerPoints(parent);
        RecalculateColliderSize();
        UpdateProxyText();
        //Debug.Log(inputField.textComponent.renderedWidth);
        //Debug.Log(inputField.textComponent.renderedHeight);
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

    public void RegulatePanelSizesRelativeBoundaryPointsOfParentTransform(Transform parent)
    {
        Vector3 rightSidePoint = new Vector3(parent.localScale.x / 2, go.transform.localPosition.y, go.transform.localPosition.z) + parent.position;
        Vector3 downSidePoint = new Vector3(go.transform.localPosition.x, -parent.localScale.y / 2, go.transform.localPosition.z) + parent.position;
        float maxWidth = Vector3.Distance(parent.InverseTransformPoint(cornerPoints[0]) + parent.transform.position, rightSidePoint);
        float maxHeight = Vector3.Distance(parent.InverseTransformPoint(cornerPoints[0]) + parent.transform.position, downSidePoint);
        float currentLineWidth = proxyText.renderedWidth * go.transform.localScale.x;
        float currentLineCountHeight = proxyText.renderedHeight * go.transform.localScale.y;
        //float currentLineWidth = Vector3.Distance(parent.InverseTransformPoint(cornerPoints[0]) + parent.transform.position, parent.InverseTransformPoint(cornerPoints[3]) + parent.transform.position);
        //float currentLineCountHeight = Vector3.Distance(parent.InverseTransformPoint(cornerPoints[0]) + parent.transform.position, parent.InverseTransformPoint(cornerPoints[1]) + parent.transform.position);

        Debug.DrawLine(parent.InverseTransformPoint(cornerPoints[0] + parent.transform.position), rightSidePoint, Color.green);
        Debug.DrawLine(parent.InverseTransformPoint(cornerPoints[0] + parent.transform.position), parent.InverseTransformPoint(cornerPoints[3] + parent.transform.position), Color.red);

        Debug.DrawLine(parent.InverseTransformPoint(cornerPoints[0] + parent.transform.position), downSidePoint, Color.green);
        Debug.DrawLine(parent.InverseTransformPoint(cornerPoints[0] + parent.transform.position), parent.InverseTransformPoint(cornerPoints[1] + parent.transform.position), Color.red);

        ContentSizeFitter contentSizeFitter = proxyText.GetComponent<ContentSizeFitter>();

        if (currentLineWidth >= maxWidth)
        {
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        else
        {
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        if (currentLineCountHeight >= maxHeight)
        {
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        else
        {
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        if (contentSizeFitter.horizontalFit == ContentSizeFitter.FitMode.Unconstrained && contentSizeFitter.verticalFit == ContentSizeFitter.FitMode.Unconstrained)
        {
            inputField.characterLimit = proxyText.text.Count();
        }

        Debug.Log(proxyText.renderedWidth * go.transform.localScale.x);
        Debug.Log(currentLineWidth);
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
                    text.GetComponent<RectTransform>().localPosition = proxyText.GetComponent<RectTransform>().localPosition;
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
    
    public void RecalculateCornerPoints(Transform parent)
    {
        Rect rect = rtBorders.rect;

        Vector3 leftUpCorner = new Vector3(rect.xMin * go.transform.localScale.x, rect.yMax * go.transform.localScale.y, parent.localPosition.z) + rtBorders.position;
        Vector3 leftDownCorner = new Vector3(rect.xMin * go.transform.localScale.x, rect.yMin * go.transform.localScale.y, parent.localPosition.z) + rtBorders.position;
        Vector3 rightDownCorner = new Vector3(rect.xMax * go.transform.localScale.x, rect.yMin * go.transform.localScale.y, parent.localPosition.z) + rtBorders.position;
        Vector3 rightUpCorner = new Vector3(rect.xMax * go.transform.localScale.x, rect.yMax * go.transform.localScale.y, parent.localPosition.z) + rtBorders.position;

        Vector3 projectedLUCorner = new Vector3(Math.ProjectPointOnTransformAxes(leftUpCorner, parent).x, Math.ProjectPointOnTransformAxes(leftUpCorner, parent).y, leftUpCorner.z);
        Vector3 projectedLDCorner = new Vector3(Math.ProjectPointOnTransformAxes(leftDownCorner, parent).x, Math.ProjectPointOnTransformAxes(leftDownCorner, parent).y, leftDownCorner.z);
        Vector3 projectedRDCorner = new Vector3(Math.ProjectPointOnTransformAxes(rightDownCorner, parent).x, Math.ProjectPointOnTransformAxes(rightDownCorner, parent).y, rightDownCorner.z);
        Vector3 projectedRUCorner = new Vector3(Math.ProjectPointOnTransformAxes(rightUpCorner, parent).x, Math.ProjectPointOnTransformAxes(rightUpCorner, parent).y, rightUpCorner.z);

        //Debug.DrawLine(projectedLUCorner, projectedLDCorner, Color.red, 20);
        //Debug.DrawLine(projectedLDCorner, projectedRDCorner, Color.red, 20);
        //Debug.DrawLine(projectedRDCorner, projectedRUCorner, Color.red, 20);
        //Debug.DrawLine(projectedRUCorner, projectedLUCorner, Color.red, 20);

        cornerPoints[0] = projectedLUCorner;
        cornerPoints[1] = projectedLDCorner;
        cornerPoints[2] = projectedRDCorner;
        cornerPoints[3] = projectedRUCorner;
    }
}

//class Highlighting: TMP_Text
//{
//    private TMP_Text highlightedText;

//    public TMP_Text HighlightedText
//    {
//        get
//        {
//            return highlightedText;
//        }

//        set
//        {
//            highlightedText = value;
//        }
//    }

//    Highlighting(Vector3 start, Vector3 end, ref int index, Color32 highlightColor): base()
//    {
//        this.DrawTextHighlight(start, end, ref index, highlightColor);
//    }

//    protected override void DrawTextHighlight(Vector3 start, Vector3 end, ref int index, Color32 highlightColor)
//    {
        
//    }
//}




