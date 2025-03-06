using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private LevelConfigurationSO levelConfiguration;

    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap floorTilemap;

    [SerializeField] private BoxCollider2D cameraBoundsCollider;
    [SerializeField] private CinemachineConfiner2D confiner2D;

    [SerializeField] private GameObject coinHolder;

    private int mapWidth;
    private int mapHeight;

    private float noiseScaleMin;
    private float noiseScaleMax;
    private float noiseScale;
    private int[,] noiseMap;

    private TileBase wallTile;
    private TileBase floorTile;

    private Grid mapGrid;

    private List<GameObject> obstacleWithCoinPrefabs;
    private int coinCount;

    private static readonly Vector2Int[] neighborOffsets = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private static readonly Vector2Int[] neighborOffsetsWithDiagonal = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1)
    };

    private void Start()
    {
        GameEvents.Current.OnGenerateMap += GenerateMap;
        
        GenerateMap();
        SetCameraBounds();
    }

    private void OnDisable()
    {
        GameEvents.Current.OnGenerateMap -= GenerateMap;
    }

    private void Awake()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        mapWidth = levelConfiguration.LevelWidth;
        mapHeight = levelConfiguration.LevelHeight;

        noiseScaleMin = levelConfiguration.NoiseScaleMin;
        noiseScaleMax = levelConfiguration.NoiseScaleMax;

        wallTile = levelConfiguration.WallTile;
        floorTile = levelConfiguration.FloorTile;

        obstacleWithCoinPrefabs = levelConfiguration.ObstacleWithCoinPrefabs;
        coinCount = levelConfiguration.CoinCount;
    }

    private void SetCameraBounds()
    {
        cameraBoundsCollider.offset = new Vector2((mapWidth - 1) / 2f, (mapHeight - 1) / 2f);
        cameraBoundsCollider.size = new Vector2(mapWidth - 1, mapHeight - 1);
        
        confiner2D.InvalidateBoundingShapeCache();
    }
    
    private void GenerateMap()
    {
        GeneratePerlinNoiseMap();
        HashSet<Vector2Int> avaliablefloors = BreadthFirstSearch();
        FillClosedSpaces(avaliablefloors);
        SpawnCoins(avaliablefloors);
        RenderMap();
    }

    private void GeneratePerlinNoiseMap()
    {
        noiseMap = new int[mapWidth, mapHeight];
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
            }
        }
    }

    private Vector2Int GetCenterPoint()
    {
        int xMiddle = mapWidth / 2;
        int yMiddle = mapHeight / 2;
        return new Vector2Int(xMiddle, yMiddle);
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
        wallTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();

        for (int x = 0; x < mapWidth; x++)
        for (int y = 0; y < mapHeight; y++)
            if (noiseMap[x, y] == 1)
                wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
            else
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
    }

    private void FillClosedSpaces(HashSet<Vector2Int> avaliableFloorsForPlayer)
    {
        for (int x = 1; x < mapWidth - 1; x++)
        for (int y = 1; y < mapHeight - 1; y++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (noiseMap[x, y] == 0 && !avaliableFloorsForPlayer.Contains(pos))
                noiseMap[x, y] = 1;
        }
    }

    private HashSet<Vector2Int> BreadthFirstSearch()
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        Vector2Int startPoint = GetCenterPoint();

        queue.Enqueue(startPoint);
        visited.Add(startPoint);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int dir in neighborOffsets)
            {
                Vector2Int neighbor = current + dir;

                if (noiseMap[neighbor.x, neighbor.y] == 0 && !visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return visited;
    }

    private void SpawnCoins(HashSet<Vector2Int> avaliableFloorsForCoins)
    {
        foreach (Transform child in coinHolder.transform)
            Destroy(child.gameObject);

        int placedCoins = 0;
        List<Vector2Int> avaliablePositions = new List<Vector2Int>(avaliableFloorsForCoins);
        HashSet<Vector2Int> unavailableFloorsForCoins = new HashSet<Vector2Int>();

        while (placedCoins < coinCount)
        {
            int randomIndex = Random.Range(0, avaliablePositions.Count);
            Vector2Int position = avaliablePositions[randomIndex];
            if (!IsCentralArea(position.x, position.y) && !unavailableFloorsForCoins.Contains(position))
            {
                int randomCoin = Random.Range(0, obstacleWithCoinPrefabs.Count);
                Vector2 coinPosition = new Vector2(position.x, position.y);
                GameObject coin = Instantiate(obstacleWithCoinPrefabs[randomCoin],
                    coinPosition,
                    Quaternion.identity,
                    coinHolder.transform);
                unavailableFloorsForCoins.Add(position);
                placedCoins++;
                foreach (var neighbor in neighborOffsetsWithDiagonal)
                {
                    unavailableFloorsForCoins.Add(position + neighbor);
                }
            }
        }
    }
}