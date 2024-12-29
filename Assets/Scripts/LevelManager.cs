using System.Collections.Generic;
using UnityEngine;



public class LevelManager : MonoBehaviour
{
    [SerializeField] private Manager manager;

    private List<ItemDetail> items = new List<ItemDetail>();
    public string Name { get; set; }


    public void SetItemdetails(List<ItemDetail> itemDetails)
    {
        items = itemDetails;
        manager.StartGame();

    }
    public List<ItemDetail> GetItems()
    {
        return items;
    }




}



