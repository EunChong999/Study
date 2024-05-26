﻿using System.Collections;
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
        foreach (Vector2 dir in GetDirections())
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
        // 다음 이동할 노드의 좌표 계산
        int x = node.gridX + (int)direction.x;
        int y = node.gridY + (int)direction.y;

        // 다음 이동할 노드가 범위를 벗어나거나 장애물인 경우
        if (!NodeManager.instance.IsWithinBounds(x, y) || NodeManager.instance.nodes[x, y].isObs || IsDiagonalBlocked(node, direction))
        {
            return null; // 유효한 이동이 아니므로 null 반환
        }

        // 다음 이동할 노드
        Node nextNode = NodeManager.instance.nodes[x, y];

        // 시각적 재귀 표시가 활성화된 경우
        if (visualizeRecursion)
        {
            // 다음 노드가 이미 탐색되지 않았다면
            if (!nextNode.isSearch)
                coroutine = StartCoroutine(DelayJump(nextNode)); // 딜레이를 주고 재귀 호출 시작
        }

        // 다음 노드가 목표 노드인 경우
        if (nextNode == NodeManager.instance.endNode)
            return nextNode; // 목표 노드를 반환하여 탐색 종료

        // 강제 이웃 검사
        if (IsForcedNeighbor(node, nextNode, direction))
        {
            return nextNode;
        }

        // 대각선 이동인 경우
        if (direction.x != 0 && direction.y != 0)
        {
            // 대각선 방향으로의 이동을 고려하여 재귀 호출
            if (Jump(nextNode, new Vector2(direction.x, 0)) != null || Jump(nextNode, new Vector2(0, direction.y)) != null)
                return nextNode; // 이동 가능한 경우 다음 노드를 반환하여 탐색 종료
        }

        // 대각선 이동이 아닌 경우, 현재 방향으로 이동을 계속 진행
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

    private bool IsDiagonalBlocked(Node node, Vector2 direction)
    {
        int x = node.gridX + (int)direction.x;
        int y = node.gridY + (int)direction.y;

        // 대각선 이동할 위치가 맵 밖이거나 장애물인 경우 true 반환
        if (!NodeManager.instance.IsWithinBounds(x, y) || NodeManager.instance.nodes[x, y].isObs)
        {
            return true;
        }

        // 대각선 방향으로 이동할 때, 인접한 두 개의 노드가 모두 장애물이면 true 반환
        int adjacentX = node.gridX + Mathf.RoundToInt(direction.x);
        int adjacentY = node.gridY;
        if (NodeManager.instance.IsWithinBounds(adjacentX, adjacentY) && NodeManager.instance.nodes[adjacentX, adjacentY].isObs)
        {
            adjacentX = node.gridX;
            adjacentY = node.gridY + Mathf.RoundToInt(direction.y);
            if (NodeManager.instance.IsWithinBounds(adjacentX, adjacentY) && NodeManager.instance.nodes[adjacentX, adjacentY].isObs)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false; // 대각선 방향으로 이동 가능
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

    // 가능한 방향 목록을 반환합니다.
    private List<Vector2> GetDirections()
    {
        List<Vector2> directions = new List<Vector2>
        {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right
        };

        // 대각선 방향을 추가합니다.
        directions.AddRange(new List<Vector2> { Vector2.up + Vector2.left, Vector2.up + Vector2.right, Vector2.down + Vector2.left, Vector2.down + Vector2.right });

        return directions;
    }
}
