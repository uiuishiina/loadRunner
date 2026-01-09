using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player_Move : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameManager_script gameManager;
    [SerializeField] float speed;
    [SerializeField] float FallSpeed;
    [SerializeField] float DigSpeed;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    Vector2 inputvec;
    bool CanMove = true;
    Vector3 movevec;
    public int GetGold { private set; get; } = 0;
    enum St
    {
        Idle,Move,Fall,Radder,Bar,Dig
    }
    St PlayerState = St.Idle;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //入力関数
    void OnMove(InputValue input)
    {
        if (CanMove) {
            PlayerState = St.Move;
            inputvec = input.Get<Vector2>();
            var t = CheckTile(inputvec);
            //移動できるかチェック
            if(inputvec.y != 0)
            {
                if (t == null || t.name == "radder")
                {
                    t = CheckTile(new Vector2());//プレイヤーのところに梯子があるか
                    if (t == null || t.name != "radder")
                    {
                        if(inputvec.y > 0)
                        {
                            return;
                        }
                    }
                    PlayerState = St.Radder;
                }
            }
            if (ChackMove(t))
            {
                movevec = new Vector3(player.transform.position.x + inputvec.x, player.transform.position.y + inputvec.y, player.transform.position.z);
                CanMove = false;
                if (inputvec.x > 0) {
                    spriteRenderer.flipX = false;
                }
                else if(inputvec.x < 0){
                    spriteRenderer.flipX = true;
                }
            }
        }
    }

    void OnZ(InputValue input)
    {
        Dig(new Vector2(-1,-1));
    }
    void OnC(InputValue input)
    {
        Dig(new Vector2(1, -1));
    }
    private void Update()
    {
        if (!CanMove) {
            if (PlayerState == St.Dig) { Debug.Log("Dig"); }
            else if (transform.position == movevec) {
                PlayerState = St.Idle;
                animator.SetBool("InRadder", false);
                animator.SetBool("InBar", false);
                animator.SetBool("InAir", false);
                if (LateCheckMove())  {
                    var t = CheckTile(new Vector2());
                    if(t != null)
                    {
                        if (t.name == "radder") {
                            PlayerState = St.Radder;
                        }
                        else if(t.name == "bar") {
                            PlayerState = St.Bar;
                        }
                    }
                    CanMove = true;
                }
                else  {
                    var m = Fall();
                    movevec = new Vector3(player.transform.position.x + m.x, player.transform.position.y + m.y, player.transform.position.z);
                    PlayerState = St.Fall;
                }
            }
            if (PlayerState == St.Fall) transform.position = Vector3.MoveTowards(player.transform.position, movevec, FallSpeed * Time.deltaTime);
            else transform.position = Vector3.MoveTowards(player.transform.position, movevec, speed * Time.deltaTime);

            //------  アニメーション  ------
            if (PlayerState == St.Move)
            {

            }
            else if (PlayerState == St.Radder)
            {
                animator.SetBool("InRadder", true);
            }
            else if(PlayerState == St.Fall)
            {
                animator.SetBool("InAir", true);
                return;
            }
            if (PlayerState == St.Bar) {
                animator.SetBool("InBar", true);
            }
            if (PlayerState == St.Dig) {
                animator.SetBool("InDig", true);
            }
            var s = Vector3.Distance(player.transform.position, movevec);
            animator.SetFloat("Speed", s);

            var y = Getpos().y;
            if (y == 6) { gameManager.Result(true); }
        }
    }

    Vector2 Getpos(){
        return new Vector2(player.transform.position.x, player.transform.position.y);
    }

    public TileBase CheckTile(Vector2 t)//タイルがあるか
    {
        var tile = gameManager.useblocktile;
        var p = tile.WorldToCell(Getpos() + t);
        if (tile.HasTile(p)) {
            return tile.GetTile<TileBase>(p);
        }

        tile = gameManager.usemovetile;
        p = tile.WorldToCell(Getpos() + t);
        if (tile.HasTile(p))
        {
            return tile.GetTile<TileBase>(p);
        }

        if (gameManager.goolobj_)
        {
            tile = gameManager.useGooltile;
            p = tile.WorldToCell(Getpos() + t);
            if (tile.HasTile(p))
            {
                return tile.GetTile<TileBase>(p);
            }
        }
        

        return null;
    }
    public Vector3Int GetTilepos(Vector2 t)//タイルのポジション取得
    {
        var tile = gameManager.useblocktile;
        var p = tile.WorldToCell(Getpos() + t);
        return p;
    }


    public bool ChackMove(TileBase b)//動けるかタイルの種類をチェック
    {
        if(b == null){
            return true;
        }
        if (b.name == "bar" || b.name == "radder" || b.name == "block6" || b.name == "gold")
        {
            return true;
        }
        return false;
    }

    public bool LateCheckMove()//動いた後のチェック
    {
        var t = CheckTile(new Vector2());
        if (t != null) {
            if (t.name == "gold") {
                GetGold++;
                gameManager.GoldGet();
                gameManager.usemovetile.SetTile(GetTilepos(new Vector2()), null);
            }
        }

        t = CheckTile(new Vector2(0, -1));
        //下に何もないとき
        if (t == null||t.name == "block6")
        {
            t = CheckTile(new Vector2());
            //playerの位置にとどまれるものがあるか
            if(t == null) {
                return false;
            }
            else if(t.name != "bar" || t.name != "radder") {

            }
            
        }
        return true;
    }

    public Vector3 Fall()//おちる先を決める関数
    {
        int x = -2;
        while (true) {
            var t = CheckTile(new Vector2(0, x));
            if(t != null)
            {
                if(t.name == "bar" || t.name == "radder")
                {
                    x--;
                }
                break;
            }
            x--;
        }
        x++;
        return new Vector3(0,x,0);
    }

    void Dig(Vector2 side)//掘るか決める関数
    {
        if (CanMove)
        {
            var t = CheckTile(side);
            if (t != null)
            {
                if (t.name == "block1")
                {
                    PlayerState = St.Dig;
                    CanMove = false;
                    StartCoroutine(DigMove(side));
                }
            }
        }
    }

    IEnumerator DigMove(Vector2 side)//掘るコルーチン
    {
        yield return StartCoroutine(Enumerator(side));
        animator.SetBool("InDig", false);
        PlayerState = St.Idle;
        CanMove = true;
        StartCoroutine(Fill(side));
        yield return null;
    }
    IEnumerator Enumerator(Vector2 side) {
        var pos = GetTilepos(side);
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock2);
        yield return new WaitForSeconds(DigSpeed);
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock3);
        yield return new WaitForSeconds(DigSpeed);
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock4);
        yield return new WaitForSeconds(DigSpeed);
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock5);
        yield return new WaitForSeconds(DigSpeed);
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock6);
        yield return new WaitForSeconds(DigSpeed);
    }
    IEnumerator Fill(Vector2 side)  //埋めるコルーチン
    {
        var pos = GetTilepos(side);
        for (int i = 0; i < 5; i++) {
            yield return new WaitForSeconds(1);
        }
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock5);
        yield return new WaitForSeconds(DigSpeed);
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock4);
        yield return new WaitForSeconds(DigSpeed);
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock3);
        yield return new WaitForSeconds(DigSpeed);
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock2);
        yield return new WaitForSeconds(DigSpeed);
        gameManager.useblocktile.SetTile(pos, gameManager.data.Brock1);
        yield return new WaitForSeconds(DigSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "TileMap")
        {
            Debug.Log("co");
            var i = Getpos();
            Vector3Int pos = new Vector3Int((int)i.x, (int)i.y + 1, 0);
            transform.position = pos;
        }
    }
}