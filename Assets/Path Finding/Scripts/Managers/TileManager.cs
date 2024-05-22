using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public List<Tile> tiles;    
    public List<Transform> tilesTransform;    

    [SerializeField]
    private GameObject tile;
    [SerializeField]
    private int maxX;
    [SerializeField]
    private int maxY;
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
        MakeTiles();
        startTemp = start;
        endTemp = end;
        startTemp.SetActive(false);
        endTemp.SetActive(false);
    }

    private void MakeTiles()
    {
        float x = 0;
        float y = 0;

        for (x = 0; x < maxX; x++)
        {
            for (y = 0; y < maxY; y++)
            {
                GameObject temp = Instantiate(tile, new Vector3(x, y, 0), Quaternion.identity, transform);
                tiles.Add(temp.GetComponent<Tile>());
                tilesTransform.Add(temp.transform);
            }
        }

        transform.position = new Vector3(-(x / 2), -(y / 2), 0);
    }
}
