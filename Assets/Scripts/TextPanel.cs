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

        rtBorders = go.transform.GetChild(0).GetComponent<RectTransform>().GetChild(1).GetComponent<RectTransform>();
        //textPanel.transform.LocalReset();

        go.transform.localPosition = Vector3.forward * Sizes.BuildingParameters.blockWidth / 2;
        go.transform.rotation = Quaternion.LookRotation(parent.transform.forward);
        go.transform.localScale = parent.transform.localScale / 1000;

        proxyText = go.GetComponentInChildren<TMP_Text>();
        inputField = go.GetComponentInChildren<TMP_InputField>();

        inputField.onFocusSelectAll = false;
        boxCollider = proxyText.gameObject.AddComponent<BoxCollider>();
        //textPanel.layer = LayersNameTable.TextPanel;

        Focus();
        inputField.onValueChanged.AddListener(delegate { OnAnyChange(); });
        OnAnyChange();

        cornerPoints = new Vector3[4];
    }

    public IEnumerator UpdateCollider()
    {
        yield return null;

        RecalculateColliderSize();
    }

    public void Focus()
    {
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void OnAnyChange()
    {
        UpdateProxyText();
        RecalculateColliderSize();
        //SelectText();
    }

    void RecalculateColliderSize()
    {
        boxCollider.size = new Vector3(proxyText.rectTransform.sizeDelta.x, proxyText.rectTransform.sizeDelta.y, Sizes.Notebook.textPanelColliderDepth);
        boxCollider.center = proxyText.bounds.center;
    }

    public void Move(Vector2 localPosition)
    {
        go.transform.localPosition = (Vector3)localPosition + Vector3.forward * (Sizes.BuildingParameters.blockWidth / 2 + Sizes.Notebook.offsetFromWall);
    }

    string HighlightText(string text, int beginningOfSelection, int endOfSelection)
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

    void UpdateProxyText()
    {
        proxyText.text = inputField.text;

        if (inputField.isFocused)
        {
            //RectTransform borders = go.transform.GetChild(0).GetComponent<RectTransform>().GetChild(1).GetComponent<RectTransform>();
            //borders.gameObject.SetActive(true);
            //proxyText.text = inputField.text;
            //if(inputField.isFocused)
            //{
            //	RectTransform borders = panelGO.transform.GetChild(0).GetComponent<RectTransform>().GetChild(1).GetComponent<RectTransform>();
            //	borders.gameObject.SetActive(true);
            //	//proxyText.text = inputField.text;

            //if(inputField.text.Length == 0)
            //{
            //	borders.gameObject.SetActive(false);
            //}
            //	/*if(inputField.text.Length == 0)
            //	{
            //		borders.gameObject.SetActive(false);
            //	}*/

            //	if(inputField.text.Length > 0)
            //	{
            //		proxyText.text = inputField.text.Replace(' ', ',');
            //	}

            //	//var a = Environment.NewLine;
            //	//Debug.Log(a);
            //	//if((inputField.text.Length > 0 && inputField.text.Substring(inputField.text.Length - a.Length) == a))
            //	//{
            //	//	proxyText.text += a;
            //	//	//proxyText.text = inputField.text.Replace('\n', '-');
            //	//	//Debug.Log(proxyText.text.Last());
            //	//}
            //}
        }
        //newText = HighlightText(inputField.text, inputField.selectionAnchorPosition, inputField.caretPosition);
        //inputField.text = newText;
    }

    public void SetCaretPosition(TMP_InputField inputField, int caretPos)
    {
        string tempText = inputField.text;
        inputField.text = inputField.text.Substring(0, caretPos);
        inputField.MoveTextEnd(false);
        inputField.text = tempText;
    }

    public void RecalculateCornerPoints(GameObject parent)
    {
        Rect rect = rtBorders.rect;
        rect.size = proxyText.rectTransform.sizeDelta;

        Vector3 a = new Vector3(rect.xMin, rect.yMax) / 1000;
        Vector3 b = new Vector3(rect.xMin, rect.yMin) / 1000;
        Vector3 c = new Vector3(rect.xMax, rect.yMin) / 1000;
        Vector3 d = new Vector3(rect.xMax, rect.yMax) / 1000;

        Vector3 leftUpCorner = new Vector3(a.x, a.y, parent.transform.localPosition.z) + rtBorders.position;
        Vector3 leftDownCorner = new Vector3(b.x, b.y, parent.transform.localPosition.z) + rtBorders.position;
        Vector3 rightDownCorner = new Vector3(c.x, c.y, parent.transform.localPosition.z) + rtBorders.position;
        Vector3 rightUpCorner = new Vector3(d.x, d.y, parent.transform.localPosition.z) + rtBorders.position;

        Vector3 projectedLUCorner = new Vector3(ProjectPointOnTransformAxes(leftUpCorner, parent.transform).x, ProjectPointOnTransformAxes(leftUpCorner, parent.transform).y, leftUpCorner.z);
        Vector3 projectedLDCorner = new Vector3(ProjectPointOnTransformAxes(leftDownCorner, parent.transform).x, ProjectPointOnTransformAxes(leftDownCorner, parent.transform).y, leftDownCorner.z);
        Vector3 projectedRDCorner = new Vector3(ProjectPointOnTransformAxes(rightDownCorner, parent.transform).x, ProjectPointOnTransformAxes(rightDownCorner, parent.transform).y, rightDownCorner.z);
        Vector3 projectedRUCorner = new Vector3(ProjectPointOnTransformAxes(rightUpCorner, parent.transform).x, ProjectPointOnTransformAxes(rightUpCorner, parent.transform).y, rightUpCorner.z);

        /*Debug.DrawLine(projectedLUCorner, projectedLDCorner, Color.red, 20);
        Debug.DrawLine(projectedLDCorner, projectedRDCorner, Color.red, 20);
        Debug.DrawLine(projectedRDCorner, projectedRUCorner, Color.red, 20);
        Debug.DrawLine(projectedRUCorner, projectedLUCorner, Color.red, 20);*/

        cornerPoints[0] = projectedLUCorner;
        cornerPoints[1] = projectedLDCorner;
        cornerPoints[2] = projectedRDCorner;
        cornerPoints[3] = projectedRUCorner;
    }

    public Vector2 ProjectPointOnTransformAxes(Vector3 localPoint, Transform transform)
    {
        Vector3 rightAxisVector = Vector3.Project(localPoint, transform.right) + new Vector3(0, 0, localPoint.z / 2);
        Vector3 upAxisVector = Vector3.Project(localPoint, transform.up) + new Vector3(0, 0, localPoint.z / 2);
        Vector2 projectedPoint = new Vector2(rightAxisVector.x, upAxisVector.y);

        return projectedPoint;
    }
}



