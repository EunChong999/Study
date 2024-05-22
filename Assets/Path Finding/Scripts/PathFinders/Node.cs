using UnityEngine;

public class Node
{
    public Vector3 position; // 노드의 위치
    public float cost; // 시작 노드부터 현재 노드까지의 비용
    public Node parent; // 이전 노드

    public Node(Vector3 pos)
    {
        position = pos;
        cost = Mathf.Infinity;
        parent = null;
    }
}
