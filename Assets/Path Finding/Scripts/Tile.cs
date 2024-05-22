using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private GameObject obsTemp;

    private void Start()
    {
        obsTemp = Instantiate(ObsManager.instance.obs, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity);
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

        if (Input.GetMouseButton(1)) 
        {
            if (TileManager.instance.endTemp.activeSelf && TileManager.instance.endTemp.transform.position.Equals(transform.position))
                TileManager.instance.endTemp.SetActive(false);
            else
                TileManager.instance.endTemp.SetActive(true);

            TileManager.instance.endTemp.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        }

        if (Input.GetMouseButton(2)) 
        {
            if (ObsManager.instance.isActive)
                obsTemp.SetActive(true);
        }
    }
}
