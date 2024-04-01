using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;
    public RuleTile ruleTile;
    public bool inBackground = false;
    public bool isBreakable = true;
    public int hardness = 3;
    public Sprite tileDrop;
}