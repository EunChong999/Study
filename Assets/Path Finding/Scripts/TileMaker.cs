using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaker : MonoBehaviour
{
    [SerializeField]
    private GameObject tile;
    [SerializeField]
    private int maxX;
    [SerializeField]
    private int maxY;

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
                Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, transform);
            }
        }

        transform.position = new Vector3(-(x / 2), -(y / 2), 0);
    }
}
