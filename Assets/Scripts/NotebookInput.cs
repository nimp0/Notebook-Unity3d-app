using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

enum NotebookMode
{
    Drawing,
    Erasing,
    Typing,
    Moving,
}

static class NotebookInput
{
    public static NotebookMode notebookInputMode = NotebookMode.Drawing;

    public static void ChangeMode()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
        {
            notebookInputMode = NotebookMode.Drawing;
        }

        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftControl))
        {
            notebookInputMode = NotebookMode.Erasing;
        }

        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
        {
            notebookInputMode = NotebookMode.Typing;
        }

        else if (UnityEngine.Input.GetKeyDown(KeyCode.LeftAlt))
        {
            notebookInputMode = NotebookMode.Moving;
        }
    }
}

