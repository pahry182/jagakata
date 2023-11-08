using UnityEngine;

public class ArrangeChildren : MonoBehaviour
{
    public int numColumns = 3;
    public float horizontalSpacing = 1.0f;
    public float verticalSpacing = 1.0f;

    private void Start()
    {
        // Arrange children initially
        ArrangeChildrenInGrid();
    }

    private void OnTransformChildrenChanged()
    {
        // Re-arrange children when a new child is added
        ArrangeChildrenInGrid();
    }

    private void ArrangeChildrenInGrid()
    {
        int childCount = transform.childCount;

        // Calculate the number of rows based on the child count and number of columns
        int numRows = (childCount + numColumns - 1) / numColumns;

        // Calculate the starting position to maintain middle center formation
        float startX = -horizontalSpacing * (numColumns - 1) * 0.5f;
        float startY = verticalSpacing * (numRows - 1) * 0.5f;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            int row = i / numColumns;
            int col = i % numColumns;

            // Calculate the position based on row and column, accounting for center offset
            float xPos = startX + col * horizontalSpacing;
            float yPos = startY - row * verticalSpacing;
            Vector3 childPosition = new Vector3(xPos, yPos, 0);

            // Set the new position
            child.localPosition = childPosition;
        }
    }
}
