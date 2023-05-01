using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    int enemyHealth = 200;
    
    [Header("Nav Mesh Agent")]
    NavMeshAgent enemyAgent;
    Transform player;
    Animator enemyAnimator;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public Transform projectile;
    public GameObject bullet;

    [Header("Patrolling")]
    Vector3 walkPoint;
    Vector3 distanceToWalkPoint;
    public float walkRange;
    public bool isWalking;

    [Header("Sight and Attack")]
    public float sightRange;
    public float attackRange;
    public bool isInSightRange, isInAttackRange;
    float attackDelay = 0.3f;
    float nextAttack;
    public bool isAttacking;
    float walkTime = 0f;

    [Header("Enemy Audio")]
    public AudioSource enemyFireSound;
    public AudioSource enemyMoveSound;
    public AudioSource enemyHitSound;

    public ParticleSystem enemyMuzlleFlash;
    bool isEnemyDeath;
    bool isEnemyProvoke;
    Collider[] myProvokedAllies;

    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        isInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        myProvokedAllies = Physics.OverlapSphere(transform.position, sightRange, enemyLayer);  //take a array of enemy allies collider for check eachother.
        IsAllyProvoked();

        if (!isEnemyDeath)
        {
            if (!isInSightRange && !isInAttackRange && !isEnemyProvoke)
            {
                enemyAnimator.SetBool("Detect", false);
                enemyAgent.speed = 1f;
                Patrolling();
            }
            else if ((isInSightRange && !isInAttackRange) || (isEnemyProvoke && !isInAttackRange))
            {
                enemyAgent.speed = 6f;
                enemyAnimator.SetBool("Idle", false);
                enemyAnimator.SetBool("Walk", false);
                enemyAnimator.SetBool("Detect", true);
                enemyAnimator.SetBool("Attack", false);
                Detecting();
            }
            else if (isInAttackRange || isEnemyProvoke)
            {
                enemyAnimator.SetBool("Detect", false);
                enemyAnimator.SetBool("Attack", true);
                EnemyAttack();
            }
        }
    }
      
    void Patrolling()
    {
        

        if (!isWalking)
        {
            float randomPosX = Random.Range(-walkRange, walkRange);
            float randomPosZ = Random.Range(-walkRange, walkRange);

            walkPoint = new Vector3(transform.position.x + randomPosX, transform.position.y, transform.position.z + randomPosZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer) && walkTime < Time.timeSinceLevelLoad)
            {
                isWalking = true;
            }
        }

        if (isWalking)
        {
            enemyAgent.isStopped= false;
            enemyAgent.angularSpeed = 360;
            enemyAgent.SetDestination(walkPoint);
            enemyAnimator.SetBool("Idle", false);
            enemyAnimator.SetBool("Walk", true);
            enemyMoveSound.Play();
        }
            
        distanceToWalkPoint = transform.position - walkPoint;
        if(distanceToWalkPoint.magnitude < 1f)
        {
            if (isWalking)
            {
                walkTime = Random.Range(3f,5f) + Time.timeSinceLevelLoad;

                enemyAgent.isStopped = true;

                enemyAnimator.SetBool("Idle", true);
                enemyAnimator.SetBool("Walk", false);
            }  
            isWalking= false;
        }
    }

    void Detecting()
    {
        enemyAgent.isStopped = false;
        enemyAgent.SetDestination(player.position);
        transform.LookAt(player.position);
    }

    void EnemyAttack()
    {
        enemyAgent.SetDestination(transform.position);
        transform.LookAt(player.position);

        isAttacking = false;
            
        if(nextAttack < Time.timeSinceLevelLoad) 
        {
            nextAttack = attackDelay + Time.timeSinceLevelLoad;
            Rigidbody rb = Instantiate(bullet, projectile.position, Quaternion.LookRotation(projectile.up,-projectile.forward)).GetComponent<Rigidbody>();
            rb.AddForce(projectile.forward * 100f, ForceMode.Impulse);
            Destroy(rb.gameObject,1f);
            enemyMuzlleFlash.Play();
            enemyFireSound.Play();
            isAttacking = true;
        }
    }

    public void EnemyGetDamage(int damage)
    {
        if (!isEnemyDeath)
        {
            enemyHealth -= damage;

            isEnemyProvoke = true;
            Detecting();
            if (enemyHealth <= 0)
            {
                EnemyDeath();
            }
        }
    }

    void EnemyDeath()
    {
        isEnemyDeath = true;
        enemyAgent.isStopped = true;   // isStopped NavMeshAgent'ýn kendi bool'ý. enemyAgent.Stop() da kullanýlabilir ama V.studio bu bool'u kullanmayý öneriyor.
        enemyAnimator.SetLayerWeight(1, 1);
        enemyAnimator.SetBool("DeathBool", true);
        enemyAnimator.SetFloat("Death", Random.Range(0f, 1f));
        Destroy(this.gameObject,10f);
    }

    public bool MyAllyProvoked()
    {
        return isEnemyProvoke;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void IsAllyProvoked()
    {
        if (myProvokedAllies[0].GetComponent<EnemyManager>().MyAllyProvoked())
        {
            isEnemyProvoke = true;
        }
    }
}
