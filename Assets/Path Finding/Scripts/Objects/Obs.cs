using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Obs : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(2))
        {
            ObsManager.instance.isActive = false;
        }
        else if (Input.GetMouseButton(2))
        {
            if (!ObsManager.instance.isActive)
                gameObject.SetActive(false);
        }
    }
}
