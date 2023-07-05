using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private PrototypeControls input = null;
    private Rigidbody2D rb = null;
    private BoxCollider2D coll;
    private LogicScript logicScript;
    private PopupManager popupManager;
    private Animator animator;
    private SpriteRenderer sprite;
    private MainCameraScript mainCamera;

    [SerializeField] private LayerMask jumpableGround;

    private bool isAlive = false;
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    private float jumpStartY = 0f;
    private float animationTime = 0.3f;
    private bool canPickupRelic = false;
    private GameObject currentRelic = null;

    public float moveSpeed = 10f;
    public float jumpSpeed = 10f;
    public float maxHeight = 1.5f;
    public GameObject deathExplosion;
    public GameObject spentPC;


    private void Awake()
    {
        input = new PrototypeControls();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        logicScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        popupManager = GameObject.FindGameObjectWithTag("PopupManager").GetComponent<PopupManager>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<MainCameraScript>();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += onMovePerformed;
        input.Player.Movement.canceled += onMoveCancelleded;
        input.Player.Jump.performed += onJumpPerformed;
        input.Player.Jump.canceled += onJumpCancelled;
        input.Player.Interact.started += onInteractStarted;
        input.Player.SelfDestruct.started += onSelfDestructStarted;
        input.Player.ContinuePopup.started += onContinueStarted;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Movement.performed -= onMovePerformed;
        input.Player.Movement.canceled -= onMoveCancelleded;
        input.Player.Jump.started -= onJumpPerformed;
        input.Player.Jump.canceled -= onJumpCancelled;
        input.Player.Interact.started -= onInteractStarted;
        input.Player.SelfDestruct.started -= onSelfDestructStarted;
        input.Player.ContinuePopup.started -= onContinueStarted;
    }

    // Movement controls
    private void FixedUpdate()
    {
        bool isPCGrounded = isGrounded();
        if (isPCGrounded)
        {
            animator.SetBool("isFalling", false);
        }
        else
        {
            if (rb.position.y > jumpStartY + maxHeight)
            {
                verticalInput = 0;
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
            else if (rb.position.y <= jumpStartY + maxHeight && rb.velocity.y < 0)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
        }

        if (isAlive && !popupManager.isPopupOpen)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, verticalInput * jumpSpeed);
        }
    }

    private void onMovePerformed(InputAction.CallbackContext value)
    {
        horizontalInput = value.ReadValue<Vector2>().x;
        if (isAlive)
        {
            if (horizontalInput > 0)
            {
                sprite.flipX = false;
            }
            else if (horizontalInput < 0)
            {
                sprite.flipX = true;
            }
        }
        animator.SetBool("isRunning", true);
    }

    private void freezePlayer()
    {
        animator.SetBool("isFrozen", true);
        isAlive = false;
    }

    private void unfreezePlayer()
    {
        animator.SetBool("isFrozen", false);
        isAlive = true;
    }

    private void onMoveCancelleded(InputAction.CallbackContext value)
    {
        horizontalInput = 0;
        animator.SetBool("isRunning", false);
    }

    private void onJumpPerformed(InputAction.CallbackContext value)
    {
        if (isGrounded())
        {
            verticalInput = 1;
            jumpStartY = rb.position.y;
            animator.SetBool("isJumping", true);
        }
    }

    private void onJumpCancelled(InputAction.CallbackContext value)
    {
        verticalInput = 0;
        animator.SetBool("isJumping", false);
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    // Relic Pickup Controls/Death trigger
    private IEnumerator deathLogic()
    {
        freezePlayer();
        GameObject explosion = Instantiate(deathExplosion, rb.position, Quaternion.identity);
        mainCamera.triggerShake();
        yield return new WaitForSecondsRealtime(2);
        Destroy(explosion);
        logicScript.addLife();
        Instantiate(spentPC, rb.position, Quaternion.identity);
        rb.position = new Vector2(-4.73f, 0.13f);
        unfreezePlayer();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("Relic"))
        {
        canPickupRelic = true;
        currentRelic = collision.gameObject;
        }
        else if (collision.gameObject.tag == "Trap")
        {
        StartCoroutine(deathLogic());
        }
        else if (collision.gameObject.tag == "Finish")
        {
            freezePlayer();
            popupManager.showFinishingPopup();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("Relic"))
        {
            canPickupRelic = false;
            currentRelic = null;
        }
    }

    private void onSelfDestructStarted(InputAction.CallbackContext value)
    {
        if (!popupManager.isPopupOpen)
        {
            StartCoroutine(deathLogic());
        }
    }

    private void onInteractStarted(InputAction.CallbackContext value)
    {
        if (canPickupRelic && currentRelic is not null)
        {
            freezePlayer();
            StartCoroutine(openRelic(currentRelic));
            currentRelic = null;
            logicScript.addRelic();
        }
    }

    private IEnumerator openRelic(GameObject relic)
    {
        Animator relicAnimator = relic.GetComponent<Animator>();
        relicAnimator.SetTrigger("onRelicOpen");
        yield return new WaitForSeconds(animationTime);
        string relicName = relic.name;
        Destroy(relic);
        popupManager.showRelicPopup(relicName);
    }

    // Continue through Popup logic
    private void onContinueStarted(InputAction.CallbackContext value)
    {
        if (popupManager.isPopupOpen)
        {
            popupManager.hidePopup();
            unfreezePlayer();
        }
    }
}
