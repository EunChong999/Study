using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3 position; 
    public float cost; 
    public Tile parent; 

    public Tile(Vector3 pos)
    {
        position = pos;
        cost = Mathf.Infinity;
        parent = null;
    }

    private GameObject obsTemp;

    private void Start()
    {
        obsTemp = Instantiate(ObsManager.instance.obs, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity, ObsManager.instance.transform);
        obsTemp.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            if (TileManager.instance.startTemp.activeSelf && TileManager.instance.startTemp.transform.position.Equals(transform.position))
                TileManager.instance.startTemp.SetActive(false);
            else
                TileManager.instance.startTemp.SetActive(true);

            TileManager.instance.startTemp.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        }
        else if (Input.GetMouseButton(1)) 
        {
            if (TileManager.instance.endTemp.activeSelf && TileManager.instance.endTemp.transform.position.Equals(transform.position))
                TileManager.instance.endTemp.SetActive(false);
            else
                TileManager.instance.endTemp.SetActive(true);

            TileManager.instance.endTemp.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        }
        else
        {
            if (Input.GetMouseButtonDown(2))
            {
                ObsManager.instance.isActive = true;
            }
            else if (Input.GetMouseButton(2))
            {
                if (ObsManager.instance.isActive)
                    obsTemp.SetActive(true);
            }
        }
    }
}
