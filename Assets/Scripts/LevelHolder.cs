using System.Collections.Generic;
using UnityEngine;



public class LevelHolder : MonoBehaviour
{
    // Purpose of this script to put gameinternal reference to the buttonCat
    [SerializeField] private GameInternal gameInternal;
    [SerializeField] private List<ButtonCat> buttonCats;

    public void SetButtonListItem(ButtonCat _buttonCat)
    {
        buttonCats.Add(_buttonCat);
    }
    public void ClearButtonList()
    {
        buttonCats.Clear();
    }
}