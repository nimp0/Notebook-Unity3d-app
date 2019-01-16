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

    private MovementWindow activatedMovementWindow = null;
    private PaintLine drawnPaintLine = null;
    private TextPanel activatedTextPanel;
    private List<TextPanel> textPanels = new List<TextPanel>();
    private List<PaintLine> paintLines = new List<PaintLine>();

    private RaycastHit hit;
    private Ray ray;
    private bool isTextInputing;

    void Update()
    {
        for (int i = 0; i < textPanels.Count; i++)
        {
            textPanels[i].OnAnyChange();
        }

        isTextInputing = false;
        NotebookInput.ChangeMode();
        ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Sizes.Physics.raycastDistance, LayerMasksList.TextPanelMask))
        {
            for (int i = 0; i < textPanels.Count; i++)
            {
                if (textPanels[i].inputField.caretPosition != textPanels[i].inputField.selectionAnchorPosition)
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        TryHighlightText(textPanels[i]);
                    }
                }
            }

            return;
        }

        if (Physics.Raycast(ray, out hit, Sizes.Physics.raycastDistance, LayerMasksList.WallMask))
        {
            if (NotebookInput.notebookInputMode == NotebookMode.Drawing)
            {
                DestroyMovementWindow();

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
                DestroyMovementWindow();

                if (Input.GetMouseButton(0))
                {
                    TryErasePaintLine();
                }
            }

            else if (NotebookInput.notebookInputMode == NotebookMode.Typing)
            {
                DestroyMovementWindow();

                if (Input.GetMouseButtonDown(0))
                {
                    CreateTextPanel(hit.point);
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
                DestroyMovementWindow();
                TryStopDrawing();
            }

            else if (NotebookInput.notebookInputMode == NotebookMode.Moving)
            {
                if (Input.GetMouseButton(0))
                {
                    DestroyMovementWindow();
                }

                //if (mWfunc == WindowStage.Selecting)
                //{
                //    if (Input.GetMouseButtonUp(0))
                //    {
                //        TryStopSelecting(hit.point);
                //    }
                //}

                //else if (mWfunc == WindowStage.Moving)
                //{
                //    if (Input.GetMouseButtonUp(0))
                //    {
                //        TryStopMoving();
                //    }
                //}
            }
        }
    }

    #region Drawing

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

    #endregion

    #region Erasing

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

    #endregion

    #region Typing

    void CreateTextPanel(Vector3 localPoint)
    {
        /*RaycastHit hit;
        Physics.Raycast(Ray, out hit, Sizes.Physics.raycastDistance, 1 << LayersNameTable.InfoWall);*/
        Vector2 wallRelativePosition = hit.transform.InverseTransformPoint(hit.point);

        activatedTextPanel = BuildTextPanel(wallRelativePosition);
        //builtTextPanel.Move(wallRelativePosition);

        //createdTextPanel.Focus();
        activatedTextPanel.inputField.onSelect.AddListener(delegate { OnTextPanelSelect(); });
        activatedTextPanel.inputField.onDeselect.AddListener(delegate { OnTextPanelDeselect(activatedTextPanel); DestroyEmptyTextPanel(activatedTextPanel); });
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

    #endregion

    #region Moving

    void TryStartSelecting(Vector3 localPoint)
    {
        if (selectingStage == SelectionStage.None)
        {
            selectingStage = SelectionStage.InProgress;
            activatedMovementWindow = BuildMovementWindow(localPoint);
        }
    }

    void TrySelecting(Vector3 localPoint)
    {
        TryStartSelecting(localPoint);
        activatedMovementWindow.RecalculateWindowSizeByLocalPoint(localPoint);
    }

    void TryStopSelecting(Vector3 localPoint)
    {
        if (selectingStage == SelectionStage.InProgress)
        {
            activatedMovementWindow.FinalizeSelecting(localPoint, wall.transform);

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
        if (Math.IsLeftUp(localPoint, activatedMovementWindow.cornerPoints[0]) && Math.IsLeftDown(localPoint, activatedMovementWindow.cornerPoints[1]) && Math.IsRightDown(localPoint, activatedMovementWindow.cornerPoints[2]) && Math.IsRightUp(localPoint, activatedMovementWindow.cornerPoints[3]))
        {
            activatedMovementWindow.SetPivotByLocalPoint(localPoint);
        }

        else
        {
            DestroyMovementWindow();
        }

        //movementWindow.ChangeLocalPositionOfTransformChildsRelativelyPivot(wall.transform, localPoint);
    }

    void TryMoving(Vector3 localPoint)
    {
        //TryStartMoving(localPoint);
        if (activatedMovementWindow != null)
        {
            activatedMovementWindow.MoveWindowByPoint(localPoint);
        }
    }

    void TryStopMoving()
    {
        activatedMovementWindow.ChangeTransformParentBySelectedTransforms(wall.transform);
        DestroyMovementWindow();
    }

    void DestroyMovementWindow()
    {
        if (activatedMovementWindow != null)
        {
            Destroy(activatedMovementWindow.go);
            activatedMovementWindow = null;
            mWfunc = WindowStage.Selecting;
            selectingStage = SelectionStage.None;
        }
    }

    #endregion

    #region Builders

    MovementWindow BuildMovementWindow(Vector3 localPoint)
    {
        MovementWindow newMovementWindow = new MovementWindow(wall);
        newMovementWindow.go.transform.position = localPoint;

        return newMovementWindow;
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
        newTextPanel.go.transform.localPosition = new Vector3(localPoint.x, localPoint.y, localPoint.z - wall.transform.localScale.z / 2);
        textPanels.Add(newTextPanel);

        return newTextPanel;
    }

    #endregion








}