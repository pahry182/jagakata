using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static Util Instance { get; internal set; }

    private System.Random random = new System.Random();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public char[][] DeepcloneArray(char[][] sourceArr)
    {
        char[][] clonedArr = new char[sourceArr.Length][];
        for (int i = 0; i < clonedArr.Length; i++)
        {
            clonedArr[i] = new char[sourceArr[i].Length];
            for (int j = 0; j < clonedArr[i].Length; j++)
            {
                clonedArr[i][j] = sourceArr[i][j];
            }
        }

        return clonedArr;
    }

    public char GetRandomAlphabet()
    {
        int randomNumber = random.Next(26); // Generates a random number between 0 and 25

        char randomAlphabet = (char)('A' + randomNumber); // Converts the random number to an alphabet character

        return randomAlphabet;
    }

    public static List<XYPoint> DeepCopyList(List<XYPoint> originalList)
    {
        if (originalList == null)
        {
            return null;
        }

        List<XYPoint> newList = new List<XYPoint>();

        foreach (XYPoint item in originalList)
        {
            if (item != null)
            {
                // Create a new XYPoint instance and copy the values from the original item.
                XYPoint newItem = new XYPoint(item.X, item.Y);
                newList.Add(newItem);
            }
        }

        return newList;
    }

}
