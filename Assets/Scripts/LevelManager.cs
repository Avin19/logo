using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Manager manager;

    // internal storage for items
    private List<ItemDetail> items = new List<ItemDetail>();

    // Public name property (sheet/category name)
    public string Name { get; set; }

    /// <summary>
    /// Event fired when items are updated. Subscribers receive an immutable copy of the list.
    /// </summary>
    public event Action<List<ItemDetail>> OnItemsUpdated;

    private void Awake()
    {
        // Auto-assign manager if not set (reduces inspector setup mistakes)

    }

    /// <summary>
    /// Replaces current items with the provided list (defensive copy), fires event and triggers manager.
    /// Backwards-compatible signature.
    /// </summary>
    public void SetItemdetails(List<ItemDetail> itemDetails)
    {
        SetItemdetails(itemDetails, Name);
    }

    /// <summary>
    /// Replaces current items and optionally sets the category name.
    /// </summary>
    /// <param name="itemDetails">List of ItemDetail (can be null)</param>
    /// <param name="categoryName">Optional category name to set (e.g. "Cars" or "Countries")</param>
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

    /// <summary>
    /// Clears current items and notifies subscribers.
    /// </summary>
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

    /// <summary>
    /// Returns a read-only view of items.
    /// </summary>
    public IReadOnlyList<ItemDetail> GetItemsReadOnly()
    {
        return items.AsReadOnly();
    }

    /// <summary>
    /// Convenience lookup by Manufacturer name (case-insensitive). Returns null if not found.
    /// </summary>
    public ItemDetail GetItemByName(string manufacturer)
    {
        if (string.IsNullOrEmpty(manufacturer)) return null;
        return items.FirstOrDefault(i => string.Equals(i.Manufacturer, manufacturer, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Try-get pattern for item lookup.
    /// </summary>
    public bool TryGetItemByName(string manufacturer, out ItemDetail result)
    {
        result = GetItemByName(manufacturer);
        return result != null;
    }

    /// <summary>
    /// Safe index access.
    /// </summary>
    public ItemDetail GetItemAt(int index)
    {
        if (items == null || index < 0 || index >= items.Count) return null;
        return items[index];
    }

    /// <summary>
    /// Number of items currently loaded.
    /// </summary>
    public int Count => items?.Count ?? 0;
}
