using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{ 
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;
    public UIController _uiController;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public Transform orientation;

    private bool _isGrounded;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;
    private Rigidbody _rigidBody;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.freezeRotation = true;
    }

    private void Update()
    {

        // ground check
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 7f, whatIsGround);

        RegisterInput();
        SpeedControl();

        // handle drag
        if (_isGrounded)
            _rigidBody.drag = groundDrag;
        else
            _rigidBody.drag = 0;
    }

    private void FixedUpdate()
    {
        if (_uiController.IsOptionsMenuOpened)
            return;
        MovePlayer();
    }

    private void RegisterInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if(Input.GetKeyDown(jumpKey) && readyToJump && _isGrounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        // on ground
        if(_isGrounded)
            _rigidBody.AddForce(_moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!_isGrounded)
            _rigidBody.AddForce(_moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            _rigidBody.velocity = new Vector3(limitedVel.x, _rigidBody.velocity.y, limitedVel.z);
        }

    }

    private void Jump()
    {
        //reset y velocity
        _rigidBody.velocity = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);
        _rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
 
    private void ResetJump()
    {
        readyToJump = true;
    }
}
