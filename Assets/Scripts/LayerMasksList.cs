using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class LayerMasksList
{
    public static LayerMask TextPanelMask
    {
        get
        {
            return 1 << 9;
        }
    }

    public static int WallMask
    {
        get
        {
            return 1 << 10;
        }
    }
}

