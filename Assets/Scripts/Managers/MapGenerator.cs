using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private List<GameObject> coinPrefabs;
    
    [SerializeField] private float noiseScaleMin;
    [SerializeField] private float noiseScaleMax;
    
    [SerializeField] private int mapWidth = 50;
    [SerializeField] private int mapHeight = 50;
    
    [SerializeField] private Transform mapParent;

    [SerializeField] private int coinCount;

    private float noiseScale;
    private int[,] noiseMap;
    private List<Vector2Int> floorPositions = new List<Vector2Int>();
    
    private static readonly Vector2Int[] neighborOffsets = new Vector2Int[]
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
    };

    private void Start()
    {
        StartCoroutine(GenerateMap());
    }

    internal IEnumerator GenerateMap()
    {
        noiseMap = new int[mapWidth, mapHeight];
        floorPositions.Clear();
        noiseScale = Random.Range(noiseScaleMin, noiseScaleMax);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (IsCentralArea(x, y))
                {
                    noiseMap[x, y] = 0;
                }
                else if (IsBorder(x, y))
                {
                    noiseMap[x, y] = 1;
                }
                else
                {
                    float noiseValue = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
                    noiseMap[x, y] = (noiseValue > 0.5f) ? 1 : 0;
                }

                if (noiseMap[x, y] == 0)
                {
                    floorPositions.Add(new Vector2Int(x, y));
                }
            }
        }
        
        EnsureConnectivity();
        RenderMap();
        SpawnCoins();
        yield return null;
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
        {
            Destroy(mapParent.gameObject);
        }

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
                        
                        HashSet<Vector2Int> newVisited = BreadthFirstSearch(start);
                        foreach (var nv in newVisited)
                        {
                            visited.Add(nv);
                        }
                        
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
        List<Vector2Int> path = FindPathBFS(from, to);
        foreach (Vector2Int p in path)
        {
            noiseMap[p.x, p.y] = 0;
        }
    }

    private List<Vector2Int> FindPathBFS(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parents = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        parents[start] = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            
            if (current == goal)
                return ReconstructPath(parents, goal);
            
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!parents.ContainsKey(neighbor))
                {
                    parents[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        return new List<Vector2Int>();
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> parents, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = goal;
        
        while (parents[current] != current)
        {
            path.Add(current);
            current = parents[current];
        }
        path.Add(current);
        path.Reverse();
        return path;
    }
    
    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>(4);
        foreach (Vector2Int offset in neighborOffsets)
        {
            Vector2Int neighbor = pos + offset;
            if (neighbor.x >= 0 && neighbor.y >= 0 && neighbor.x < mapWidth && neighbor.y < mapHeight)
            {
                neighbors.Add(neighbor);
            }
        }
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
            
            int randomCoinNumber = Random.Range(0, coinPrefabs.Count);
            Instantiate(coinPrefabs[randomCoinNumber], new Vector3(pos.x, pos.y, 0), Quaternion.identity, mapParent);
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
