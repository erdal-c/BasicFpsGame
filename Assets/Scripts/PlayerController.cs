using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    WeaponManager weaponManager;
    Animator playerAnimator;

    [SerializeField]
    float playerSpeed;
    int playerHealth = 100;

    Vector3 gravityVector;
    float gravity = -10f;

    public AudioSource playerMoveSound;
    public AudioSource playerJumpSound;
    public AudioSource playerHitSound;
    public AudioSource ammoBoxSound;
    

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        playerAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        GravityControl();
        PlayerJump();
        playerAnimator.SetBool("playerScope", weaponManager.ReturnScopeData());
    }
    private void PlayerMove()
    {
        Vector3 movVector = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        playerAnimator.SetFloat("Move", movVector.magnitude);   //Set animator parameter for walking weapon shake.


        float deger = playerSpeed * Time.deltaTime;
        characterController.Move(new Vector3(movVector.x * deger, movVector.y * 0, movVector.z * deger));

        gravityVector.y += gravity * 4 * Time.deltaTime;
        characterController.Move(gravityVector * Time.deltaTime);

        if (movVector.magnitude > 0.2 && characterController.isGrounded)
        {
            playerMoveSound.enabled= true;
        }
        else
        {
            playerMoveSound.enabled = false;
        }
    }

    void PlayerJump()
    {
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            gravityVector.y += Mathf.Sqrt(playerSpeed * -2f * gravity);
            playerJumpSound.Play();
        }
    }

    void GravityControl()
    {
        if(characterController.isGrounded && gravityVector.y < 0 )  
        {
            gravityVector.y = -2f;
        }
    }

    void GetDamage(int damageAmount)
    {
        playerHealth -= damageAmount;
        if (playerHealth <= 0)
        {
            PlayerDeath();
        }
    }

    void PlayerDeath()
    {
        playerHealth = 0;
    }

    public int ReturnHealth()
    {
        return playerHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AmmoBox"))
        {
            Destroy(other.gameObject);
            weaponManager.SetRepo(24);
            ammoBoxSound.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            GetDamage(10);
            playerHitSound.Play();
        }
    }


}