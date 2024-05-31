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
        Debug.Assert(!(width % 2 == 0 || height % 2 == 0), "홀수로 입력하십시오.");
        if (Input.GetKeyDown(KeyCode.Space)) Generate();
    }

    private void Generate()
    {
        map = new int[width, height];

        // 미로 초기화
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 1 && y == 0) map[x, y] = ROAD; // 시작 위치
                else if (x == width - 2 && y == height - 1) map[x, y] = ROAD; // 출구 위치
                else if (x == 0 || x == width - 1 || y == 0 || y == height - 1) map[x, y] = WALL; // 가장자리 벽으로 채움
                else if (x % 2 == 0 || y % 2 == 0) map[x, y] = WALL; // 짝수 칸 벽으로 채움
                else map[x, y] = ROAD; // 나머지 칸에는 길 배치
            }
        }

        // 미로 생성
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos;
                if (x % 2 == 0 || y % 2 == 0) continue; // 짝수 칸은 건너 뜀
                if (x == width - 2 && y == height - 2) continue; // 우측 상단 모서리에 닿으면 길을 생성하지 않음
                if (x == width - 2) pos = new Vector2Int(x, y + 1); // 오른쪽 끝에 닿으면 길 생성 방향을 위로 설정
                else if (y == height - 2) pos = new Vector2Int(x + 1, y); // 위쪽 끝에 닿으면 길 생성 방향을 오른쪽으로 설정
                else if (Random.Range(0, 2) == 0) pos = new Vector2Int(x + 1, y); // 랜덤으로 방향 지정 (위쪽, 오른쪽)
                else pos = new Vector2Int(x, y + 1);
                map[pos.x, pos.y] = ROAD; // 맵 데이터에 값 저장
            }
        }

        // 기존의 타일맵 오브젝트를 모두 삭제
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }

        // 3D 오브젝트로 미로 생성
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Create3DObject(x, y);
            }
        }

        parent.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    private void Create3DObject(int x, int y)
    {
        Vector3 position = new Vector3(-width / 2 + x, 0, -height / 2 + y); // 생성 위치를 화면 중앙으로 설정
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
