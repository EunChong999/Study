using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JPS : MonoBehaviour
{
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
        StartCoroutine(CheckNeighbours());
    }

    IEnumerator CheckNeighbours()
    {


        // 다음 노드를 체크하기 위해 재귀 호출
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(CheckNeighbours());
    }
}
