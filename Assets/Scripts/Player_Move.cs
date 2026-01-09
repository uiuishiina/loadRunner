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
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Vector2 inputvec;
    bool CanMove = true;
    Vector3 movevec;
    Vector2 Beforepos;
    enum St
    {
        Idle,Move,Fall,Radder,Bar,Dig
    }
    St PlayerState = St.Idle;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
    private void Update()
    {
        if (!CanMove) {
            if (transform.position == movevec) {
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

        return null;
    }

    
    public bool ChackMove(TileBase b)//動けるかタイルの種類をチェック
    {
        if(b == null){
            return true;
        }
        if (b.name == "block0"){
            return false;
        }
        return true;
    }

    public bool LateCheckMove()//動いた後のチェック
    {
        var t = CheckTile(new Vector2(0, -1));
        //下に何もないとき
        if (t == null)
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
}