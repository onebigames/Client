using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SlotItem : UIElement
{
    [SerializeField] private int id = -1;
    private Image itemIcon = null;

    private CharacterInventory inventory = null;
    private Container container = null;

    private new void Start()
    {
        base.Start();
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
    }

    private void Update()
    {
        if (!inventory)
        {
            inventory = FindObjectOfType<CharacterInventory>();
            container = inventory ? inventory.GetSlotById(id) : null;
            return;
        }
    }

    private void UseSlot()
    {
        return;
    }

    public new void Show()
    {
        base.Show();
        if (container != null && container.GetContained().Count > 0)
        {
            itemIcon.enabled = true;
        }
    }

    public new void Hide()
    {
        base.Hide();
        itemIcon.enabled = false;
    }

    private void InteractItemInHandAndSlot() { }

    private void RemoveFromSlot()
    {
        Containable containable = container.GetContained()[0];
        containable.Container = inventory.GetActiveHand();
        itemIcon.enabled = false;
    }

    private void AddToSlot()
    {
        Containable containable = inventory.GetInHand();
        containable.Container = container;
        itemIcon.sprite = Instantiate(containable.generatedIcon);
        itemIcon.transform.SetAsLastSibling();
        itemIcon.enabled = true;
    }

    public void ClickButton()
    {
        bool itemInHand = inventory.GetInHand() != null;
        bool itemInSlot = container.GetContained().Count != 0;
        if (itemInHand && itemInSlot)
        {
            InteractItemInHandAndSlot();
            return;
        }
        else if (itemInHand && !itemInSlot)
        {
            AddToSlot();
            return;
        }
        else if (!itemInHand && itemInSlot)
        {
            RemoveFromSlot();
            return;
        }
    }

}
