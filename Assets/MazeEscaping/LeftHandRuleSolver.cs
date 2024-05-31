using UnityEngine;
using System.Collections.Generic;

public class LeftHandRuleSolver : MonoBehaviour
{
    [SerializeField] private MazeGeneratorByBinaryTree mazeGenerator;
    [SerializeField] private GameObject pathParent;
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private Material pathMaterial;
    [SerializeField] private Color markerColor = Color.green;
    [SerializeField] private Color pathColor = Color.yellow;

    private int[,] map;
    private int width;
    private int height;

    private void Start()
    {
        map = mazeGenerator.map;
        width = mazeGenerator.width;
        height = mazeGenerator.height;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            map = mazeGenerator.map;
            FindPathUsingLeftHandRule();
            pathParent.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void FindPathUsingLeftHandRule()
    {
        Vector2Int start = new Vector2Int(1, 0);
        Vector2Int end = new Vector2Int(width - 2, height - 1);
        List<Vector2Int> path = new List<Vector2Int>();

        // 끝점에 도달할 때까지 경로 탐색
        if (LeftHandRule(start, end, path))
        {
            // 끝점에서 시작점까지 백트래킹하며 경로를 노란색으로 표시
            for (int i = path.Count - 2; i >= 0; i--)
            {
                Vector2Int pos = path[i];
                CreateMarker(pos.x, pos.y, pathColor);
            }
        }
    }

    private bool LeftHandRule(Vector2Int start, Vector2Int end, List<Vector2Int> path)
    {
        Vector2Int current = start;
        Vector2Int direction = Vector2Int.up; // 시작 방향은 위쪽

        while (current != end)
        {
            path.Add(current);
            Vector2Int left = GetLeftDirection(direction);
            if (IsValid(current + left) && map[current.x + left.x, current.y + left.y] == 0)
            {
                direction = left;
                current += direction;
            }
            else if (IsValid(current + direction) && map[current.x + direction.x, current.y + direction.y] == 0)
            {
                current += direction;
            }
            else
            {
                direction = GetRightDirection(direction);
            }
        }

        path.Add(end);
        return true;
    }

    private Vector2Int GetLeftDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return Vector2Int.left;
        if (direction == Vector2Int.left) return Vector2Int.down;
        if (direction == Vector2Int.down) return Vector2Int.right;
        return Vector2Int.up;
    }

    private Vector2Int GetRightDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return Vector2Int.right;
        if (direction == Vector2Int.right) return Vector2Int.down;
        if (direction == Vector2Int.down) return Vector2Int.left;
        return Vector2Int.up;
    }

    private bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    private void CreateMarker(int x, int y, Color color)
    {
        Vector3 position = new Vector3(-width / 2 + x, 0, -height / 2 + y);
        GameObject marker = Instantiate(markerPrefab, position, Quaternion.identity);
        marker.GetComponent<Renderer>().material.color = color;
        marker.transform.parent = pathParent.transform;
    }
}
