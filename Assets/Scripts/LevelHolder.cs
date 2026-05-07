using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LevelHolder : MonoBehaviour
{
    // Purpose of this script to put gameinternal reference to the buttonCat
    [SerializeField] private RectTransform level;

    public void ClearButtonList()
    {

        for (int i = 0; i <= level.gameObject.GetComponentCount(); i++)
        {
            Destroy(level.GetChild(i));
        }


    }
}