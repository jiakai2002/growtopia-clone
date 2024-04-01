using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float horizontal;

    public float jumpForce;
    public bool onGround;
    public float moveSpeed;

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;
    public TileClass selectedTile;
    public bool fistSelected;
    public bool wrenchSelected;
    public Inventory inventory;
    public GameObject player;
    public TerrainGeneration terrainGeneration;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
        fistSelected = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                ;
            }
            else if (fistSelected)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var playerWorldPos = player.transform.position;
                terrainGeneration.hitTile(mousePos, playerWorldPos);
            }
            else if (selectedTile != null)
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var playerWorldPos = player.transform.position;
                terrainGeneration.addTile(mousePos, playerWorldPos, selectedTile);
            }
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (horizontal < 0)
            transform.localScale = new Vector3(1, 1, 1);
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        anim.SetFloat("horizontal", horizontal);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            anim.SetBool("jump", true);
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
                isJumping = false;
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W))
        {
            isJumping = false;
            anim.SetBool("jump", false);
        }
    }
}
