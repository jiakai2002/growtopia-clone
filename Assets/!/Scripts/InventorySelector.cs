using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySelector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    public Inventory inventory;
    public Player playerController;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerPressRaycast.gameObject;
        Vector3 pos = obj.GetComponent<Transform>().parent.localPosition;
        int x = (int)(pos.x - (-253)) / 55;
        int y = (int)(pos.y - (-98)) / 55;
        if (inventory.inventorySelect.GetComponent<Transform>().localPosition != pos)
        {
            if (x >= 0 && y >= 0)
            {
                if (x == 0 && y == 4)
                {
                    selectFist();
                }
                else if (x == 1 && y == 4)
                {
                    selectWrench();
                }
                else
                {
                    selectItem(x, y);
                }
            }
        }
    }
    public void selectItem(int x, int y)
    {
        inventory.inventorySelect.transform.localPosition = inventory.uiSlots[x, y].transform.localPosition - inventory.inventorySelect.transform.parent.localPosition;
        inventory.inventorySelect.SetActive(true);
        inventory.hotbarSelect.SetActive(false);
        playerController.fistSelected = false;
        playerController.wrenchSelected = false;
        playerController.selectedTile = inventory.inventorySlots[x, y].block;
        updateHotbar(inventory.inventorySlots[x, y]);
    }
    public void selectFist()
    {
        inventory.hotbarUISlots[0].transform.GetChild(0).GetComponent<Image>().sprite = inventory.fistSprite;
        inventory.inventorySelect.transform.localPosition = new Vector3(0, 0, 0);
        inventory.hotbarSelect.transform.localPosition = new Vector3(0, 0, 0);
        inventory.inventorySelect.SetActive(true);
        inventory.hotbarSelect.SetActive(true);
        playerController.wrenchSelected = false;
        playerController.fistSelected = true;
        playerController.selectedTile = null;
    }
    public void selectWrench()
    {
        inventory.hotbarUISlots[0].transform.GetChild(0).GetComponent<Image>().sprite = inventory.wrenchSprite;
        inventory.inventorySelect.transform.localPosition = inventory.uiSlots[1, 4].transform.localPosition - inventory.inventorySelect.transform.parent.localPosition;
        inventory.inventorySelect.SetActive(true);
        inventory.hotbarSelect.SetActive(true);
        playerController.wrenchSelected = true;
        playerController.fistSelected = false;
        playerController.selectedTile = null;
    }
    public void updateHotbar(InventorySlot inventorySlot)
    {
        inventory.AddHotbar(inventorySlot);
        inventory.hotbarSelect.SetActive(true);
    }
}
