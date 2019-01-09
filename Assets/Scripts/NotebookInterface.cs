using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum NotebookIM
{
    Drawing,
    Erasing,
    Typing,
    Moving,
}

enum WindowStage
{
    Selecting,
    Moving
}

enum LineBuildingStages
{
    None,
    InProgress
}

enum SelectionStates
{
    None,
    InProgress
}

public class NotebookInterface : MonoBehaviour
{
    NotebookIM notebookInputMode = NotebookIM.Drawing;
    LineBuildingStages lineBuildingStage = LineBuildingStages.None;
    WindowStage mWfunc = WindowStage.Selecting;
    SelectionStates selectingStage = SelectionStates.None;

    public Canvas canvas;
    public GameObject wall;
    public Camera camera;

    private MovementWindow movementWindow = null;
    private PaintLine drawnPaintLine = null;
    private TextPanel textPanel;
    private List<TextPanel> textPanels = new List<TextPanel>();
    private List<PaintLine> paintLines = new List<PaintLine>();

    private RaycastHit hit;
    private Ray ray;
    private bool isTextInputing;

    void Update()
    {
        for (int i = 0; i < textPanels.Count; i++)
        {
            //panels[i].OnAnyChange();
            textPanels[i].RecalculateCornerPoints(wall);
        }

        ray = camera.ScreenPointToRay(Input.mousePosition);

        if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
        {
            notebookInputMode = NotebookIM.Drawing;
        }

        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftControl))
        {
            notebookInputMode = NotebookIM.Erasing;
        }

        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
        {
            notebookInputMode = NotebookIM.Typing;
        }

        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftAlt))
        {
            notebookInputMode = NotebookIM.Moving;
        }

        if (Physics.Raycast(ray, out hit, Sizes.Physics.raycastDistance, Physics.AllLayers))
        {
             if (notebookInputMode == NotebookIM.Drawing)
             {
                 if (Input.GetMouseButton(0))
                 {
                     TryDrawLine();
                 }
                 else if (Input.GetMouseButtonUp(0))
                 {
                     TryStopDrawing();
                 }
             }
             else if (notebookInputMode == NotebookIM.Erasing)
             {
                 if (Input.GetMouseButton(0))
                 {
                    TryErasePaintLine();
                 }
             }

            if (notebookInputMode == NotebookIM.Typing)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    CreateTextPanel(hit.point);
                }
            }

            else if (notebookInputMode == NotebookIM.Moving)
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
            if (notebookInputMode == NotebookIM.Drawing)
            {
                TryStopDrawing();
            }


            else if (notebookInputMode == NotebookIM.Moving)
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
        if (selectingStage == SelectionStates.None)
        {
            selectingStage = SelectionStates.InProgress;
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
        if (selectingStage == SelectionStates.InProgress)
        {
            movementWindow.FinalizeSelecting(localPoint, wall.transform);

            for (int i = 0; i < paintLines.Count; i++)
            {
                //movementWindow.DetectForTransformsInMW(lines[i].go.transform, lines[i].recalculatedPointsVectors);
            }

            selectingStage = SelectionStates.None;
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
        if (lineBuildingStage == LineBuildingStages.None)
        {
            lineBuildingStage = LineBuildingStages.InProgress;
            drawnPaintLine = BuildPaintLine();
        }
    }

    void TryStopDrawing()
    {
        if (lineBuildingStage == LineBuildingStages.InProgress)
        {
            drawnPaintLine.Finilize();
            //paintLine.RecalculatePointsVectors(wall.transform);
            lineBuildingStage = LineBuildingStages.None;
            drawnPaintLine = null;
        }
    }

    void TryDrawLine()
    {
        TryStartDrawing();
        //RaycastHit hit;
        //Physics.Raycast(Ray, out hit, Sizes.Physics.raycastDistance, Physics.AllLayers);
        Vector3 wallRelativePosition = wall.transform.InverseTransformPoint(hit.point);
        drawnPaintLine.AddPoint(wallRelativePosition);
    }

    void TryErasePaintLine()
    {
        Vector2 wallRelativePosition = hit.transform.InverseTransformPoint(hit.point);

        for (int i = 0; i < paintLines.Count; i++)
        {
            PaintLine paintLine = paintLines[i];

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

        TextPanel builtTextPanel = BuildTextPanel(wallRelativePosition);
        builtTextPanel.Move(wallRelativePosition);
        /*TextEntity textEntity = Network.CreateText(VandalWall.Id, "Default text");
        textEntity.X = wallRelativePosition.x;
        textEntity.Y = wallRelativePosition.y;
        Network.UpdateText(textEntity.Id, textEntity);
        builtTextPanel.TextEntity = textEntity;*/

        builtTextPanel.Focus();
        builtTextPanel.inputField.onSelect.AddListener(delegate { OnTextPanelSelect(); });
        builtTextPanel.inputField.onDeselect.AddListener(delegate { OnTextPanelDeselect(builtTextPanel); });
        builtTextPanel.inputField.onValueChanged.AddListener(delegate { DestroyEmptyTextPanel(builtTextPanel); });
    }

    

    void DestroyEmptyTextPanel(TextPanel textPanel)
    {
        if (textPanel.inputField.text.Length == 0)
        {
            Destroy(textPanel.go);
        }
    }

    void OnTextPanelSelect()
    {
        isTextInputing = true;
    }

    void OnTextPanelDeselect(TextPanel textPanel)
    {
        isTextInputing = false;
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
        newTextPanel.go.transform.position = localPoint;
        textPanels.Add(newTextPanel);

        return newTextPanel;
    }
}