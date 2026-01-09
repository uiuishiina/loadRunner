using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager_script : MonoBehaviour
{
    [SerializeField, Header("使用するblockマップ")] public Tilemap useblocktile;
    [SerializeField, Header("使用するmoveマップ")] public Tilemap usemovetile;
    [SerializeField, Header("使用するGoolマップ")] public Tilemap useGooltile;
    [SerializeField, Header("使用するGoolマップ")] public GameObject useGooltileObject;
    public bool goolobj_;

    [SerializeField, Header("タイルの種類")] public TileData data;

    [SerializeField, Header("Player")] public Player_Move Player_;
    [SerializeField, Header("Gold")] public int GoldCount_;

    private void Start()
    {
        goolobj_ = false;
        CreateGool(goolobj_);
    }

    public void GoldGet()
    {
        var g = Player_.GetGold;
        if (g == GoldCount_)
        {
            goolobj_ = true;
            CreateGool(goolobj_);
        }
    }

    void CreateGool(bool ans)
    {
        useGooltileObject.SetActive(ans);
    }

    public void Result(bool ans)
    {
        if (ans) { GameClear(); }
        else{ GameOver(); }
    }

    void GameClear()
    {
        SceneManager.LoadScene("ClearScene");
    }
    void GameOver()
    {
        SceneManager.LoadScene("SampleScene");
    }
}