//public static void BuildContext(ContextInputWindow ciw)
//{
//	RectTransform rt = ciw.proxyText.GetComponent<RectTransform>();
//	rt.pivot = new Vector2(.5f, 1f);
//	ciw.go.transform.localPosition = -Vector3.up * 50;
//	TMP_InputField inputField = ciw.inputField;
//	inputField.textComponent.color = Color.black;
//	TextAlignmentOptions aligment = TextAlignmentOptions.TopLeft;
//	TextOverflowModes overflowMode = TextOverflowModes.Overflow;

//	inputField.textComponent.alignment = aligment;
//	//inputField.textComponent.enableAutoSizing = true;
//	inputField.textComponent.fontSizeMax = 14;
//	inputField.textComponent.enableWordWrapping = true;
//	inputField.textComponent.overflowMode = overflowMode;
//	inputField.textComponent.fontSize = Sizes.InpuwWindowData.nameFotnSize;
//	inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
//	inputField.GetComponent<UnityEngine.UI.Image>().color = Color.white;

//	TMP_Text fakeText = ciw.proxyText;
//	//fakeText.enableAutoSizing = true;
//	fakeText.fontSizeMax = 14;
//	fakeText.alignment = aligment;
//	fakeText.overflowMode = overflowMode;
//	fakeText.fontSize = Sizes.InpuwWindowData.nameFotnSize;
//	fakeText.GetComponent<UnityEngine.UI.ContentSizeFitter>().horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
//	fakeText.GetComponent<UnityEngine.UI.ContentSizeFitter>().verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
//}