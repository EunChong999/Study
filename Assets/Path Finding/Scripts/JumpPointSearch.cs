using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 점프 포인트 검색 알고리즘을 구현한 클래스
public class JumpPointSearch : MonoBehaviour
{
    // 시각적 재귀 표시를 위한 토글
    public Toggle visualizeRecursionToggle;

    // 시각적 재귀 표시 여부
    public bool visualizeRecursion;

    // 목표 노드
    private Node endNode;

    // 직선 및 대각선 이동 비용
    private float straightStepCost = 1.0f;
    private float diagonalStepCost = Mathf.Sqrt(2.0f);

    // 오픈 리스트 및 클로즈 리스트
    private PriorityQueue<Node> openList = new PriorityQueue<Node>();
    private HashSet<Node> closeList = new HashSet<Node>();

    // 딜레이 및 웨이팅 시간
    private float waitTime;
    private WaitForSeconds waitForSeconds;

    // 현재 노드
    public Node curNode;

    // 경로 찾기 메서드
    public void FindPath()
    {
        // 시작 노드 설정
        Node start = NodeManager.instance.startNode;

        // 시각적 재귀 표시 상태 업데이트
        visualizeRecursion = visualizeRecursionToggle.isOn;

        // 현재 노드 설정
        curNode = start;
        start.g_cost = 0;
        start.h_cost = CalculateHeuristic(start, NodeManager.instance.endNode);
        start.f_cost = start.g_cost + start.h_cost;

        // 경로 탐색 시작
        SearchPath();
    }

    // 휴리스틱(예측 비용) 계산
    private float CalculateHeuristic(Node node, Node endNode)
    {
        // 현재 노드 위치와 목표 노드 위치 사이의 거리를 반환
        Vector3 nodePos = node.transform.position;
        Vector3 endPos = endNode.transform.position;
        return Vector3.Distance(nodePos, endPos);
    }

    // 경로 탐색 메서드
    private void SearchPath()
    {
        // 시작 노드를 오픈 리스트에 추가
        openList.Enqueue(curNode, curNode.f_cost);

        // 오픈 리스트가 비어있지 않은 동안 반복
        while (openList.Count > 0)
        {
            // 현재 노드를 오픈 리스트에서 추출
            Node current = openList.Dequeue();
            closeList.Add(current);

            // 현재 노드가 목표 노드인 경우
            if (current == NodeManager.instance.endNode)
            {
                // 목표 노드 설정 및 시각적 경로 표시
                endNode = current;
                if (!visualizeRecursion)
                {
                    endNode.VisualizePath();
                }
                return;
            }

            // 이웃 노드 식별
            IdentifySuccessors(current);
        }
    }

    // 이웃 노드 식별 메서드
    private void IdentifySuccessors(Node node)
    {
        // 모든 방향에 대해 이웃 노드를 찾고 처리
        foreach (Vector2 dir in GetDirections())
        {
            Node jumpNode = Jump(node, dir);

            // 이웃 노드 정보 업데이트 및 오픈 리스트에 추가
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

    // 점프 메서드
    private Node Jump(Node node, Vector2 direction)
    {
        // 다음 이동할 위치 계산
        int x = node.gridX + (int)direction.x;
        int y = node.gridY + (int)direction.y;

        // 다음 이동할 위치가 유효한지 확인
        if (!NodeManager.instance.IsWithinBounds(x, y) || NodeManager.instance.nodes[x, y].isObs || IsDiagonalBlocked(node, direction))
        {
            return null; // 유효하지 않은 이동일 경우 null 반환
        }

        // 다음 이동할 노드 설정
        Node nextNode = NodeManager.instance.nodes[x, y];

        // 시각적 재귀 표시가 활성화된 경우
        if (visualizeRecursion)
        {
            if (!nextNode.isSearch)
                StartCoroutine(DelayJump(nextNode));
        }

        // 목표 노드인 경우 반환
        if (nextNode == NodeManager.instance.endNode)
            return nextNode;

        // 강제 이웃 검사
        if (IsForcedNeighbor(node, nextNode, direction))
        {
            return nextNode;
        }

        // 대각선 이동인 경우
        if (direction.x != 0 && direction.y != 0)
        {
            if (Jump(nextNode, new Vector2(direction.x, 0)) != null || Jump(nextNode, new Vector2(0, direction.y)) != null)
                return nextNode;
        }

        // 대각선 이동이 아닌 경우 계속 진행
        return Jump(nextNode, direction);
    }

    // 딜레이를 준 후 점프 메서드 호출
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

    // 강제 이웃인지 확인
    // 강제 이웃 검사 메서드
    private bool IsForcedNeighbor(Node currentNode, Node nextNode, Vector2 direction)
    {
        int cx = currentNode.gridX;
        int cy = currentNode.gridY;
        int nx = nextNode.gridX;
        int ny = nextNode.gridY;

        if (direction.x != 0) // 수평 이동인 경우
        {
            // 이동할 방향의 위쪽 또는 아래쪽 인접한 노드가 장애물이고, 다음 노드의 해당 방향 인접한 노드가 장애물이 아닌 경우 강제 이웃으로 판단
            if ((NodeManager.instance.IsWithinBounds(cx, cy + 1) && NodeManager.instance.nodes[cx, cy + 1].isObs && !NodeManager.instance.nodes[nx, ny + 1].isObs) ||
                (NodeManager.instance.IsWithinBounds(cx, cy - 1) && NodeManager.instance.nodes[cx, cy - 1].isObs && !NodeManager.instance.nodes[nx, ny - 1].isObs))
            {
                return true;
            }
        }
        else if (direction.y != 0) // 수직 이동인 경우
        {
            // 이동할 방향의 왼쪽 또는 오른쪽 인접한 노드가 장애물이고, 다음 노드의 해당 방향 인접한 노드가 장애물이 아닌 경우 강제 이웃으로 판단
            if ((NodeManager.instance.IsWithinBounds(cx + 1, cy) && NodeManager.instance.nodes[cx + 1, cy].isObs && !NodeManager.instance.nodes[nx + 1, ny].isObs) ||
                (NodeManager.instance.IsWithinBounds(cx - 1, cy) && NodeManager.instance.nodes[cx - 1, cy].isObs && !NodeManager.instance.nodes[nx - 1, ny].isObs))
            {
                return true;
            }
        }

        return false;
    }

    // 대각선 이동 시 장애물 검사
    private bool IsDiagonalBlocked(Node node, Vector2 direction)
    {
        int x = node.gridX + (int)direction.x;
        int y = node.gridY + (int)direction.y;

        // 대각선 이동할 위치가 맵 범위를 벗어나거나 장애물인 경우 true 반환
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

    // 이동 비용 계산
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

    // 가능한 방향 목록 반환
    private List<Vector2> GetDirections()
    {
        List<Vector2> directions = new List<Vector2>
        {
            Vector2.up, 
            Vector2.down, 
            Vector2.left, 
            Vector2.right, 
            Vector2.up + Vector2.left, 
            Vector2.up + Vector2.right, 
            Vector2.down + Vector2.left, 
            Vector2.down + Vector2.right
        };

        return directions;
    }
}

