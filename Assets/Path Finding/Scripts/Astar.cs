using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Astar : MonoBehaviour
{
    public bool allowDiagonal;
    public bool crossCorners;

    private float straightStepCost = 1.0f;
    private float diagonalStepCost = Mathf.Sqrt(2.0f);

    public List<Node> openList = new List<Node>();
    public List<Node> closeList = new List<Node>();

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
        StartCoroutine(CheckNeighbours(curNode));
    }

    IEnumerator CheckNeighbours(Node parent)
    {
        Vector3 pos = parent.transform.position;
        closeList.Add(parent);
        parent.isClosed = true;

        foreach (Transform t in NodeManager.instance.nodeTransforms)
        {
            Node n = t.GetComponent<Node>();

            if (!n.isObs && !n.isClosed)
            {
                float additionalCost = 0;
                bool isNeighbor = false;

                // 상하좌우 및 대각선 방향 검사
                if (Mathf.Approximately(pos.x, t.position.x + 1) && Mathf.Approximately(pos.y, t.position.y))
                {
                    isNeighbor = true;
                    additionalCost = straightStepCost; // 오른쪽
                }
                else if (Mathf.Approximately(pos.x, t.position.x - 1) && Mathf.Approximately(pos.y, t.position.y))
                {
                    isNeighbor = true;
                    additionalCost = straightStepCost; // 왼쪽
                }
                else if (Mathf.Approximately(pos.x, t.position.x) && Mathf.Approximately(pos.y, t.position.y + 1))
                {
                    isNeighbor = true;
                    additionalCost = straightStepCost; // 위
                }
                else if (Mathf.Approximately(pos.x, t.position.x) && Mathf.Approximately(pos.y, t.position.y - 1))
                {
                    isNeighbor = true;
                    additionalCost = straightStepCost; // 아래
                }
                else if (allowDiagonal)
                {
                    if (Mathf.Approximately(pos.x, t.position.x + 1) && Mathf.Approximately(pos.y, t.position.y + 1))
                    {
                        bool isLeftObs = false;
                        bool isBelowObs = false;
                        isLeftObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x - 1) && Mathf.Approximately(x.position.y, pos.y))?.GetComponent<Node>().isObs ?? true;
                        isBelowObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x) && Mathf.Approximately(x.position.y, pos.y - 1))?.GetComponent<Node>().isObs ?? true;

                        if (crossCorners)
                        {
                            if (!isLeftObs || !isBelowObs)
                            {
                                isNeighbor = true;
                                additionalCost = diagonalStepCost; // 오른쪽 위 대각선
                            }
                        }
                        else
                        {
                            if (!isLeftObs && !isBelowObs)
                            {
                                isNeighbor = true;
                                additionalCost = diagonalStepCost; // 오른쪽 위 대각선
                            }
                        }
                    }
                    else if (Mathf.Approximately(pos.x, t.position.x + 1) && Mathf.Approximately(pos.y, t.position.y - 1))
                    {
                        bool isAboveObs = false;
                        bool isLeftObs = false;
                        isAboveObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x) && Mathf.Approximately(x.position.y, pos.y + 1))?.GetComponent<Node>().isObs ?? true;
                        isLeftObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x - 1) && Mathf.Approximately(x.position.y, pos.y))?.GetComponent<Node>().isObs ?? true;

                        if (crossCorners)
                        {
                            if (!isAboveObs || !isLeftObs)
                            {
                                isNeighbor = true;
                                additionalCost = diagonalStepCost; // 오른쪽 아래 대각선
                            }
                        }
                        else
                        {
                            if (!isAboveObs && !isLeftObs)
                            {
                                isNeighbor = true;
                                additionalCost = diagonalStepCost; // 오른쪽 아래 대각선
                            }
                        }
                    }
                    else if (Mathf.Approximately(pos.x, t.position.x - 1) && Mathf.Approximately(pos.y, t.position.y + 1))
                    {
                        bool isRightObs = false;
                        bool isBelowObs = false;
                        isRightObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x + 1) && Mathf.Approximately(x.position.y, pos.y))?.GetComponent<Node>().isObs ?? true;
                        isBelowObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x) && Mathf.Approximately(x.position.y, pos.y - 1))?.GetComponent<Node>().isObs ?? true;

                        if (crossCorners)
                        {
                            if (!isRightObs || !isBelowObs)
                            {
                                isNeighbor = true;
                                additionalCost = diagonalStepCost; // 왼쪽 위 대각선
                            }
                        }
                        else
                        {
                            if (!isRightObs && !isBelowObs)
                            {
                                isNeighbor = true;
                                additionalCost = diagonalStepCost; // 왼쪽 위 대각선
                            }
                        }
                    }
                    else if (Mathf.Approximately(pos.x, t.position.x - 1) && Mathf.Approximately(pos.y, t.position.y - 1))
                    {
                        bool isRightObs = false;
                        bool isAboveObs = false;
                        isRightObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x + 1) && Mathf.Approximately(x.position.y, pos.y))?.GetComponent<Node>().isObs ?? true;
                        isAboveObs = NodeManager.instance.nodeTransforms.Find(x => Mathf.Approximately(x.position.x, pos.x) && Mathf.Approximately(x.position.y, pos.y + 1))?.GetComponent<Node>().isObs ?? true;

                        if (crossCorners)
                        {
                            if (!isRightObs || !isAboveObs)
                            {
                                isNeighbor = true;
                                additionalCost = diagonalStepCost; // 왼쪽 아래 대각선
                            }
                        }
                        else
                        {
                            if (!isRightObs && !isAboveObs)
                            {
                                isNeighbor = true;
                                additionalCost = diagonalStepCost; // 왼쪽 아래 대각선
                            }
                        }
                    }
                }

                if (isNeighbor && !openList.Contains(n) && !closeList.Contains(n))
                {
                    if (n == NodeManager.instance.endNode)
                    {
                        parent.VisualizePath();
                        yield break;
                    }

                    n.parentNode = parent;
                    n.g_cost = parent.g_cost + additionalCost;
                    n.h_cost = Vector3.Distance(n.transform.position, NodeManager.instance.endNode.transform.position); // Heuristic cost
                    n.f_cost = n.g_cost + n.h_cost; // Total cost
                    openList.Add(n);
                    n.isOpen = true;
                }
            }
        }

        // 더 이상 열린 노드가 없으면 종료
        if (openList.Count <= 0)
        {
            yield break;
        }

        // 가장 낮은 f_cost를 가진 노드를 선택
        Node n1 = openList[0];

        foreach (Node n2 in openList)
        {
            if (n2.f_cost < n1.f_cost || (n2.f_cost == n1.f_cost && n2.h_cost < n1.h_cost))
            {
                n1 = n2;
            }
        }

        // 선택한 노드를 openList에서 제거
        openList.Remove(n1);

        // 다음 노드를 체크하기 위해 재귀 호출
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(CheckNeighbours(n1));
    }
}
