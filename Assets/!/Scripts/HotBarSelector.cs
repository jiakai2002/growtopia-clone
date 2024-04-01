using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotBarSelector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public Inventory inventory;
    public Player playerController;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerPressRaycast.gameObject;
        Vector3 pos = obj.GetComponent<Transform>().parent.localPosition;
        var name = obj.GetComponent<Transform>().name;
        int x = (int)(pos.x - (-85)) / 55;
        int y = (int)(pos.y / 55);
        if (x == 0)
        {
            if (playerController.fistSelected)
            {
                selectWrench();
            }
            else
            {
                selectFist();
            }
        }
        else
        {
            selectItem(x, y);
        }
    }
    public void selectItem(int x, int y)
    {
        inventory.hotbarSelect.transform.localPosition = inventory.hotbarUISlots[x].transform.localPosition - inventory.hotbarSelect.transform.parent.localPosition;
        inventory.hotbarSelect.SetActive(true);
        inventory.inventorySelect.SetActive(true);
        playerController.fistSelected = false;
        playerController.wrenchSelected = false;
        playerController.selectedTile = inventory.hotbarSlots[x].block;
        selectInInventory(inventory.hotbarSlots[x]);
    }
    public void selectFist()
    {
        inventory.hotbarUISlots[0].transform.GetChild(0).GetComponent<Image>().sprite = inventory.fistSprite;
        inventory.inventorySelect.transform.localPosition = new Vector3(0, 0, 0);
        inventory.hotbarSelect.transform.localPosition = new Vector3(0, 0, 0);
        inventory.inventorySelect.SetActive(true);
        inventory.hotbarSelect.SetActive(true);
        playerController.fistSelected = true;
        playerController.wrenchSelected = false;
        playerController.selectedTile = null;
    }
    public void selectWrench()
    {
        inventory.hotbarUISlots[0].transform.GetChild(0).GetComponent<Image>().sprite = inventory.wrenchSprite;
        inventory.inventorySelect.transform.localPosition = inventory.uiSlots[1, 4].transform.localPosition - inventory.inventorySelect.transform.parent.localPosition;
        inventory.inventorySelect.SetActive(true);
        inventory.hotbarSelect.SetActive(true);
        playerController.fistSelected = false;
        playerController.wrenchSelected = true;
        playerController.selectedTile = null;
    }
    public void selectInInventory(InventorySlot inventorySlot)
    {
        for (int x = 0; x < inventory.inventoryWidth; x++)
        {
            for (int y = 0; y < inventory.inventoryHeight; y++)
            {
                if (inventory.inventorySlots[x, y] != null && inventory.inventorySlots[x, y] == inventorySlot)
                    inventory.inventorySelect.transform.localPosition = inventory.uiSlots[x, y].transform.localPosition - inventory.inventorySelect.transform.parent.localPosition;
            }
        }
    }
}