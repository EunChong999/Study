using UnityEngine;
using System.Collections.Generic;

public class BresenhamAlgorithm : MonoBehaviour
{
    [SerializeField]
    private NodeManager nodeManager;
    
    public int width = 10;  // Array width
    public int height = 10; // Array height
    private int[,] grid;    // 2D array

    private List<Vector2Int> linePoints; // List to store line points

    void Start()
    {
        // Initialize the 2D array
        grid = new int[width, height];

        // Initialize the list to store line points
        linePoints = new List<Vector2Int>();

        // Example: Draw a line from (1, 1) to (8, 6)
        DrawLine(1, 1, 28, 15);

        // Print the grid
        PrintGrid();


    }

    private void Update()
    {
        // Print the line points
        PrintLinePoints();
    }

    void DrawLine(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Mark the current position in the 2D array
            grid[x0, y0] = 1;

            // Add the current position to the list of line points
            linePoints.Add(new Vector2Int(x0, y0));

            // Break the loop if the end point is reached
            if (x0 == x1 && y0 == y1) break;
            int e2 = err * 2;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    void PrintGrid()
    {
        string gridString = "";
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                gridString += grid[x, y] == 1 ? "X" : ".";
            }
            gridString += "\n";
        }
        Debug.Log(gridString);
    }

    void PrintLinePoints()
    {
        string pointsString = "Line points: ";
        foreach (Vector2Int point in linePoints)
        {
            pointsString += $"({point.x}, {point.y}) ";
            nodeManager.nodes[point.x, point.y].isPath = true;
        }
        Debug.Log(pointsString);
    }
}
