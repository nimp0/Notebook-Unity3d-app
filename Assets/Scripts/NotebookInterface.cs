using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum WindowStage
{
    Selecting,
    Moving
}

enum LineBuildingStage
{
    None,
    InProgress
}

enum SelectionStage
{
    None,
    InProgress
}

public class NotebookInterface : MonoBehaviour
{
    public Canvas canvas;
    public GameObject wall;
    public Camera camera;

    LineBuildingStage lineBuildingStage = LineBuildingStage.None;
    WindowStage mWfunc = WindowStage.Selecting;
    SelectionStage selectingStage = SelectionStage.None;

    private MovementWindow movementWindow = null;
    private PaintLine drawnPaintLine = null;
    private TextPanel createdTextPanel;
    private List<TextPanel> textPanels = new List<TextPanel>();
    private List<PaintLine> paintLines = new List<PaintLine>();

    private RaycastHit hit;
    private Ray ray;
    private bool isTextInputing;
    private bool canBeHighlighted;

    void Update()
    {
        if (createdTextPanel != null)
        {
            Debug.Log(createdTextPanel.inputField.caretPosition);
            Debug.Log(createdTextPanel.inputField.selectionAnchorPosition);
        }

        isTextInputing = false;
        NotebookInput.ChangeMode();
        ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Sizes.Physics.raycastDistance, LayerMasksList.WallMask))
        {
             if (NotebookInput.notebookInputMode == NotebookMode.Drawing)
             {
                 if (Input.GetMouseButton(0))
                 {
                     TryDrawing();
                 }
                 else if (Input.GetMouseButtonUp(0))
                 {
                     TryStopDrawing();
                 }
             }
             else if (NotebookInput.notebookInputMode == NotebookMode.Erasing)
             {
                 if (Input.GetMouseButton(0))
                 {
                    TryErasePaintLine();
                 }
             }

            else if (NotebookInput.notebookInputMode == NotebookMode.Typing)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    CreateTextPanel(hit.point);
                }

                if (createdTextPanel != null)
                {
                    if (createdTextPanel.inputField.caretPosition != createdTextPanel.inputField.selectionAnchorPosition)
                    {
                        canBeHighlighted = true;
                        //if (Input.GetKeyDown(KeyCode.Mouse1))
                        {
                            TryHighlightText(createdTextPanel);
                        }
                    }
                }
            }

            else if (NotebookInput.notebookInputMode == NotebookMode.Moving)
            {
                if (mWfunc == WindowStage.Selecting)
                {
                    if (Input.GetMouseButton(0))
                    {
                        TrySelecting(hit.point);
                    }

                    else if (Input.GetMouseButtonUp(0))
                    {
                        TryStopSelecting(hit.point);
                    }
                }

                else if (mWfunc == WindowStage.Moving)
                {
                    Vector3 localPoint = hit.transform.InverseTransformPoint(hit.point);

                    if (Input.GetMouseButtonDown(0))
                    {
                        TryStartMoving(hit.point);
                    }

                    if (Input.GetMouseButton(0))
                    {
                        TryMoving(localPoint);
                    }

                    else if (Input.GetMouseButtonUp(0))
                    {
                        TryStopMoving();
                    }
                }
            }
        }

        else
        {
            if (NotebookInput.notebookInputMode == NotebookMode.Drawing)
            {
                TryStopDrawing();
            }


            else if (NotebookInput.notebookInputMode == NotebookMode.Moving)
            {
                if (mWfunc == WindowStage.Selecting)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        TryStopSelecting(hit.point);
                    }
                }

                else if (mWfunc == WindowStage.Moving)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        TryStopMoving();
                    }
                }
            }
        }
    }

    void TryStartSelecting(Vector3 localPoint)
    {
        if (selectingStage == SelectionStage.None)
        {
            selectingStage = SelectionStage.InProgress;
            movementWindow = BuildMovementWindow(localPoint);
        }
    }

    void TrySelecting(Vector3 localPoint)
    {
        TryStartSelecting(localPoint);
        movementWindow.RecalculateWindowSizeByLocalPoint(localPoint);
    }

    void TryStopSelecting(Vector3 localPoint)
    {
        if (selectingStage == SelectionStage.InProgress)
        {
            movementWindow.FinalizeSelecting(localPoint, wall.transform);

            for (int i = 0; i < paintLines.Count; i++)
            {
                //movementWindow.DetectForTransformsInMW(lines[i].go.transform, lines[i].recalculatedPointsVectors);
            }

            selectingStage = SelectionStage.None;
            mWfunc = WindowStage.Moving;
        }
    }

    void TryStartMoving(Vector3 localPoint)
    {
        movementWindow.SetPivotByLocalPoint(localPoint);
        //movementWindow.ChangeLocalPositionOfTransformChildsRelativelyPivot(wall.transform, localPoint);
    }

    void TryMoving(Vector3 localPoint)
    {
        //TryStartMoving(localPoint);
        movementWindow.MoveWindowByPoint(localPoint);
    }

    void TryStopMoving()
    {
        movementWindow.ChangeTransformParentBySelectedTransforms(wall.transform);
        Destroy(movementWindow.go);
        movementWindow = null;
        mWfunc = WindowStage.Selecting;
    }

    void TryStartDrawing()
    {
        if (lineBuildingStage == LineBuildingStage.None)
        {
            lineBuildingStage = LineBuildingStage.InProgress;
            drawnPaintLine = BuildPaintLine();
        }
    }

    void TryStopDrawing()
    {
        if (lineBuildingStage == LineBuildingStage.InProgress)
        {
            drawnPaintLine.Finilize();
            //paintLine.RecalculatePointsVectors(wall.transform);
            lineBuildingStage = LineBuildingStage.None;
            drawnPaintLine = null;
        }
    }

    void TryDrawing()
    {
        TryStartDrawing();
        //RaycastHit hit;
        //Physics.Raycast(Ray, out hit, Sizes.Physics.raycastDistance, Physics.AllLayers);
        Vector3 wallRelativePosition = wall.transform.InverseTransformPoint(hit.point);
        drawnPaintLine.AddPoint(wallRelativePosition);
    }

    void TryErasePaintLine()
    {
        Vector3 wallRelativePosition = wall.transform.InverseTransformPoint(hit.point);

        for (int i = 0; i < paintLines.Count; i++)
        {
            PaintLine paintLine = paintLines[i];

            //for (int j = 0; j < paintLine.lineRenderer.positionCount; j++)
            //{
            //    Debug.DrawLine(wallRelativePosition + paintLine.go.transform.position, paintLine.lineRenderer.GetPosition(j) + paintLine.go.transform.position, Color.red, 10);
            //    Debug.DrawLine(paintLine.lineRenderer.GetPosition(j) + paintLine.go.transform.position, paintLine.lineRenderer.GetPosition(j) + Vector3.up * paintLine.pointsRadii[j] + paintLine.go.transform.position, Color.blue, 10);
            //}

            if (paintLine.OverlapPoint(wallRelativePosition))
            {
                Destroy(paintLine.go);
                paintLines.Remove(paintLine);
                break;
            }
        }
    }

    void CreateTextPanel(Vector3 localPoint)
    {
        /*RaycastHit hit;
        Physics.Raycast(Ray, out hit, Sizes.Physics.raycastDistance, 1 << LayersNameTable.InfoWall);*/
        Vector2 wallRelativePosition = hit.transform.InverseTransformPoint(hit.point);

        createdTextPanel = BuildTextPanel(wallRelativePosition);
        //builtTextPanel.Move(wallRelativePosition);

        //createdTextPanel.Focus();
        createdTextPanel.inputField.onSelect.AddListener(delegate { OnTextPanelSelect(); });
        createdTextPanel.inputField.onDeselect.AddListener(delegate { OnTextPanelDeselect(createdTextPanel); DestroyEmptyTextPanel(createdTextPanel); });
        //activatedTextPanel.inputField.onValueChanged.AddListener(delegate { DestroyEmptyTextPanel(activatedTextPanel); });
    }

    void TryHighlightText(TextPanel textPanel)
    {
        string newText = textPanel.HighlightText(textPanel.inputField.text, textPanel.inputField.selectionAnchorPosition, textPanel.inputField.caretPosition);
        textPanel.inputField.text = newText;
    }

    void DestroyEmptyTextPanel(TextPanel textPanel)
    {
        if (textPanel.inputField.text.Length == 0)
        {
            Destroy(textPanel.go);
            textPanels.Remove(textPanel);
        }
    }

    void OnTextPanelSelect()
    {
        isTextInputing = true;
    }

    void OnTextPanelDeselect(TextPanel textPanel)
    {
        if (!textPanel.inputField.isFocused)
        {
            isTextInputing = false;
        }
    }

    MovementWindow BuildMovementWindow(Vector3 localPoint)
    {
        MovementWindow movementWindow = new MovementWindow(wall);
        movementWindow.go.transform.position = localPoint;
        
        return movementWindow;
    }

    PaintLine BuildPaintLine(/*Color color, float width*/)
    {
        PaintLine newPaintLine = new PaintLine();
        /*newPaintLine.color = color;
        newPaintLine.width = width;*/
        newPaintLine.Create(wall);
        paintLines.Add(newPaintLine);

        return newPaintLine;
    }

    TextPanel BuildTextPanel(Vector3 localPoint)
    {
        TextPanel newTextPanel = new TextPanel();
        //infoWall.AddTextPanel(newTextPanel);
        newTextPanel.Create(wall);
        newTextPanel.go.transform.localPosition = new Vector3(localPoint.x, localPoint.y, localPoint.z - wall.transform.localScale.z/2);
        textPanels.Add(newTextPanel);

        return newTextPanel;
    }
}