using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector2 move;
    private PlayerState currentState = PlayerState.Idle; // Trạng thái hiện tại
    public float lastMoveX;
    public float lastMoveY;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Nhận input
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");

        // Cập nhật trạng thái nhân vật dựa trên input
        if (move.sqrMagnitude > 0) // Đang di chuyển
        {
            currentState = PlayerState.Moving;
            lastMoveX = move.x;
            lastMoveY = move.y;
        }
        else // Không di chuyển
        {
            currentState = PlayerState.Idle;
        }

        // Chuyển đổi trạng thái animation
        SetAnimationState();

        // Di chuyển
        rb.velocity = move * moveSpeed;
    }

    // Phương thức chuyển đổi trạng thái animation
    private void SetAnimationState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                anim.SetFloat("Speed", 0);
                anim.SetFloat("LastMoveX", lastMoveX);
                anim.SetFloat("LastMoveY", lastMoveY);
                break;

            case PlayerState.Moving:
                anim.SetFloat("Horizontal", move.x);
                anim.SetFloat("Vertical", move.y);
                anim.SetFloat("Speed", move.sqrMagnitude);
                break;

            case PlayerState.Watering:
                anim.SetFloat("LastMoveX", lastMoveX);
                anim.SetFloat("LastMoveY", lastMoveY);
                anim.SetTrigger("watering");
                break;

            case PlayerState.Digging:
                anim.SetFloat("LastMoveX", lastMoveX);
                anim.SetFloat("LastMoveY", lastMoveY);
                anim.SetTrigger("digging");
                break;
        }
    }


    // Gọi khi thực hiện hành động tưới nước
    public void IsWatering()
    {
        currentState = PlayerState.Watering;
        SetAnimationState(); // Kích hoạt trạng thái animation ngay lập tức
    }

    // Gọi khi thực hiện hành động đào đất
    public void IsDigging()
    {
        currentState = PlayerState.Digging;
        SetAnimationState(); // Kích hoạt trạng thái animation ngay lập tức
    }

    public void ResetFlipX()
    {
        spriteRenderer.flipX = false;
    }
}
