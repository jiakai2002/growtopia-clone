using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;

    [Header("Generation Settings")]
    public Tilemap tileMap;
    public Tilemap tileMapBg;
    public int worldWidth = 100;
    public int worldHeight = 60;
    public int rockChance = 4;
    public int lavaChance = 60;
    public GameObject player;
    public int range = 2;
    public Sprite[] cracks;
    public GameObject tileDrop;
    public GameObject world;
    public Inventory inventory;
    private float doorPos;
    public GameObject dustEffect;
    public float dropOffset;

    [HideInInspector]
    public Dictionary<Vector3Int, TileClass> WorldTileClasses = new Dictionary<Vector3Int, TileClass>();
    public Dictionary<Vector3Int, float[]> WorldTileBreakData = new Dictionary<Vector3Int, float[]>();
    public Dictionary<Vector3Int, GameObject> WorldTileCracks = new Dictionary<Vector3Int, GameObject>();

    public Dictionary<Vector3Int, TileClass> BackgroundTileClasses = new Dictionary<Vector3Int, TileClass>();
    public Dictionary<Vector3Int, float[]> BackgroundTileBreakData = new Dictionary<Vector3Int, float[]>();
    public Dictionary<Vector3Int, GameObject> BackgroundTileCracks = new Dictionary<Vector3Int, GameObject>();

    public void Start()
    {
        GenerateTerrain();
        player.transform.position = new Vector3(doorPos + 0.5f, 36.5f, 0);
        dropOffset = 0.2f;
    }

    public void Update()
    {
        TileRepair();
    }
    public void GenerateTerrain()
    {
        doorPos = Random.Range(25, 75);
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (y > 6 && y < 36)
                {
                    GenerateTile(x, y, tileAtlas.caveBackground, tileMapBg);
                }
                if (y < 6)
                {
                    GenerateTile(x, y, tileAtlas.bedrock, tileMap);
                }
                else if (y < 8)
                {
                    if (Random.value < lavaChance / 100f)
                    {
                        GenerateTile(x, y, tileAtlas.lava, tileMap);
                    }

                    else
                    {
                        GenerateTile(x, y, tileAtlas.dirt, tileMap);
                    }
                }
                else if (y < 10)
                {
                    if (Random.value < 0.4 * lavaChance / 100f)
                        GenerateTile(x, y, tileAtlas.lava, tileMap);
                    else
                        GenerateTile(x, y, tileAtlas.dirt, tileMap);
                }
                else if (y < 30)
                {
                    if (Random.value < (rockChance / 100f))
                        GenerateTile(x, y, tileAtlas.rock, tileMap);
                    else
                        GenerateTile(x, y, tileAtlas.dirt, tileMap);
                }
                else if (y < 36)
                {
                    if (y == 35 && x == doorPos)
                    {
                        GenerateTile(x, y, tileAtlas.bedrock, tileMap);
                        GenerateTile(x, y + 1, tileAtlas.whiteDoor, tileMap);
                    }
                    else
                    {
                        GenerateTile(x, y, tileAtlas.dirt, tileMap);
                    }
                }
            }
        }
    }
    private void GenerateTile(int x, int y, TileClass block, Tilemap tileMap)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        float[] tileData = new float[] { block.hardness, 0f };
        if (block.inBackground == false)
        {
            tileMap.SetTile(pos, block.ruleTile);
            WorldTileClasses.Add(pos, block);
            WorldTileBreakData.Add(pos, tileData);
        }
        else
        {
            tileMapBg.SetTile(pos, block.ruleTile);
            BackgroundTileClasses.Add(pos, block);
            BackgroundTileBreakData.Add(pos, tileData);
        }
    }
    public void TileRepair()
    {
        foreach (var item in WorldTileBreakData)
        {
            var tileBreakData = item.Value;
            var tilePos = item.Key;
            TileClass block = WorldTileClasses[tilePos];
            if (tileBreakData[1] > 0)
            {
                tileBreakData[1] -= Time.deltaTime;
                if (tileBreakData[1] < 0)
                {
                    Destroy(WorldTileCracks[tilePos]);
                    WorldTileCracks.Remove(tilePos);
                    tileBreakData[0] = block.hardness;
                    tileBreakData[1] = 0;
                }
            }
        }
        foreach (var item in BackgroundTileBreakData)
        {
            var tileBreakData = item.Value;
            var tilePos = item.Key;
            TileClass block = BackgroundTileClasses[tilePos];
            if (tileBreakData[1] > 0)
            {
                tileBreakData[1] -= Time.deltaTime;
                if (tileBreakData[1] < 0)
                {
                    Destroy(BackgroundTileCracks[tilePos]);
                    BackgroundTileCracks.Remove(tilePos);
                    tileBreakData[0] = block.hardness;
                    tileBreakData[1] = 0;
                }
            }
        }
    }
    bool isInRange(Vector3Int playerPos, Vector3Int tilePos)
    {
        BoxCollider2D box = player.GetComponent<BoxCollider2D>();
        float playerWidth = box.size.x / 2;
        float playerHeight = box.size.y / 2;
        int playerRight = (int)(playerPos.x + playerWidth);
        int playerLeft = (int)(playerPos.x - playerWidth);
        int playerTop = (int)(playerPos.x + playerHeight);
        int playerBot = (int)(playerPos.x - playerHeight);
        if (tilePos.x >= playerLeft - range && tilePos.x <= playerRight + range)
        {
            if (tilePos.y >= playerPos.y - range && tilePos.y <= playerPos.y + range)
            {
                return true;
            }
        }
        return false;
    }
    public void newCrack(Vector3Int tilePos, Dictionary<Vector3Int, GameObject> tileCracks)
    {
        GameObject crack = new GameObject();
        crack.transform.position = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, tilePos.z);
        crack.AddComponent<SpriteRenderer>();
        if (WorldTileClasses.ContainsKey(tilePos))
        {
            crack.transform.parent = tileMap.transform;
            crack.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder + 1;
        }
        else
        {
            crack.transform.parent = tileMapBg.transform;
            crack.GetComponent<SpriteRenderer>().sortingOrder = tileMapBg.GetComponent<TilemapRenderer>().sortingOrder + 1;
        }
        tileCracks[tilePos] = crack;

    }
    public void hitTile(Vector3 mousePos, Vector3 playerWorldPos)
    {
        Vector3Int tilePos = tileMap.WorldToCell(mousePos);
        Vector3Int playerPos = tileMap.WorldToCell(playerWorldPos);
        if (isInRange(playerPos, tilePos))
        {
            if (WorldTileClasses.ContainsKey(tilePos))
            {
                TileClass block = WorldTileClasses[tilePos];
                var tileBreakData = WorldTileBreakData[tilePos];
                if (block.isBreakable)
                {
                    if (!WorldTileCracks.ContainsKey(tilePos))
                        newCrack(tilePos, WorldTileCracks);
                    tileBreakData[1] = 6;
                    tileBreakData[0] = System.Math.Max(0, tileBreakData[0] - 1);
                    GameObject crack = WorldTileCracks[tilePos];
                    UpdateCrack(tilePos, crack, block, tileBreakData);
                    GameObject dust = Instantiate(dustEffect, new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, 0), Quaternion.identity, tileMap.transform);
                    Destroy(dust, dust.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                }
            }
            else if (BackgroundTileClasses.ContainsKey(tilePos))
            {
                TileClass block = BackgroundTileClasses[tilePos];
                var tileBreakData = BackgroundTileBreakData[tilePos];
                if (block.isBreakable)
                {
                    if (!BackgroundTileCracks.ContainsKey(tilePos))
                        newCrack(tilePos, BackgroundTileCracks);
                    tileBreakData[1] = 6;
                    tileBreakData[0] = System.Math.Max(0, tileBreakData[0] - 1);
                    GameObject crack = BackgroundTileCracks[tilePos];
                    UpdateCrack(tilePos, crack, block, tileBreakData);
                    GameObject dust = Instantiate(dustEffect, new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, 0), Quaternion.identity, tileMapBg.transform);
                    Destroy(dust, dust.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                }
            }
        }
    }

    public void addTile(Vector3 mousePos, Vector3 playerWorldPos, TileClass selectedTile)
    {
        if (selectedTile.inBackground == false)
        {
            Vector3Int tilePos = tileMap.WorldToCell(mousePos);
            Vector3Int playerPos = tileMap.WorldToCell(playerWorldPos);
            if (isInRange(playerPos, tilePos) && tileMap.GetTile(tilePos) == null)
            {
                GenerateTile(tilePos.x, tilePos.y, selectedTile, tileMap);
                inventory.Remove(selectedTile, 1);
            }
        }
        else
        {
            Vector3Int tilePos = tileMapBg.WorldToCell(mousePos);
            Vector3Int playerPos = tileMapBg.WorldToCell(playerWorldPos);
            if (isInRange(playerPos, tilePos) && tileMapBg.GetTile(tilePos) != selectedTile.ruleTile)
            {
                GenerateTile(tilePos.x, tilePos.y, selectedTile, tileMapBg);
                inventory.Remove(selectedTile, 1);
            }
        }
    }
    void UpdateCrack(Vector3Int tilePos, GameObject crack, TileClass block, float[] tileBreakData)
    {
        if (tileBreakData[0] == 0)
        {
            removeTile(tilePos);
        }
        else if (tileBreakData[0] / block.hardness >= 0.75)
        {
            crack.GetComponent<SpriteRenderer>().sprite = cracks[0];
        }
        else if (tileBreakData[0] / block.hardness >= 0.50)
        {
            crack.GetComponent<SpriteRenderer>().sprite = cracks[1];
        }
        else if (tileBreakData[0] / block.hardness >= 0.25)
        {
            crack.GetComponent<SpriteRenderer>().sprite = cracks[2];
        }
        else if (tileBreakData[0] / block.hardness < 0.25)
        {
            crack.GetComponent<SpriteRenderer>().sprite = cracks[3];
        }
    }
    void removeTile(Vector3Int tilePos)
    {
        if (WorldTileClasses.ContainsKey(tilePos))
        {
            Destroy(WorldTileCracks[tilePos]);
            tileMap.SetTile(tilePos, null);
            float offset = Random.Range(-1f * dropOffset, dropOffset);
            GameObject newTileDrop = Instantiate(tileDrop, new Vector2(tilePos.x + 0.5f, tilePos.y + 0.5f + offset), Quaternion.identity);
            newTileDrop.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = WorldTileClasses[tilePos].tileDrop;
            newTileDrop.GetComponent<TileDropController>().tileDropClass = WorldTileClasses[tilePos];

            WorldTileCracks.Remove(tilePos);
            WorldTileClasses.Remove(tilePos);
            WorldTileBreakData.Remove(tilePos);
        }
        else if (BackgroundTileClasses.ContainsKey(tilePos))
        {
            Destroy(BackgroundTileCracks[tilePos]);
            tileMapBg.SetTile(tilePos, null);
            float offset = Random.Range(-1f * dropOffset, dropOffset);
            GameObject newTileDrop = Instantiate(tileDrop, new Vector2(tilePos.x + 0.5f, tilePos.y + 0.5f + offset), Quaternion.identity);
            newTileDrop.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = BackgroundTileClasses[tilePos].tileDrop;
            newTileDrop.GetComponent<TileDropController>().tileDropClass = BackgroundTileClasses[tilePos];

            BackgroundTileCracks.Remove(tilePos);
            BackgroundTileClasses.Remove(tilePos);
            BackgroundTileBreakData.Remove(tilePos);
        }

    }
}