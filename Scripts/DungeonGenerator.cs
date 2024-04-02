using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    [SerializeField] private TileBase[] floorTiles;
    [SerializeField] private TileBase[] wallTiles;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int minRoomSize;
    [SerializeField] private int maxRoomSize;
    [SerializeField] private int numRooms;
    [SerializeField] private float minRoomSpacing;

    private void Start()
    {
        GenerateDungeon();
        AddTilemapCollidersToWalls();
        PositionPlayerOnFloorTile();
    }

    private void GenerateDungeon()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        List<Rect> rooms = new List<Rect>();

        for (int i = 0; i < numRooms; i++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);
            int x = Random.Range(0, width - roomWidth);
            int y = Random.Range(0, height - roomHeight);

            Rect newRoom = new Rect(x, y, roomWidth, roomHeight);
            bool overlaps = false;

            // Check if the new room overlaps with existing rooms or is too close to them
            foreach (Rect room in rooms)
            {
                if (newRoom.Overlaps(room) || Vector2.Distance(newRoom.center, room.center) < minRoomSpacing)
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                rooms.Add(newRoom);

                for (int rx = x; rx < x + roomWidth; rx++)
                {
                    for (int ry = y; ry < y + roomHeight; ry++)
                    {
                        floorTilemap.SetTile(new Vector3Int(rx, ry, 0), floorTiles[Random.Range(0, floorTiles.Length)]);
                    }
                }
            }
        }

        ConnectAllRooms(rooms);
        AddWalls();
    }


    private void ConnectAllRooms(List<Rect> rooms)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                ConnectRooms(rooms[i], rooms[j]);
            }
        }
    }

    void ConnectRooms(Rect room1, Rect room2)
    {
        Vector2Int center1 = new Vector2Int((int)room1.center.x, (int)room1.center.y);
        Vector2Int center2 = new Vector2Int((int)room2.center.x, (int)room2.center.y);

        // Determine whether to create a vertical or horizontal hallway
        bool createVerticalHallway = Random.value > 0.5f;

        if (createVerticalHallway)
        {
            int startHallwayX = center1.x;
            int endHallwayX = center2.x;
            int hallwayY = center1.y;
            int hallwayWidth = 2; // Hallway width of two tiles

            // Connect the vertical part of the hallway
            for (int x = startHallwayX - hallwayWidth / 2; x <= startHallwayX + hallwayWidth / 2; x++)
            {
                for (int hY = Mathf.Min(hallwayY, center2.y); hY <= Mathf.Max(hallwayY, center2.y); hY++)
                {
                    floorTilemap.SetTile(new Vector3Int(x, hY, 0), floorTiles[Random.Range(0, floorTiles.Length)]);

                    // Check for obstructing walls and remove them if necessary
                    RemoveObstructingWall(new Vector3Int(x, hY, 0));
                }
            }

            // Connect the horizontal part of the hallway
            for (int y = Mathf.Min(hallwayY, center2.y) + 1; y <= Mathf.Max(hallwayY, center2.y) - 1; y++)
            {
                for (int hX = Mathf.Min(startHallwayX, endHallwayX); hX <= Mathf.Max(startHallwayX, endHallwayX); hX++)
                {
                    floorTilemap.SetTile(new Vector3Int(hX, y, 0), floorTiles[Random.Range(0, floorTiles.Length)]);

                    // Check for obstructing walls and remove them if necessary
                    RemoveObstructingWall(new Vector3Int(hX, y, 0));
                }
            }
        }
        else // Horizontal hallway
        {
            int startHallwayY = center1.y;
            int endHallwayY = center2.y;
            int hallwayX = center1.x;
            int hallwayHeight = 2; // Hallway height of two tiles

            // Connect the horizontal part of the hallway
            for (int y = startHallwayY - hallwayHeight / 2; y <= startHallwayY + hallwayHeight / 2; y++)
            {
                for (int hX = Mathf.Min(hallwayX, center2.x); hX <= Mathf.Max(hallwayX, center2.x); hX++)
                {
                    floorTilemap.SetTile(new Vector3Int(hX, y, 0), floorTiles[Random.Range(0, floorTiles.Length)]);

                    // Check for obstructing walls and remove them if necessary
                    RemoveObstructingWall(new Vector3Int(hX, y, 0));
                }
            }

            // Connect the vertical part of the hallway
            for (int x = Mathf.Min(hallwayX, center2.x) + 1; x <= Mathf.Max(hallwayX, center2.x) - 1; x++)
            {
                for (int hY = Mathf.Min(startHallwayY, endHallwayY); hY <= Mathf.Max(startHallwayY, endHallwayY); hY++)
                {
                    floorTilemap.SetTile(new Vector3Int(x, hY, 0), floorTiles[Random.Range(0, floorTiles.Length)]);

                    // Check for obstructing walls and remove them if necessary
                    RemoveObstructingWall(new Vector3Int(x, hY, 0));
                }
            }
        }
    }


    void RemoveObstructingWall(Vector3Int position)
    {
        if (wallTilemap.HasTile(position))
        {
            wallTilemap.SetTile(position, null);
        }
    }


    private void AddWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase tile = floorTilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    AddWallsAroundTile(x, y);
                }
            }
        }
    }

    private void AddWallsAroundTile(int x, int y)
    {
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                if (Mathf.Abs(offsetX) + Mathf.Abs(offsetY) != 1) continue;

                int checkX = x + offsetX;
                int checkY = y + offsetY;

                if (checkX < 0 || checkX >= width || checkY < 0 || checkY >= height)
                {
                    wallTilemap.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), wallTiles[Random.Range(0, wallTiles.Length)]);
                }
                else if (floorTilemap.GetTile(new Vector3Int(checkX, checkY, 0)) == null)
                {                    wallTilemap.SetTile(new Vector3Int(checkX, checkY, 0), wallTiles[Random.Range(0, wallTiles.Length)]);
                }
            }
        }
    }

    private void AddTilemapCollidersToWalls()
    {
        int tilesPerWall = CountTilesInWall();

        if (tilesPerWall > 1000)
        {
            SplitWallTilemap();
        }

        wallTilemap.gameObject.AddComponent<TilemapCollider2D>();
    }

    private void SplitWallTilemap()
    {
        List<Tilemap> wallTilemaps = new List<Tilemap>();
        wallTilemaps.Add(wallTilemap);

        int numSplits = Mathf.CeilToInt((float)CountTilesInWall() / 1000);

        for (int i = 1; i < numSplits; i++)
        {
            GameObject newTilemapObject = new GameObject("WallTilemap" + (i + 1));
            newTilemapObject.transform.SetParent(wallTilemap.transform.parent);
            newTilemapObject.transform.localPosition = Vector3.zero;
            newTilemapObject.transform.localRotation = Quaternion.identity;

            Tilemap newTilemap = newTilemapObject.AddComponent<Tilemap>();
            newTilemap.GetComponent<TilemapRenderer>().material = wallTilemap.GetComponent<TilemapRenderer>().material;
            newTilemap.GetComponent<TilemapRenderer>().sortingLayerID = wallTilemap.GetComponent<TilemapRenderer>().sortingLayerID;
            newTilemap.GetComponent<TilemapRenderer>().sortingOrder = wallTilemap.GetComponent<TilemapRenderer>().sortingOrder;

            wallTilemaps.Add(newTilemap);
        }
    }

    private int CountTilesInWall()
    {
        int count = 0;
        BoundsInt bounds = wallTilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (wallTilemap.HasTile(pos))
            {
                count++;
            }
        }
        return count;
    }

    private void PositionPlayerOnFloorTile()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                if (floorTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    Vector3Int playerPosition = new Vector3Int(x, y + 1, 0);
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    player.transform.position = floorTilemap.GetCellCenterWorld(playerPosition);
                    return;
                }
            }
        }
    }
}