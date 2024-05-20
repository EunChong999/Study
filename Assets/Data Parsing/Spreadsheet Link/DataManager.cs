using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    private int stageID;
    [SerializeField]
    private Stage stage;

    public GameObject block;
    public Transform env;

    int x;
    int y;

    private void Awake()
    {
        int index = 0;
        for (int i = 0; i < stage.Entities.Count; i++)
        {
            if (stage.Entities[i].stageID == stageID)
            {
                x = stage.Entities[i].x;
                y = stage.Entities[i].y;
            }
        }
    }

    void Start()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                var obj = Instantiate(block, env);
                obj.transform.localPosition = new Vector3(i, 0, j);
            }
        }
    }
}

