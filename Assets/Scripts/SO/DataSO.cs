using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataSO", fileName = "DataSO")]

public class DataSO : ScriptableObject
{
    public List<ItemDetail> itemDetails = new List<ItemDetail>();

}


