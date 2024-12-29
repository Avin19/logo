using System.Collections.Generic;
using UnityEngine;



public class LevelManager : MonoBehaviour
{
    [SerializeField] private Manager manager;
    [SerializeField] private GameInternal gameInternal;

    private List<ItemDetail> items = new List<ItemDetail>();

    public void SetItemdetails(List<ItemDetail> itemDetails)
    {
        items.Clear();
        items = itemDetails;
        manager.StartGame();

    }
    public List<ItemDetail> GetItems()
    {
        return items;
    }
    public void SetupGame()
    {
        gameInternal.Restart();
    }


}



