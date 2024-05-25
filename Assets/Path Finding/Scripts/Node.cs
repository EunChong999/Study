using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float g_cost;
    public float h_cost;
    public float f_cost;

    public int gridX;
    public int gridY;

    public bool isPath;
    public bool isNormal;
    public bool isObs;
    public bool isOpen;
    public bool isClosed;

    public Renderer meshRenderer;
    public Node parentNode;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            if (isNormal)
            {
                NodeManager.instance.startNode = this;
                isNormal = false;
            }
        }
        else if (Input.GetMouseButton(1)) 
        {
            if (isNormal)
            {
                NodeManager.instance.endNode = this;
                isNormal = false;
            }
        }
        else
        {
            if (this != NodeManager.instance.startNode && 
                this != NodeManager.instance.endNode)
            {
                if (Input.GetMouseButtonDown(2))
                {
                    if (isObs)
                        NodeManager.instance.isActive = false;
                    else
                        NodeManager.instance.isActive = true;
                }
                else if (Input.GetMouseButton(2))
                {
                    if (NodeManager.instance.isActive)
                    {
                        if (isNormal)
                        {
                            isObs = true;
                            isNormal = false;
                        }
                    }
                    else
                    {
                        if (!isNormal)
                        {
                            isObs = false;
                            isNormal = true;
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (isObs)
            meshRenderer.sharedMaterial = NodeManager.instance.obsMat;
        else if (isPath)
        {
            meshRenderer.sharedMaterial = NodeManager.instance.pathMat;
            isNormal = false;
        }
        else if (this == NodeManager.instance.startNode)
        {
            meshRenderer.sharedMaterial = NodeManager.instance.startMat;
            isNormal = false;
        }
        else if (this == NodeManager.instance.endNode)
        {
            meshRenderer.sharedMaterial = NodeManager.instance.endMat;
            isNormal = false;
        }
        else if (isClosed)
        {
            meshRenderer.sharedMaterial = NodeManager.instance.closeMat;
            isOpen = false;
            isNormal = false;
        }
        else if (isOpen)
        {
            meshRenderer.sharedMaterial = NodeManager.instance.openMat;
            isClosed = false;
            isNormal = false;
        }
        else
        {
            meshRenderer.sharedMaterial = NodeManager.instance.normalMat;
            isNormal = true;
        }
    }

    public void VisualizePath()
    {
        isPath = true;

        if (parentNode == NodeManager.instance.startNode)
        {

            return;
        }

        parentNode.VisualizePath();
    }
}
