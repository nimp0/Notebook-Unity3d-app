using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Linq;

public class TextHighlighting : MonoBehaviour
{
    public TMP_InputField inputField;
    
    private string mark = "<mark=#ffff09aa></mark>";
    private string selectedText = "";
    private string newText;

    void Update()
    {
        if (inputField.caretPosition != inputField.selectionAnchorPosition)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                newText = MarkText(inputField.text, inputField.selectionAnchorPosition, inputField.caretPosition);
                inputField.text = newText;

                //string textAfterMarkedText = "";

                //if (text != null)
                //{
                //    textAfterMarkedText = inputField.text.Substring(inputField.caretPosition, inputField.text.Length - inputField.caretPosition);
                //    Debug.Log(inputField.caretPosition);
                //    Debug.Log(inputField.selectionAnchorPosition);
                //    Debug.Log(textAfterMarkedText);
                //    newText = text.Substring(0, inputField.selectionAnchorPosition) + markedText + textAfterMarkedText;
                //}

                //else
                //{
                //    textAfterMarkedText = cleanedText.Substring(inputField.caretPosition, cleanedText.Length - inputField.caretPosition);
                //    newText = cleanedText.Substring(0, inputField.selectionAnchorPosition) + markedText + textAfterMarkedText;
                //}

                //inputField.text = newText;
                //text = inputField.text;

                //////newText = inputField.text.Replace(selectedText, markedText);
                ////inputField.text = newText;
            }
        }
    }

    string MarkText(string text, int beginningOfSelection, int endOfSelection)
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
}
