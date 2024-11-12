using System.Collections.Generic;
using UnityEngine;



public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform gamePanel;
    [SerializeField] private DataSO dataSO;

    private List<ItemDetail> items = new List<ItemDetail>();

    public void SwtichToGame()
    {
        gameObject.SetActive(true);
    }



}



