using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool isNormal;
    public bool isObs;
    public bool isOpen;
    public bool isClosed;

    public Renderer meshRenderer;

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
            if (this != NodeManager.instance.startNode && this != NodeManager.instance.endNode)
            {
                if (Input.GetMouseButtonDown(2))
                {
                    if (isObs)
                        ObsManager.instance.isActive = false;
                    else
                        ObsManager.instance.isActive = true;
                }
                else if (Input.GetMouseButton(2))
                {
                    if (ObsManager.instance.isActive)
                    {
                        isObs = true;
                        isNormal = false;
                    }
                    else
                    {
                        isObs = false;
                        isNormal = true;
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (isObs)
            meshRenderer.sharedMaterial = NodeManager.instance.obsMat;
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
        else
        {
            meshRenderer.sharedMaterial = NodeManager.instance.normalMat;
            isNormal = true;
        }
    }
}
