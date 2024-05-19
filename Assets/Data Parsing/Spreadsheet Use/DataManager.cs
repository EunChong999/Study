using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset data;
    private AllData datas;

    public int num;
    public GameObject block;
    public Transform env;

    private void Awake()
    {
        datas = JsonUtility.FromJson<AllData>(data.text);

        foreach (var VARIABLE in datas.stage)
        {
            print(VARIABLE.stageName);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int x = datas.stage[num].x;
        int y = datas.stage[num].y;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                var obj = Instantiate(block, env);
                obj.transform.position = new Vector3(i, 0, j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class AllData
{
    public MapData[] stage;
}

[System.Serializable]
public class MapData
{
    public int stageID;
    public string stageName;
    public int x;
    public int y;
}
