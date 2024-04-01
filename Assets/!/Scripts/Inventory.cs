using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int inventorySize;
    public int inventoryWidth;
    public int inventoryHeight;
    private int stackLimit = 200;
    public Vector2 inventOffset;
    public Vector2 hotbarOffset;
    public Vector2 multiplier;
    public TileAtlas tileAtlas;
    public Sprite fistSprite;
    public Sprite wrenchSprite;
    public Sprite slotBox;
    public GameObject hotbarUI;
    public GameObject inventoryUI;
    public GameObject hotbarSelect;
    public GameObject inventorySelect;
    public GameObject inventorySlotPrefab;
    public GameObject hotbarSlotPrefab;
    public InventorySelector inventorySelector;

    [HideInInspector]
    public InventorySlot[,] inventorySlots;
    public GameObject[,] uiSlots;
    public InventorySlot[] hotbarSlots;
    public GameObject[] hotbarUISlots;

    public void Start()
    {
        inventoryWidth = 10;
        inventoryHeight = (inventorySize + (inventoryWidth - 1)) / inventoryWidth;
        inventorySlots = new InventorySlot[inventoryWidth, inventoryHeight];
        uiSlots = new GameObject[inventoryWidth, inventoryHeight];
        hotbarSlots = new InventorySlot[4];
        hotbarUISlots = new GameObject[4];
        SetupUI();
        UpdateInventoryUI();
        UpdateHotbarUI();
        Add(tileAtlas.dirt, 10);
        Add(tileAtlas.lava, 10);
        Add(tileAtlas.rock, 10);
        Add(tileAtlas.woodenBackground, 10);
        Add(tileAtlas.woodenBlock, 10);
    }
    void SetupUI()
    {
        for (int x = 0; x < 4; x++)
        {
            GameObject hotbarSlot = Instantiate(hotbarSlotPrefab, hotbarUI.transform);
            hotbarSlot.GetComponent<RectTransform>().localPosition = new Vector3(hotbarOffset.x + multiplier.x * x, hotbarOffset.y);
            hotbarUISlots[x] = hotbarSlot;
            hotbarSlots[x] = null;
        }
        hotbarUISlots[0].transform.GetChild(0).GetComponent<Image>().sprite = fistSprite;
        hotbarUISlots[0].transform.GetChild(0).GetComponent<Image>().enabled = true;
        hotbarUISlots[0].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = null;
        hotbarSelect = Instantiate(hotbarSelect, hotbarUI.transform);
        hotbarSelect.transform.localPosition = hotbarUISlots[0].transform.localPosition;
        hotbarSelect.transform.SetParent(hotbarUISlots[0].transform);

        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {
                {
                    GameObject inventorySlot = Instantiate(inventorySlotPrefab, inventoryUI.transform);
                    inventorySlot.GetComponent<RectTransform>().localPosition = new Vector3(inventOffset.x + multiplier.x * x, inventOffset.y + multiplier.y * y);
                    uiSlots[x, y] = inventorySlot;
                    inventorySlots[x, y] = null;
                }
            }
        }
        uiSlots[0, inventoryHeight - 1].transform.GetChild(0).GetComponent<Image>().sprite = fistSprite;
        uiSlots[0, inventoryHeight - 1].transform.GetChild(0).GetComponent<Image>().enabled = true;
        uiSlots[0, inventoryHeight - 1].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = null;
        uiSlots[1, inventoryHeight - 1].transform.GetChild(0).GetComponent<Image>().sprite = wrenchSprite;
        uiSlots[1, inventoryHeight - 1].transform.GetChild(0).GetComponent<Image>().enabled = true;
        uiSlots[1, inventoryHeight - 1].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = null;
        inventorySelect = Instantiate(inventorySelect, inventoryUI.transform);
        inventorySelect.transform.localPosition = uiSlots[0, inventoryHeight - 1].transform.localPosition;
        inventorySelect.transform.SetParent(uiSlots[0, inventoryHeight - 1].transform);
    }
    void UpdateHotbarUI()
    {
        for (int x = 1; x < 4; x++)
        {
            if (hotbarSlots[x] == null)
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = false;
                hotbarUISlots[x].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = null;
            }
            else
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = hotbarSlots[x].block.tileDrop;
                hotbarUISlots[x].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = hotbarSlots[x].quantity.ToString();
            }
        }
    }
    void UpdateInventoryUI()
    {
        for (int y = 0; y < inventoryHeight; y++)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (!((y == inventoryHeight - 1 && x == 0) || (y == inventoryHeight - 1 && x == 1)))
                {
                    if (inventorySlots[x, y] == null)
                    {
                        uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = null;
                        uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = false;
                        uiSlots[x, y].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = null;
                    }
                    else
                    {
                        uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = true;
                        uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, y].block.tileDrop;
                        uiSlots[x, y].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = inventorySlots[x, y].quantity.ToString();
                    }
                }
            }
        }
    }
    public void Add(TileClass block, int n)
    {
        bool added = false;
        var pos = Contains(block);
        if (pos != Vector2.zero)
        {
            if (inventorySlots[pos.x, pos.y].quantity + 1 <= stackLimit)
                inventorySlots[pos.x, pos.y].quantity++;
        }
        else
        {
            for (int y = inventoryHeight - 1; y >= 0; y--)
            {
                if (added)
                    break;
                for (int x = 0; x < inventoryWidth; x++)
                {
                    if (!((x == 0 && y == inventoryHeight - 1) || (x == 1 && y == inventoryHeight - 1)))
                    {
                        if (inventorySlots[x, y] == null)
                        {
                            inventorySlots[x, y] = new InventorySlot { block = block, position = new Vector2Int(x, y), quantity = n };
                            added = true;
                            break;
                        }
                        else if (inventorySlots[x, y].block == block)
                        {
                            inventorySlots[x, y].quantity = Math.Min(stackLimit, inventorySlots[x, y].quantity + n);
                        }
                    }
                }
            }
        }
        UpdateInventoryUI();
        UpdateHotbarUI();
    }
    public Vector2Int Contains(TileClass block)
    {
        for (int y = 0; y < inventoryHeight; y++)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] != null && inventorySlots[x, y].block == block)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return Vector2Int.zero;
    }
    public void Remove(TileClass block, int n)
    {
        for (int y = 0; y < inventoryHeight; y++)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] != null && inventorySlots[x, y].block == block)
                {
                    inventorySlots[x, y].quantity -= n;
                    if (inventorySlots[x, y].quantity == 0)
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            if (hotbarSlots[i] == inventorySlots[x, y])
                            {
                                hotbarSlots[i] = null;
                                Debug.Log("set hotbar null" + i);
                            }
                        }
                        inventorySlots[x, y] = null;
                        shiftInventoryFrom(x, y);
                        inventorySelector.selectFist();
                    }
                }
            }
        }
        UpdateInventoryUI();
        UpdateHotbarUI();
    }
    public void shiftInventoryFrom(int i, int j)
    {
        for (int y = i; y < inventoryHeight - 1; y++)
        {
            for (int x = j; x < inventoryWidth - 1; x++)
            {
                if (x == inventoryWidth - 1)
                {
                    inventorySlots[x, y] = inventorySlots[0, y + 1];
                    Debug.Log("shift left edge");
                }
                else
                {
                    inventorySlots[x, y] = inventorySlots[x + 1, y];
                    Debug.Log("shift left");
                }
            }
        }
    }
    public void AddHotbar(InventorySlot inventorySlot)
    {
        hotbarSelect.transform.localPosition = hotbarUISlots[3].transform.localPosition - hotbarSelect.transform.parent.localPosition;
        bool added = false;
        for (int x = 1; x < 4; x++)
        {
            if (hotbarSlots[x] == null)
            {
                hotbarSlots[x] = inventorySlot;
                hotbarSelect.transform.localPosition = hotbarUISlots[x].transform.localPosition - hotbarSelect.transform.parent.localPosition;
                added = true;
                break;
            }
            else if (hotbarSlots[x] == inventorySlot)
            {
                hotbarSlots[x] = hotbarSlots[3];
                hotbarSlots[3] = inventorySlot;
                added = true;
                break;
            }
        }
        if (added == false)
        {
            for (int x = 1; x < 3; x++)
            {
                hotbarSlots[x] = hotbarSlots[x + 1];
            }
            hotbarSlots[3] = inventorySlot;
        }
        UpdateHotbarUI();
    }
}