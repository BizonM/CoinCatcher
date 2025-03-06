using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "ScriptableObject/LevelConfiguration")]
public class LevelConfigurationSO : ScriptableObject
{
    public int LevelWidth;
    public int LevelHeight;

    public float NoiseScaleMin;
    public float NoiseScaleMax;
    
    public List<GameObject> ObstacleWithCoinPrefabs;
    public int CoinCount;

    public int CoinValueToComplete;
    
    public TileBase WallTile; 
    public TileBase FloorTile;
}
