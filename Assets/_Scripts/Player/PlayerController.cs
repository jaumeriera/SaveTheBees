using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerScriptable _settings;

    Animator animator;

    public int rescuedBees;
    public int bullets;

    private float shootCooldown;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        rescuedBees = 0;
        bullets = 5;
        shootCooldown = 0;
    }

    private void Update() {
        if (shootCooldown > 0) {
            shootCooldown -= Time.deltaTime;
        }
        if(Input.GetAxisRaw("Fire1") > 0 && CanShoot()) {
            shootCooldown = _settings.shootCooldown;
            bullets -= 1;
            animator.SetTrigger("Shoot");
            print("shot");
        }
    }

    private bool CanShoot() {
        return shootCooldown <= 0 && bullets > 0; 
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        #region bee
        if (collision.gameObject.layer == (int)Layers.Bee) {
            rescuedBees += 1;
            collision.gameObject.SetActive(false);
        }
        #endregion
        #region bullet
        if (collision.gameObject.layer == (int)Layers.Bullet) {
            bullets += 1;
            collision.gameObject.SetActive(false);
        }
        #endregion
    }
}
