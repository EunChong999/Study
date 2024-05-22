using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public Transform startTile; // 시작 타일
    public Transform targetTile; // 목표 타일
    public GameObject openTilePrefab; // 열린 타일 표시 오브젝트
    public GameObject closedTilePrefab; // 닫힌 타일 표시 오브젝트

    private List<Tile> tiles = new List<Tile>(); // 타일 리스트

    private void Start()
    {
        // Transform을 Tile 객체로 변환하여 tiles 리스트에 추가
        foreach (Transform t in TileManager.instance.tilesTransform)
        {
            tiles.Add(new Tile(t.position));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            List<Tile> path = FindPath(startTile.position, targetTile.position);
            if (path != null)
            {
                foreach (Tile tile in path)
                {
                    Debug.Log("Path Tile: " + tile.position);
                    // 여기서 3D 오브젝트로 경로 시각화 가능
                }
            }
        }
    }

    List<Tile> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Tile startTile = new Tile(startPos) { cost = 0 };
        Tile targetTile = new Tile(targetPos);

        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);
        Instantiate(openTilePrefab, startTile.position, Quaternion.identity); // 열린 타일 오브젝트 생성

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].cost < currentTile.cost)
                {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);
            Instantiate(closedTilePrefab, currentTile.position, Quaternion.identity); // 닫힌 타일 오브젝트 생성

            if (currentTile.position == targetTile.position)
            {
                return RetracePath(startTile, currentTile);
            }

            foreach (Tile neighbor in GetNeighbors(currentTile))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float newCostToNeighbor = currentTile.cost + Vector3.Distance(currentTile.position, neighbor.position);
                if (newCostToNeighbor < neighbor.cost || !openSet.Contains(neighbor))
                {
                    neighbor.cost = newCostToNeighbor;
                    neighbor.parent = currentTile;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                        Instantiate(openTilePrefab, neighbor.position, Quaternion.identity); // 열린 타일 오브젝트 생성
                    }
                }
            }
        }
        return null;
    }

    List<Tile> RetracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();
        Tile currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();
        return path;
    }

    List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        foreach (Tile t in tiles)
        {
            if (t.position != tile.position)
            {
                neighbors.Add(t);
            }
        }
        return neighbors;
    }
}
