using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D RB { get; private set; }

    [SerializeField] PlayerScriptable _settings;

    // Start is called before the first frame update
    void Awake()
    {
        RB = GetComponent<Rigidbody2D>(); ;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        print(accelRate);
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
}
