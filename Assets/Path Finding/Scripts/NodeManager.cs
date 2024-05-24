using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public bool isActive;

    public List<Transform> nodeTransforms;

    public Material pathMat;
    public Material normalMat;
    public Material obsMat;
    public Material startMat;
    public Material endMat;
    public Material openMat;
    public Material closeMat;

    public Node startNode;
    public Node endNode;

    [SerializeField]
    private GameObject node;
    [SerializeField]
    private int maxX;
    [SerializeField]
    private int maxY;

    public static NodeManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        MakeTiles();
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
                nodeTransforms.Add(temp.transform);
            }
        }

        transform.position = new Vector3(-(x / 2), -(y / 2), 0);
    }
}
