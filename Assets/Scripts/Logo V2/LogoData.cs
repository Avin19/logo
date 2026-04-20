using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LogoData", menuName = "Logov2/LogoData", order = 0)]
public class LogoData : ScriptableObject
{

    public Sprite logoSprite;
    public string logoName;
    public List<bool> reavealArray = new List<bool>();
    public List<bool> filedArray = new List<bool>();

}
[CreateAssetMenu(fileName = "CatergoryData", menuName = "Logov2/Catergory", order = 1)]
public class CategoryData : ScriptableObject
{
    public LogoData[] itemList;
    public string catergoryName;
    public DiffcultLevel diffcultLevel;

}
[CreateAssetMenu(fileName = "DB", menuName = "Logov2/DB", order = 2)]
public class DataBase : ScriptableObject
{
    public CategoryData[] categoryDatasList;


}
public enum DiffcultLevel
{
    Easy,
    Medium,
    Hard
}