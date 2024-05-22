using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private GameObject start;
    [SerializeField]
    private GameObject end;

    [HideInInspector]
    public GameObject startTemp;
    [HideInInspector]
    public GameObject endTemp;

    public static TileManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        startTemp = Instantiate(start, transform.position, Quaternion.identity);
        endTemp = Instantiate(end, transform.position, Quaternion.identity);
        startTemp.SetActive(false);
        endTemp.SetActive(false);
    }
}
