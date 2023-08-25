using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildrenArranger : MonoBehaviour
{
    public int rows = 4; // Number of rows in the grid
    public int columns = 4; // Number of columns in the grid
    public float spacing = 1.0f; // Spacing between objects

    void Start()
    {

    }

    public void ArrangeChilds()
    {

        // Arrange the objects uniformly in a grid
        ArrangeChildren();
    }

    void ArrangeChildren()
    {
        int childCount = transform.childCount;

        // Calculate the total width and height of the grid
        float totalWidth = (columns - 1) * spacing;
        float totalHeight = (rows - 1) * spacing;

        // Calculate the starting position
        Vector3 startPos = transform.position - new Vector3(totalWidth / 2, totalHeight / 2, 0);

        // Arrange the immediate children objects in a grid pattern
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // Calculate the row and column index of the current object
            int row = i / columns;
            int col = i % columns;

            // Calculate the position of the child object in the grid
            Vector3 position = startPos + new Vector3(col * spacing, row * spacing, 0);

            // Set the position of the child object
            child.position = position;
        }
    }

}

