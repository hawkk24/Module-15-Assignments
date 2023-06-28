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

    [SerializeField] private LayerMask jumpableGround;

    private bool isAlive = true;
    private float horizontalInput = 0f;
    private float verticalInput = 0f;
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
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += onMovePerformed;
        input.Player.Movement.canceled += onMoveCancelleded;
        input.Player.Jump.performed += onJumpStarted;
        input.Player.Jump.canceled += onJumpCancelled;
        input.Player.Interact.started += onInteractStarted;
        input.Player.SelfDestruct.started += onSelfDestructStarted;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Movement.performed -= onMovePerformed;
        input.Player.Movement.canceled -= onMoveCancelleded;
        input.Player.Jump.started -= onJumpStarted;
        input.Player.Jump.canceled -= onJumpCancelled;
        input.Player.Interact.started -= onInteractStarted;
        input.Player.SelfDestruct.started -= onSelfDestructStarted;
    }

    // Movement controls
    private void FixedUpdate()
    {
        if (!isGrounded() && rb.position.y > 1.5)
        {
            verticalInput = 0;
        }
        if (isAlive)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, verticalInput * jumpSpeed);
        }
    }

    private void onMovePerformed(InputAction.CallbackContext value)
    {
        horizontalInput = value.ReadValue<Vector2>().x;
    }

    private void onMoveCancelleded(InputAction.CallbackContext value)
    {
        horizontalInput = 0;
    }

    private void onJumpStarted(InputAction.CallbackContext value)
    {
        if (isGrounded())
        {
            verticalInput = 1;
        }
    }

    private void onJumpCancelled(InputAction.CallbackContext value)
    {
        verticalInput = 0;
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    // Relic Pickup Controls/Death trigger

    private void testFunction()
    {
        Debug.Log("This is being called");
    }
    private IEnumerator deathLogic()
    {
        isAlive = false;
        GameObject explosion = Instantiate(deathExplosion, rb.position, Quaternion.identity);
        yield return new WaitForSecondsRealtime(2);
        Destroy(explosion);
        logicScript.addLife();
        Instantiate(spentPC, rb.position, Quaternion.identity);
        rb.position = new Vector2(-4.73f, 0.13f);
        isAlive = true;
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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Relic"))
        {
            canPickupRelic = false;
            currentRelic = null;
        }
    }

    private void onSelfDestructStarted(InputAction.CallbackContext value)
    {
        StartCoroutine(deathLogic());
    }

    private void onInteractStarted(InputAction.CallbackContext value)
    {
        if (canPickupRelic && currentRelic is not null)
        {
            Destroy(currentRelic);
            logicScript.addRelic();
        }
    }
}
