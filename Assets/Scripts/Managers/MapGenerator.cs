using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject wallPrefabs;
    [SerializeField] private GameObject floorPrefabs;
    [SerializeField] private float noiseScale;
    
    [SerializeField] private int mapWidth = 50;
    [SerializeField] private int mapHeight = 50;
    [SerializeField] private Transform mapParent;

    private int [,] noiseMap;

    private void Start()
    {
        GenerateMap();
    }
    
    public void GenerateMap()
    {
        noiseMap = new int[mapWidth, mapHeight];
        float randomValue = Random.Range(0.1f, 0.2f);
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if ((x == (mapWidth-1)/2 || x == mapWidth/2) &&
                    (y == (mapHeight-1)/2 || y == mapHeight/2))
                {
                    noiseMap[x, y] = 0;
                }
                else
                {
                    float noise = Mathf.PerlinNoise(x * randomValue, y * randomValue);
                    noiseMap[x,y] = (noise > 0.5f) ? 1 : 0;
                }
            }
        }
        
        RenderMap();
    }

    private void RenderMap()
    {
        if(mapParent != null)
            Destroy(mapParent.gameObject);
        
        mapParent = new GameObject("MapParent").transform;
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2 position = new Vector2(x, y);
                if (noiseMap[x, y] == 1)
                {
                    Instantiate(wallPrefabs, position, Quaternion.identity, mapParent);
                }
                else
                {
                    Instantiate(floorPrefabs, position, Quaternion.identity, mapParent);
                }
            }
        }
    }
}
