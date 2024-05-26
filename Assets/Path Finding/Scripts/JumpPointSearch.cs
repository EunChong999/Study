using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpPointSearch : MonoBehaviour
{
    public int searchCount;
    public int preSearchCount;
    public Node endNode;

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

        if (!nextNode.isSearch) // 다음 노드가 탐색되지 않았을 때만 딜레이를 추가
            coroutine = StartCoroutine(DelayJump(nextNode));

        if (nextNode == NodeManager.instance.endNode)
            return nextNode;

        if (IsForcedNeighbor(nextNode, direction))
            return nextNode;

        if (direction.x != 0 && direction.y != 0)
        {
            if (Jump(nextNode, new Vector2(direction.x, 0)) != null || Jump(nextNode, new Vector2(0, direction.y)) != null)
                return nextNode;
        }
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

        preSearchCount++;

        yield return waitForSeconds;

        if (node == NodeManager.instance.endNode)
        {
            endNode.VisualizePath();
            StopAllCoroutines();
        }

        searchCount++;

        node.isSearch = true;
    }

    private bool IsForcedNeighbor(Node node, Vector2 direction)
    {
        int x = node.gridX;
        int y = node.gridY;

        if (direction.x != 0 && direction.y == 0)
        {
            if ((!NodeManager.instance.IsWithinBounds(x, y + 1) || NodeManager.instance.nodes[x, y + 1].isObs) &&
                NodeManager.instance.IsWithinBounds(x + (int)direction.x, y + 1) && !NodeManager.instance.nodes[x + (int)direction.x, y + 1].isObs)
                return true;

            if ((!NodeManager.instance.IsWithinBounds(x, y - 1) || NodeManager.instance.nodes[x, y - 1].isObs) &&
                NodeManager.instance.IsWithinBounds(x + (int)direction.x, y - 1) && !NodeManager.instance.nodes[x + (int)direction.x, y - 1].isObs)
                return true;
        }
        else if (direction.y != 0 && direction.x == 0)
        {
            if ((!NodeManager.instance.IsWithinBounds(x + 1, y) || NodeManager.instance.nodes[x + 1, y].isObs) &&
                NodeManager.instance.IsWithinBounds(x + 1, y + (int)direction.y) && !NodeManager.instance.nodes[x + 1, y + (int)direction.y].isObs)
                return true;

            if ((!NodeManager.instance.IsWithinBounds(x - 1, y) || NodeManager.instance.nodes[x - 1, y].isObs) &&
                NodeManager.instance.IsWithinBounds(x - 1, y + (int)direction.y) && !NodeManager.instance.nodes[x - 1, y + (int)direction.y].isObs)
                return true;
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
        Vector3 pos = node.transform.position;
        List<Vector2> directions = new List<Vector2>
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

        void AddDiagonalDirection(Vector2 direction)
        {
            bool isValidDirection = false; // 방향이 유효한지 확인하는 변수 추가
            foreach (Transform t in NodeManager.instance.nodeTransforms)
            {
                if (Mathf.Approximately(pos.x, t.position.x + 1) && Mathf.Approximately(pos.y, t.position.y + 1))
                {
                    bool isLeftObs = false;
                    bool isBelowObs = false;
                    isLeftObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x - 1) && Mathf.Approximately(x.position.y, pos.y))?.GetComponent<Node>().isObs ?? true;
                    isBelowObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x) && Mathf.Approximately(x.position.y, pos.y - 1))?.GetComponent<Node>().isObs ?? true;

                    if (!isLeftObs || !isBelowObs)
                    {
                        isValidDirection = true;
                    }
                }
                else if (Mathf.Approximately(pos.x, t.position.x + 1) && Mathf.Approximately(pos.y, t.position.y - 1))
                {
                    bool isAboveObs = false;
                    bool isLeftObs = false;
                    isAboveObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x) && Mathf.Approximately(x.position.y, pos.y + 1))?.GetComponent<Node>().isObs ?? true;
                    isLeftObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x - 1) && Mathf.Approximately(x.position.y, pos.y))?.GetComponent<Node>().isObs ?? true;

                    if (!isAboveObs || !isLeftObs)
                    {
                        isValidDirection = true;
                    }
                }
                else if (Mathf.Approximately(pos.x, t.position.x - 1) && Mathf.Approximately(pos.y, t.position.y + 1))
                {
                    bool isRightObs = false;
                    bool isBelowObs = false;
                    isRightObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x + 1) && Mathf.Approximately(x.position.y, pos.y))?.GetComponent<Node>().isObs ?? true;
                    isBelowObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x) && Mathf.Approximately(x.position.y, pos.y - 1))?.GetComponent<Node>().isObs ?? true;

                    if (!isRightObs || !isBelowObs)
                    {
                        isValidDirection = true;
                    }
                }
                else if (Mathf.Approximately(pos.x, t.position.x - 1) && Mathf.Approximately(pos.y, t.position.y - 1))
                {
                    bool isRightObs = false;
                    bool isAboveObs = false;
                    isRightObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x + 1) && Mathf.Approximately(x.position.y, pos.y))?.GetComponent<Node>().isObs ?? true;
                    isAboveObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x) && Mathf.Approximately(x.position.y, pos.y + 1))?.GetComponent<Node>().isObs ?? true;

                    if (!isRightObs || !isAboveObs)
                    {
                        isValidDirection = true;
                    }
                }
            }

            if (isValidDirection) // 모든 조건을 만족할 때만 방향 추가
            {
                directions.Add(direction);
            }
        }

        AddDiagonalDirection(Vector2.up + Vector2.left);
        AddDiagonalDirection(Vector2.up + Vector2.right);
        AddDiagonalDirection(Vector2.down + Vector2.left);
        AddDiagonalDirection(Vector2.down + Vector2.right);

        return directions;
    }

}
