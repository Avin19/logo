using System.Collections.Generic;
using UnityEngine;



public class LevelManager : MonoBehaviour
{
    [SerializeField] private Manager manager;

    private List<ItemDetail> items = new List<ItemDetail>();

    public void SetItemdetails(List<ItemDetail> itemDetails)
    {
        items = itemDetails;
        Debug.Log("Item is passed");
        manager.StartGame();

    }
    public List<ItemDetail> GetItems()
    {
        return items;
    }
    public void SetupGame()
    {

    }


}



