using UnityEngine;
using System;
using System.Collections.Generic;

public class XYPoint
{
    public int X { get; set; }
    public int Y { get; set; }

    public XYPoint(int newX, int newY)
    {
        X = newX;
        Y = newY;
    }

    public bool IsEqual(XYPoint p)
    {
        if (p != null)
        {
            return ((X == p.X) && (Y == p.Y));
        }
        else
        {
            return false;
        }
    }
}
