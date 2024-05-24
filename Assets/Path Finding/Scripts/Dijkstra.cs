using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public float step_cost;
    public float waitTime;

    public List<Node> openList = new List<Node>();
    public List<Node> closeList = new List<Node>();

    public Node curNode;

    WaitForSeconds waitForSeconds;

    private void Start()
    {
        waitForSeconds = new WaitForSeconds(waitTime);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindPath(NodeManager.instance.startNode);
        }
    }

    private void FindPath(Node start)
    {
       curNode = start;
       StartCoroutine(RunPathFInd(curNode, curNode.transform.position));
    }

    IEnumerator RunPathFInd(Node parent, Vector3 pos)
    {
        // 부모 노드를 먼저 closeList에 추가
        closeList.Add(parent);
        parent.isClosed = true;

        foreach (Transform t in NodeManager.instance.nodeTransforms)
        {
            Node n = t.GetComponent<Node>();

            if (!n.isObs && !n.isClosed)
            {
                // 좌표 값이 일치하면 노드 추가
                if (pos.x == t.position.x + 1 && pos.y == t.position.y)
                {
                    // 중복 추가 방지
                    if (!openList.Contains(n) && !closeList.Contains(n))
                    {
                        if (n == NodeManager.instance.endNode)
                        {
                            parent.VisualizePath();
                            yield break;
                        }

                        n.parentNode = parent;
                        openList.Add(n);
                        n.isOpen = true;
                        n.g_cost = curNode.g_cost + step_cost;
                    }
                }
                // 오른쪽
                else if (pos.x == t.position.x - 1 && pos.y == t.position.y)
                {
                    // 중복 추가 방지
                    if (!openList.Contains(n) && !closeList.Contains(n))
                    {
                        if (n == NodeManager.instance.endNode)
                        {
                            parent.VisualizePath();
                            yield break;
                        }

                        n.parentNode = parent;
                        openList.Add(n);
                        n.isOpen = true;
                        n.g_cost = curNode.g_cost + step_cost;
                    }
                }
                // 위
                else if (pos.x == t.position.x && pos.y == t.position.y - 1)
                {
                    // 중복 추가 방지
                    if (!openList.Contains(n) && !closeList.Contains(n))
                    {
                        if (n == NodeManager.instance.endNode)
                        {
                            parent.VisualizePath();
                            yield break;
                        }

                        n.parentNode = parent;
                        openList.Add(n);
                        n.isOpen = true;
                        n.g_cost = curNode.g_cost + step_cost;
                    }
                }
                // 아래
                else if (pos.x == t.position.x && pos.y == t.position.y + 1)
                {
                    // 중복 추가 방지
                    if (!openList.Contains(n) && !closeList.Contains(n))
                    {
                        if (n == NodeManager.instance.endNode)
                        {
                            parent.VisualizePath();
                            yield break;
                        }

                        n.parentNode = parent;
                        openList.Add(n);
                        n.isOpen = true;
                        n.g_cost = curNode.g_cost + step_cost;
                    }
                }
            }
        }

        // 더 이상 열린 노드가 없으면 종료
        if (openList.Count <= 0)
        {
            yield break;
        }

        // 가장 가까운 노드를 선택
        Node n1 = openList[0];

        foreach (Node n2 in openList)
        {
            if (n2.g_cost < n1.g_cost)
            {
                n1 = n2;
            }
        }

        // 선택한 노드를 openList와 closeList에서 제거
        openList.Remove(n1);

        // 다음 노드를 체크하기 위해 재귀 호출
        yield return waitForSeconds;
        StartCoroutine(RunPathFInd(n1, n1.transform.position));
    }
}
