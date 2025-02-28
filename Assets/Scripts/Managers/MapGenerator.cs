using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject coinPrefab;
    
    [SerializeField] private float noiseScaleMin;
    [SerializeField] private float noiseScaleMax;

    [SerializeField] private int mapWidth = 50;
    [SerializeField] private int mapHeight = 50;
    
    [SerializeField] private Transform mapParent;

    [SerializeField] private int coinCount;

    private int[,] noiseMap;
    private List<Vector2Int> floorPositions = new List<Vector2Int>();

    private void Start()
    {
        GenerateMap();
    }

    internal void GenerateMap()
    {
        noiseMap = new int[mapWidth, mapHeight];
        floorPositions.Clear();
        float noiseScale = Random.Range(noiseScaleMin, noiseScaleMax);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsCentralArea(x, y))
                    noiseMap[x, y] = 0;
                else if (IsBorder(x, y))
                    noiseMap[x, y] = 1;
                else
                    noiseMap[x, y] = Mathf.PerlinNoise(x * noiseScale, y * noiseScale) > 0.5f ? 1 : 0;

                if (noiseMap[x, y] == 0)
                    floorPositions.Add(new Vector2Int(x, y));
            }
        }
        
        EnsureConnectivity();
        RenderMap();
        SpawnCoins();
    }

    private bool IsCentralArea(int x, int y)
    {
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;
        return Mathf.Abs(x - centerX) <= 1 && Mathf.Abs(y - centerY) <= 1;
    }

    private bool IsBorder(int x, int y)
    {
        return x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1;
    }

    private void RenderMap()
    {
        if (mapParent != null)
            Destroy(mapParent.gameObject);

        mapParent = new GameObject("MapParent").transform;
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (noiseMap[x, y] == 1)
                {
                    Instantiate(wallPrefab, new Vector2(x, y), Quaternion.identity, mapParent);
                }
            }
        }
    }

    private void EnsureConnectivity()
    {
        Vector2Int start = new Vector2Int(mapWidth / 2, mapHeight / 2);
        HashSet<Vector2Int> visited = BreadthFirstSearch(start);

        for (int x = 1; x < mapWidth - 1; x++)
        {
            for (int y = 1; y < mapHeight - 1; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (noiseMap[x, y] == 0 && !visited.Contains(pos))
                {
                    CreatePathToNearestAccessible(pos, visited);
                }
            }
        }
    }

    private HashSet<Vector2Int> BreadthFirstSearch(Vector2Int start)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && noiseMap[neighbor.x, neighbor.y] == 0)
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return visited;
    }

    private void CreatePathToNearestAccessible(Vector2Int start, HashSet<Vector2Int> visited)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> localVisited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        localVisited.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!localVisited.Contains(neighbor))
                {
                    if (visited.Contains(neighbor))
                    {
                        ConnectPoints(start, neighbor);
                        return;
                    }

                    queue.Enqueue(neighbor);
                    localVisited.Add(neighbor);
                }
            }
        }
    }

    private void ConnectPoints(Vector2Int from, Vector2Int to)
    {
        Vector2Int current = from;

        while (current != to)
        {
            noiseMap[current.x, current.y] = 0;

            if (Random.value > 0.5f)
            {
                current.x += Mathf.Clamp(to.x - current.x, -1, 1);
            }
            else
            {
                current.y += Mathf.Clamp(to.y - current.y, -1, 1);
            }
        }
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(pos.x - 1, pos.y),
            new Vector2Int(pos.x + 1, pos.y),
            new Vector2Int(pos.x, pos.y - 1),
            new Vector2Int(pos.x, pos.y + 1)
        };

        neighbors.RemoveAll(n => n.x < 0 || n.y < 0 || n.x >= mapWidth || n.y >= mapHeight);
        return neighbors;
    }

    private void SpawnCoins()
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = 1; x < mapWidth - 1; x++)
        {
            for (int y = 1; y < mapHeight - 1; y++)
            {
                if (IsValidCoinPosition(x, y))
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i = 0; i < coinCount && validPositions.Count > 0; i++)
        {
            int index = Random.Range(0, validPositions.Count);
            Vector2Int pos = validPositions[index];
            validPositions.RemoveAt(index);

            Instantiate(coinPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, mapParent);
        }
    }

    private bool IsValidCoinPosition(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (noiseMap[x + dx, y + dy] == 1) 
                    return false;
            }
        }
        return true;
    }
}
