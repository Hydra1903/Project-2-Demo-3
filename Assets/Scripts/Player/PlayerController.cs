using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rigidbody2D;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    public float moveX, moveY;
    public float lastMoveX;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
         moveX = Input.GetAxis("Horizontal");
         moveY = Input.GetAxis("Vertical");
        // Cập nhật hướng dựa vào di chuyển

        if (moveX != 0 || moveY != 0)
        {
            lastMoveX = moveX;

            //Set Animation
            anim.SetFloat("moveX", moveX);
            anim.SetFloat("moveY", moveY);
        }

        rigidbody2D.velocity = new Vector2(moveX, moveY) * moveSpeed;

        if(moveX == 1 || moveX == -1 || moveY == 1 || moveY == -1)
        {
            anim.SetFloat("lastMoveX", moveX);
            anim.SetFloat("lastMoveY", moveY);
        }
        
    }
    public void IsWatering()
    {
        if (lastMoveX < 0)
        {
            spriteRenderer.flipX = true; // Lật hình ảnh sang trái
            anim.SetTrigger("watering");
        }
        else if (lastMoveX > 0)
        {
            spriteRenderer.flipX = false; // Không lật hình ảnh (hướng sang phải)
            anim.SetTrigger("watering");
        }
    }
    public void IsDigging()
    {
        if (lastMoveX < 0)
        {
            spriteRenderer.flipX = true; // Lật hình ảnh sang trái
            anim.SetTrigger("digging");
        }
        else if (lastMoveX > 0)
        {
            spriteRenderer.flipX = false; // Không lật hình ảnh (hướng sang phải)
            anim.SetTrigger("digging");
        }
    }
    public void ResetFlipX()
    {
        spriteRenderer.flipX = false;
    }

}
