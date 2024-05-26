using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpPointSearch : MonoBehaviour
{
    public bool visualizeRecursion;

    private Node endNode;
    private float straightStepCost = 1.0f;
    private float diagonalStepCost = Mathf.Sqrt(2.0f);

    private PriorityQueue<Node> openList = new PriorityQueue<Node>();
    private HashSet<Node> closeList = new HashSet<Node>();
    private float waitTime;
    private WaitForSeconds waitForSeconds;
    private Coroutine coroutine;

    public Node curNode;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath(NodeManager.instance.startNode);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void FindPath(Node start)
    {
        curNode = start;
        start.g_cost = 0;
        start.h_cost = CalculateHeuristic(start, NodeManager.instance.endNode);
        start.f_cost = start.g_cost + start.h_cost;
        
        SearchPath();
    }

    private float CalculateHeuristic(Node node, Node endNode)
    {
        Vector3 nodePos = node.transform.position;
        Vector3 endPos = endNode.transform.position;
        return Vector3.Distance(nodePos, endPos);
    }

    private void SearchPath()
    {
        openList.Enqueue(curNode, curNode.f_cost);

        while (openList.Count > 0)
        {
            Node current = openList.Dequeue();
            closeList.Add(current);

            if (current == NodeManager.instance.endNode)
            {
                endNode = current;

                if (!visualizeRecursion)
                {
                    endNode.VisualizePath();
                }

                return;
            }

            IdentifySuccessors(current);
        }
    }

    private void IdentifySuccessors(Node node)
    {
        foreach (Vector2 dir in GetDirections(node))
        {
            Node jumpNode = Jump(node, dir);
            if (jumpNode != null && !closeList.Contains(jumpNode))
            {
                if (!openList.Contains(jumpNode))
                {
                    jumpNode.parentNode = node;
                    jumpNode.g_cost = node.g_cost + CalculateCost(node, jumpNode);
                    jumpNode.h_cost = CalculateHeuristic(jumpNode, NodeManager.instance.endNode);
                    jumpNode.f_cost = jumpNode.g_cost + jumpNode.h_cost;

                    openList.Enqueue(jumpNode, jumpNode.f_cost);
                }
            }
        }
    }

    private Node Jump(Node node, Vector2 direction)
    {
        int x = node.gridX + (int)direction.x;
        int y = node.gridY + (int)direction.y;

        if (!NodeManager.instance.IsWithinBounds(x, y) || NodeManager.instance.nodes[x, y].isObs)
        {
            return null;
        }

        Node nextNode = NodeManager.instance.nodes[x, y];

        if (nextNode.isObs)
            return null;

        if (visualizeRecursion)
        {
            if (!nextNode.isSearch)
                coroutine = StartCoroutine(DelayJump(nextNode));
        }

        if (nextNode == NodeManager.instance.endNode)
            return nextNode;

        if (IsForcedNeighbor(node, nextNode, direction))
            return nextNode;

        // 대각선 이동 시 직선 경로 검사
        if (direction.x != 0 && direction.y != 0)
        {
            Node jumpX = Jump(nextNode, new Vector2(direction.x, 0));
            Node jumpY = Jump(nextNode, new Vector2(0, direction.y));

            // 대각선 방향으로 직선 경로에 장애물이 있으면 해당 방향으로의 이동을 제한합니다.
            if ((jumpX != null && NodeManager.instance.nodes[jumpX.gridX, jumpX.gridY].isObs) &&
                (jumpY != null && NodeManager.instance.nodes[jumpY.gridX, jumpY.gridY].isObs))
            {
                return null;
            }
            // 부모 노드 기준으로 오른쪽 대각선 위에 있을 때
            else if (direction.x > 0 && direction.y > 0)
            {
                // 왼쪽 노드와 아래 노드의 isObs 검사
                if ((jumpX != null && NodeManager.instance.nodes[jumpX.gridX, jumpX.gridY].isObs) ||
                    (jumpY != null && NodeManager.instance.nodes[jumpY.gridX, jumpY.gridY].isObs))
                {
                    return null; // 이동 제한
                }
            }
            // 부모 노드 기준으로 오른쪽 대각선 아래에 있을 때
            else if (direction.x > 0 && direction.y < 0)
            {
                // 왼쪽 노드와 위 노드의 isObs 검사
                if ((jumpX != null && NodeManager.instance.nodes[jumpX.gridX, jumpX.gridY].isObs) ||
                    (jumpY != null && NodeManager.instance.nodes[jumpY.gridX, jumpY.gridY].isObs))
                {
                    return null; // 이동 제한
                }
            }
            // 부모 노드 기준으로 왼쪽 대각선 아래에 있을 때
            else if (direction.x < 0 && direction.y < 0)
            {
                // 오른쪽 노드와 위 노드의 isObs 검사
                if ((jumpX != null && NodeManager.instance.nodes[jumpX.gridX, jumpX.gridY].isObs) ||
                    (jumpY != null && NodeManager.instance.nodes[jumpY.gridX, jumpY.gridY].isObs))
                {
                    return null; // 이동 제한
                }
            }
            // 부모 노드 기준으로 왼쪽 대각선 위에 있을 때
            else if (direction.x < 0 && direction.y > 0)
            {
                // 오른쪽 노드와 아래 노드의 isObs 검사
                if ((jumpX != null && NodeManager.instance.nodes[jumpX.gridX, jumpX.gridY].isObs) ||
                    (jumpY != null && NodeManager.instance.nodes[jumpY.gridX, jumpY.gridY].isObs))
                {
                    return null; // 이동 제한
                }
            }

            // 대각선 방향으로 이동할 수 있으면 다음 노드 반환
            if (jumpX != null || jumpY != null)
            {
                return nextNode;
            }
        }

        // 다음 노드로 재귀적으로 점프
        return Jump(nextNode, direction);
    }

    IEnumerator DelayJump(Node node)
    {
        if (NodeManager.instance.searchNodes.Exists(n => n == node))
        {
            waitTime += 0.01f;
            waitForSeconds = new WaitForSeconds(waitTime);
        }
        else
        {
            yield break;
        }

        NodeManager.instance.searchNodes.Remove(node);

        yield return waitForSeconds;

        if (node == NodeManager.instance.endNode)
        {
            endNode.VisualizePath();
            StopAllCoroutines();
        }

        node.isSearch = true;
    }

    private bool IsForcedNeighbor(Node currentNode, Node nextNode, Vector2 direction)
    {
        int cx = currentNode.gridX;
        int cy = currentNode.gridY;
        int nx = nextNode.gridX;
        int ny = nextNode.gridY;

        if (direction.x != 0 && direction.y == 0)
        {
            // 수평 이동 시
            if ((NodeManager.instance.IsWithinBounds(cx, cy + 1) && NodeManager.instance.nodes[cx, cy + 1].isObs &&
                 NodeManager.instance.IsWithinBounds(nx, ny + 1) && !NodeManager.instance.nodes[nx, ny + 1].isObs) ||
                (NodeManager.instance.IsWithinBounds(cx, cy - 1) && NodeManager.instance.nodes[cx, cy - 1].isObs &&
                 NodeManager.instance.IsWithinBounds(nx, ny - 1) && !NodeManager.instance.nodes[nx, ny - 1].isObs))
            {
                return true;
            }
        }
        else if (direction.x == 0 && direction.y != 0)
        {
            // 수직 이동 시
            if ((NodeManager.instance.IsWithinBounds(cx + 1, cy) && NodeManager.instance.nodes[cx + 1, cy].isObs &&
                 NodeManager.instance.IsWithinBounds(nx + 1, ny) && !NodeManager.instance.nodes[nx + 1, ny].isObs) ||
                (NodeManager.instance.IsWithinBounds(cx - 1, cy) && NodeManager.instance.nodes[cx - 1, cy].isObs &&
                 NodeManager.instance.IsWithinBounds(nx - 1, ny) && !NodeManager.instance.nodes[nx - 1, ny].isObs))
            {
                return true;
            }
        }
        else if (direction.x != 0 && direction.y != 0)
        {
            // 대각선 이동 시
            if ((NodeManager.instance.IsWithinBounds(cx - (int)direction.x, cy) && NodeManager.instance.nodes[cx - (int)direction.x, cy].isObs &&
                 NodeManager.instance.IsWithinBounds(nx - (int)direction.x, ny) && !NodeManager.instance.nodes[nx - (int)direction.x, ny].isObs) ||
                (NodeManager.instance.IsWithinBounds(cx, cy - (int)direction.y) && NodeManager.instance.nodes[cx, cy - (int)direction.y].isObs &&
                 NodeManager.instance.IsWithinBounds(nx, ny - (int)direction.y) && !NodeManager.instance.nodes[nx, ny - (int)direction.y].isObs))
            {
                return true;
            }
        }

        return false;
    }

    private float CalculateCost(Node node, Node nextNode)
    {
        if (Mathf.Abs(node.gridX - nextNode.gridX) + Mathf.Abs(node.gridY - nextNode.gridY) == 1)
        {
            return straightStepCost;
        }
        else
        {
            return diagonalStepCost;
        }
    }

    private List<Vector2> GetDirections(Node node)
    {
        List<Vector2> directions = new List<Vector2>
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        void AddDiagonalDirection(Vector2 direction, Vector2 check1, Vector2 check2)
        {
            int x1 = node.gridX + (int)check1.x;
            int y1 = node.gridY + (int)check1.y;
            int x2 = node.gridX + (int)check2.x;
            int y2 = node.gridY + (int)check2.y;

            bool check1Valid = NodeManager.instance.IsWithinBounds(x1, y1) && !NodeManager.instance.nodes[x1, y1].isObs;
            bool check2Valid = NodeManager.instance.IsWithinBounds(x2, y2) && !NodeManager.instance.nodes[x2, y2].isObs;

            // 해당 대각선 방향의 주변에 장애물이 없고, 두 직선 경로가 모두 유효할 때 대각선 방향 추가
            if (check1Valid || check2Valid)
            {
                directions.Add(direction);
            }
        }

        AddDiagonalDirection(Vector2.up + Vector2.left, Vector2.left, Vector2.up);
        AddDiagonalDirection(Vector2.up + Vector2.right, Vector2.right, Vector2.up);
        AddDiagonalDirection(Vector2.down + Vector2.left, Vector2.left, Vector2.down);
        AddDiagonalDirection(Vector2.down + Vector2.right, Vector2.right, Vector2.down);

        return directions;
    }
}
