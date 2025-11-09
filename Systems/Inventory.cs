using System.Collections.Generic;

namespace SuperOtto.Systems;

/// <summary>
/// Represents an item in the inventory.
/// </summary>
public class Item
{
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public int Quantity { get; set; }

    public Item(string name, ItemType type, int quantity = 1)
    {
        Name = name;
        Type = type;
        Quantity = quantity;
    }
}

public enum ItemType
{
    Seed,
    Crop,
    Tool
}

/// <summary>
/// Manages the player's inventory.
/// </summary>
public class Inventory
{
    private List<Item> _items = new List<Item>();
    public int SelectedSlot { get; set; } = 0;
    public const int MaxSlots = 10;

    public Inventory()
    {
        // Start with some basic items
        AddItem(new Item("Wheat Seeds", ItemType.Seed, 10));
        AddItem(new Item("Corn Seeds", ItemType.Seed, 5));
        AddItem(new Item("Hoe", ItemType.Tool, 1));
        AddItem(new Item("Watering Can", ItemType.Tool, 1));
    }

    public void AddItem(Item item)
    {
        // Try to stack with existing item
        var existingItem = _items.Find(i => i.Name == item.Name && i.Type == item.Type);
        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else if (_items.Count < MaxSlots)
        {
            _items.Add(item);
        }
    }

    public bool RemoveItem(string name, int quantity = 1)
    {
        var item = _items.Find(i => i.Name == name);
        if (item != null && item.Quantity >= quantity)
        {
            item.Quantity -= quantity;
            if (item.Quantity <= 0)
                _items.Remove(item);
            return true;
        }
        return false;
    }

    public Item GetSelectedItem()
    {
        if (SelectedSlot >= 0 && SelectedSlot < _items.Count)
            return _items[SelectedSlot];
        return null;
    }

    public List<Item> GetItems() => _items;

    public bool HasItem(string name, int quantity = 1)
    {
        var item = _items.Find(i => i.Name == name);
        return item != null && item.Quantity >= quantity;
    }
}
