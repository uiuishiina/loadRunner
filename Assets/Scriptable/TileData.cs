using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData",menuName = "Tile")]
public class TileData : ScriptableObject
{
    [SerializeField,Header("使用するタイル")]
    public Tile Brock0;
    public Tile Brock1;
    public Tile Brock2;
    public Tile Brock3;
    public Tile Brock4;
    public Tile Brock5;
    public Tile Brock6;
    public Tile Gold;
    public Tile Bar;
    public Tile Radder;
}
