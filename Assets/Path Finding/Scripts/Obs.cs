using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obs : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButton(2))
        {
            if (!ObsManager.instance.isActive)
                gameObject.SetActive(false);
        }
    }
}
