using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    // Use this for initialization
    #region Editor variables
    [SerializeField]
    Text respawnText;

    #region Ground Check Variables
    [SerializeField]
    float groundDetectRadius = 0.2f;
    [SerializeField]
    Transform groundDetectPoint;
    [SerializeField]
    LayerMask whatIsGround;
    #endregion

    #region Wall Check Variables
    [SerializeField]
    float wallDetectRadius = 0.2f;
    [SerializeField]
    Transform wallDetectPoint;
    [SerializeField]
    LayerMask whatIsWall;
    #endregion

    #region Ice Check Variables
    [SerializeField]
    float iceDetectRadius = 0.2f;
    [SerializeField]
    Transform iceDetectPoint;
    [SerializeField]
    LayerMask whatIsIce;
    #endregion

    [SerializeField]
    float moveSpeed = 5;
    [SerializeField]
    float jumpVelocity = 5;

    float autoRotationDelay = .5f;
    [SerializeField]
    Animator animator;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    #endregion

    #region Properties
    //public Checkpoint LastCheckPoint { get; set; }
    //public Checkpoint StartingPoint { get; set; }
    #endregion

    #region Private fields
    Rigidbody2D rigidBody2D;
    bool isOnGround;
    bool isOnIce;
    [SerializeField]
    bool isOnWall;
    bool isAlive;
    bool noInput;
    #endregion

    void Start() {
        respawnText.gameObject.SetActive(false);
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            HandleMovementInput();
            UpdateIsOnGround();
            UpdateIsOnWall();
            HandleWallClimbingInput();
            HandleJumpInput();
            HandleAttackInput();
            if (this.moveSpeed > 0)
            {
                // transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                GetComponent<SpriteRenderer>();
            }
        }
        else
        {
            respawnText.gameObject.SetActive(true);
            if (Input.GetButtonDown("Jump"))
            {
                Respawn();
                respawnText.gameObject.SetActive(false);
            }
        }
    }
    #region Environmental Layer Mask Checks
    private void UpdateIsOnGround()
    {
        Collider2D[] groundObjects =
        Physics2D.OverlapCircleAll(groundDetectPoint.position, groundDetectRadius, whatIsGround);
        isOnGround = groundObjects.Length > 0;
    }

    private void UpdateIsOnWall()
    {
        Collider2D[] wallObjects =
        Physics2D.OverlapCircleAll(wallDetectPoint.position, wallDetectRadius, whatIsWall);
        isOnWall = wallObjects.Length > 0;
    }

    private void UpdateIsOnIce()
    {
        Collider2D[] iceObjects =
        Physics2D.OverlapCircleAll(wallDetectPoint.position, iceDetectRadius, whatIsIce);
        isOnIce = iceObjects.Length > 0;
    }
    #endregion

    #region Player Input Methods
    private void HandleAttackInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Weapon Fired");
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && (isOnGround || isOnIce))
        {
            float currentXVelocity = rigidBody2D.velocity.x;
            if (isOnIce) { jumpVelocity = jumpVelocity - 2; }
            Vector2 velocityToSet = new Vector2(currentXVelocity, jumpVelocity);

            rigidBody2D.velocity = velocityToSet;
        }
    }

    private void HandleWallClimbingInput()
    {
        float climbInput = Input.GetAxis("Vertical");
        animator.SetFloat("yInput", Mathf.Abs(climbInput));
        float currentXVelocity = rigidBody2D.velocity.x;

        if (isOnWall)
        {
            Debug.Log("Is On Wall");
            Vector2 velocityToSet = new Vector2(currentXVelocity, moveSpeed * climbInput * Time.deltaTime);
            rigidBody2D.velocity = velocityToSet;
        }
    }

    private void HandleMovementInput()
    {
        float movementInput = Input.GetAxis("Horizontal");
        animator.SetFloat("xInput", Mathf.Abs(movementInput));
        float currentYVelocity = rigidBody2D.velocity.y;

        bool isFacingRight = !spriteRenderer.flipX;
        if (isFacingRight && movementInput < 0)
        {
            Debug.Log("Moving right");
            isFacingRight = true;
        }
        else if (!isFacingRight && movementInput > 0)
        {
            Debug.Log("Moving left");
            spriteRenderer.flipX = false;
        }

        Vector2 velocityToSet = new Vector2(moveSpeed * movementInput * Time.deltaTime, currentYVelocity);
        rigidBody2D.velocity = velocityToSet;

        if (movementInput != Mathf.Abs(0)) { noInput = true; } else noInput = false;
    }
    #endregion
    private void LateUpdate()
    {
        if (noInput || isOnGround)
        {
            autoRotationDelay -= Time.deltaTime;

            if (autoRotationDelay <= 0)
               {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime);
                  //  rigidBody2D.MoveRotation(rigidBody2D.rotation * Time.fixedDeltaTime);
               }
        }
        else if (isOnIce) { rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation; /*autoRotationDelay = 1.4f;*/ }
        else { autoRotationDelay = .5f; }
    }

    public void TakeDamage()
    {
        isAlive = false;

    }

    public void Respawn()
    {
        isAlive = true;
      //  transform.position = LastCheckPoint.transform.position;

    }
}
