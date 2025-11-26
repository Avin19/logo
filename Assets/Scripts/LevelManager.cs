using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Manager manager;


    private List<ItemDetail> items = new List<ItemDetail>();


    public string Name { get; set; }


    public event Action<List<ItemDetail>> OnItemsUpdated;




    public void SetItemdetails(List<ItemDetail> itemDetails)
    {
        SetItemdetails(itemDetails, Name);
    }


    public void SetItemdetails(List<ItemDetail> itemDetails, string categoryName)
    {
        if (!string.IsNullOrEmpty(categoryName))
            Name = categoryName;

        if (itemDetails == null)
        {
            items = new List<ItemDetail>();
        }
        else
        {
            // make a defensive copy to avoid external mutation
            items = new List<ItemDetail>(itemDetails);
        }

        // Fire event with a read-only copy
        OnItemsUpdated?.Invoke(new List<ItemDetail>(items));

        // Only start game if we have items
        if (items != null && items.Count > 0)
        {
            if (manager != null)
            {
                manager.StartGame();
            }
            else
            {
                Debug.LogWarning("[LevelManager] Manager reference is not assigned. Items updated but cannot call StartGame().");
            }
        }
        else
        {
            Debug.Log("[LevelManager] Items set but empty — not calling Manager.StartGame().");
        }
    }


    public void ClearItems()
    {
        items.Clear();
        OnItemsUpdated?.Invoke(new List<ItemDetail>(items));
    }

    /// <summary>
    /// Returns a defensive copy of the current items list.
    /// </summary>
    public List<ItemDetail> GetItems()
    {
        return new List<ItemDetail>(items);
    }


    public IReadOnlyList<ItemDetail> GetItemsReadOnly()
    {
        return items.AsReadOnly();
    }


    public ItemDetail GetItemByName(string manufacturer)
    {
        if (string.IsNullOrEmpty(manufacturer)) return null;
        return items.FirstOrDefault(i => string.Equals(i.Manufacturer, manufacturer, StringComparison.OrdinalIgnoreCase));
    }


    public bool TryGetItemByName(string manufacturer, out ItemDetail result)
    {
        result = GetItemByName(manufacturer);
        return result != null;
    }


    public ItemDetail GetItemAt(int index)
    {
        if (items == null || index < 0 || index >= items.Count) return null;
        return items[index];
    }


    public int Count => items?.Count ?? 0;
}
