using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsManager : MonoBehaviour
{
    public GameObject obs;

    public static ObsManager instance;

    public bool isActive;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isActive = !isActive;
        }
    }
}
