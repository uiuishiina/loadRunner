using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager_script : MonoBehaviour
{
    [SerializeField, Header("使用するblockマップ")] public Tilemap useblocktile;
    [SerializeField, Header("使用するmoveマップ")] public Tilemap usemovetile;
    //[SerializeField, Header("使用するblockマップ")] public Tilemap useblocktile;
    [SerializeField, Header("タイルの種類")] public TileData data;
}