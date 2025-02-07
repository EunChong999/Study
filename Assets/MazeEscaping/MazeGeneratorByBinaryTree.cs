using UnityEngine;

public class MazeGeneratorByBinaryTree : MonoBehaviour
{
    public int width;
    public int height;

    public int[,] map { get; private set; }

    private const int ROAD = 0;
    private const int WALL = 1;

    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject roadPrefab;

    [SerializeField] private Color roadColor = Color.white;
    [SerializeField] private Color wallColor = Color.black;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Generate();
    }

    private void Generate()
    {
        map = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 1 && y == 0) map[x, y] = ROAD; 
                else if (x == width - 2 && y == height - 1) map[x, y] = ROAD; 
                else if (x == 0 || x == width - 1 || y == 0 || y == height - 1) map[x, y] = WALL; 
                else if (x % 2 == 0 || y % 2 == 0) map[x, y] = WALL; 
                else map[x, y] = ROAD; 
            }
        }
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos;
                if (x % 2 == 0 || y % 2 == 0) continue; 
                if (x == width - 2 && y == height - 2) continue; 
                if (x == width - 2) pos = new Vector2Int(x, y + 1); 
                else if (y == height - 2) pos = new Vector2Int(x + 1, y); 
                else if (Random.Range(0, 2) == 0) pos = new Vector2Int(x + 1, y); 
                else pos = new Vector2Int(x, y + 1);
                map[pos.x, pos.y] = ROAD; 
            }
        }
        
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Create3DObject(x, y);
            }
        }
    }

    private void Create3DObject(int x, int y)
    {
        Vector3 position = new Vector3(-width / 2 + x, 0, -height / 2 + y);
        GameObject obj;

        if (map[x, y] == WALL)
        {
            obj = Instantiate(wallPrefab, position, Quaternion.identity, parent.transform);
            obj.GetComponent<MeshRenderer>().material.color = wallColor;
        }
        else
        {
            obj = Instantiate(roadPrefab, position, Quaternion.identity, parent.transform);
            obj.GetComponent<MeshRenderer>().material.color = roadColor;
        }
    }
}
