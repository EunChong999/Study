using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeManager : MonoBehaviour
{
    public bool isActive;

    public List<Transform> nodeTransforms;
    public List<Node> searchNodes;
    public Node[,] nodes = new Node[0, 0];

    public Material pathMat;
    public Material normalMat;
    public Material obsMat;
    public Material startMat;
    public Material endMat;
    public Material openMat;
    public Material closeMat;
    public Material searchMat;

    public Node startNode;
    public Node endNode;

    [SerializeField]
    private GameObject node;
    public int maxX;
    public int maxY;

    public static NodeManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        nodes = new Node[maxX, maxY];
        MakeTiles();
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < maxX && y < maxY;
    }

    private void MakeTiles()
    {
        float x = 0;
        float y = 0;

        for (x = 0; x < maxX; x++)
        {
            for (y = 0; y < maxY; y++)
            {
                GameObject temp = Instantiate(node, new Vector3(x, y, 0), Quaternion.identity, transform);
                temp.GetComponent<Node>().gridX = (int)x;
                temp.GetComponent<Node>().gridY = (int)y;
                nodes[temp.GetComponent<Node>().gridX, temp.GetComponent<Node>().gridY] = temp.GetComponent<Node>();
                nodeTransforms.Add(temp.transform);
                searchNodes.Add(temp.GetComponent<Node>());
            }
        }

        transform.position = new Vector3(-(x / 2), -(y / 2), 0);
    }
}
