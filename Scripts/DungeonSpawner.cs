using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonSpawner : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator; // Reference to the DungeonGenerator script
    public GameObject zombiePrefab;
    public GameObject exitPrefab;
    public GameObject healthFountainPrefab;
    public GameObject shieldFountainPrefab;
    
    [SerializeField] private int numZombies;

    private Tilemap tilemap;

    private void Start()
    {
        tilemap = dungeonGenerator.floorTilemap;
        StartCoroutine(SpawnObjectsAsync());
    }

    private IEnumerator SpawnObjectsAsync()
    {
        yield return null; // Wait for one frame to ensure everything is initialized properly

        SpawnEntities(zombiePrefab, numZombies);
        SpawnEntity(exitPrefab);
        SpawnEntity(healthFountainPrefab);
        SpawnEntity(shieldFountainPrefab);
    }

    private void SpawnEntities(GameObject prefab, int numEntities)
    {
        List<Vector3> spawnPositions = GetRandomFloorTiles(numEntities);

        foreach (Vector3 spawnPosition in spawnPositions)
        {
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }

    private void SpawnEntity(GameObject prefab)
    {
        Vector3 spawnPosition = GetRandomFloorTile();
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    private List<Vector3> GetRandomFloorTiles(int count)
    {
        BoundsInt bounds = tilemap.cellBounds;
        List<Vector3> spawnPositions = new List<Vector3>();

        int attempts = 0;

        while (spawnPositions.Count < count && attempts < count * 10) // Limit attempts to prevent infinite loop
        {
            Vector3Int randomPosition = new Vector3Int(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                bounds.min.z
            );

            if (tilemap.GetTile(randomPosition) != null) // Check if the tile is a floor tile
            {
                spawnPositions.Add(tilemap.GetCellCenterWorld(randomPosition));
            }

            attempts++;
        }

        return spawnPositions;
    }

    private Vector3 GetRandomFloorTile()
    {
        BoundsInt bounds = tilemap.cellBounds;

        int attempts = 0;

        while (attempts < 100) // Limit attempts to prevent infinite loop
        {
            Vector3Int randomPosition = new Vector3Int(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                bounds.min.z
            );

            if (tilemap.GetTile(randomPosition) != null) // Check if the tile is a floor tile
            {
                return tilemap.GetCellCenterWorld(randomPosition);
            }

            attempts++;
        }

        return Vector3.zero;
    }
}