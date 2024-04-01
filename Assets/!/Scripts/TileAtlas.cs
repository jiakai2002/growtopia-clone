using UnityEngine;

[CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    public TileClass dirt;
    public TileClass lava;
    public TileClass bedrock;
    public TileClass rock;
    public TileClass whiteDoor;
    public TileClass caveBackground;
    public TileClass woodenBlock;
    public TileClass woodenBackground;
}
