using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody2D RB { get; private set; }

    public bool isJumping { get; private set; }
    public float currentVertical { get; private set; }
    public float previousVertical { get; private set; }

    [SerializeField] PlayerScriptable _settings;

    public float LastPressedJumpTime { get; private set; }
    public float LastOnGroundTime { get; private set; }

    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private LayerMask _groundLayer;

    // Start is called before the first frame update
    void Awake() {
        RB = GetComponent<Rigidbody2D>();
        SetGravityScale(_settings.gravityScale);
        previousVertical = 0f;
    }

    // Update is called once per frame
    void Update() {
        LastOnGroundTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;

        currentVertical = Input.GetAxis("Vertical");

        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) {
            LastOnGroundTime = _settings.coyoteTime;
        }
            

        if (RB.velocity.y >= 0) {
            SetGravityScale(_settings.gravityScale);
        }
        else if (currentVertical < 0) {
            SetGravityScale(_settings.gravityScale * _settings.quickFallGravityMult);
        }
        else {
            SetGravityScale(_settings.gravityScale * _settings.fallGravityMult);
        }
        
        if (currentVertical > 0) {
            OnJump();
        }
        if (isJumping && RB.velocity.y < 0) {
            isJumping = false;
        }

        if (CanJump() && LastPressedJumpTime > 0) {
            isJumping = true;
            Jump();
        }
        previousVertical = currentVertical;
    }

    private void FixedUpdate() {
        DecelerateByFriction();
        Run();

    }

    private void Run() {
        // Calculate the direction we want to move and the desired velocity
        float targetSpeed = Input.GetAxis("Horizontal") * _settings.runMaxSpeed;
        // Calculate difference between current velocity and desired velocity
        float speedDiff = targetSpeed - RB.velocity.x;
        // Change acceleration depending on the situation
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _settings.acceleration : _settings.deceleration;
        //if we want to run but are already going faster than max run speed
        if (((RB.velocity.x > targetSpeed && targetSpeed > 0.01f) || (RB.velocity.x < targetSpeed && targetSpeed < -0.01f)) && _settings.doKeepRunMomentum) {
            accelRate = 0; //prevent any deceleration from happening, or in other words conserve are current momentum
        }
        // Calculate the velocityPower
        float velPower;
        if (Mathf.Abs(targetSpeed) < 0.01f) {
            velPower = _settings.stopPower;
        }
        else if (Mathf.Abs(RB.velocity.x) > 0 && (Mathf.Sign(targetSpeed) != Mathf.Sign(RB.velocity.x))) {
            velPower = _settings.turnPower;
        }
        else {
            velPower = _settings.accelPower;
        }
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
        movement = Mathf.Lerp(RB.velocity.x, movement, 1);
        RB.AddForce(movement * Vector2.right);
    }

    private void DecelerateByFriction() {
        Vector2 force = _settings.frictionAmmount * RB.velocity.normalized;
        force.x = Mathf.Min(Mathf.Abs(RB.velocity.x), Mathf.Abs(force.x)); //ensures we only slow the player down, if the player is going really slowly we just apply a force stopping them
        force.y = Mathf.Min(Mathf.Abs(RB.velocity.y), Mathf.Abs(force.y));
        force.x *= Mathf.Sign(RB.velocity.x); //finds direction to apply force
        force.y *= Mathf.Sign(RB.velocity.y);

        RB.AddForce(-force, ForceMode2D.Impulse); //applies force against movement direction
    }

    private void Jump() {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        float force = _settings.jumpForce;
        if (RB.velocity.y < 0) {
            force -= RB.velocity.y;
        }
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private bool CanJump() {
        return LastOnGroundTime > 0 && previousVertical <= currentVertical && !isJumping;
    }

    public void OnJump() {
        LastPressedJumpTime = _settings.jumpBufferTime;
    }

    public void SetGravityScale(float scale) {
        RB.gravityScale = scale;
    }
}
