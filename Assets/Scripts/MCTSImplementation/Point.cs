using UnityEngine;
using System;
using System.Collections.Generic;

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int newX, int newY)
    {
        X = newX;
        Y = newY;
    }

    public bool IsEqual(Point p)
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
