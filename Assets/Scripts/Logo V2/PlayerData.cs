using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    // Start is called before the first frame update
    public string playerID = null;
    public int coin;
    public int level;
    public bool isleaderboard;
}
