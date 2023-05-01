using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponManager : MonoBehaviour
{
    public Camera playerCam;
    public GameObject cursor;
    int ammo = 16;
    int repo = 32;
    bool isAmmoOver;
    bool isHit;
    float nextFire;

    [SerializeField]
    ParticleSystem muzzleFlash;
    [SerializeField]
    GameObject hitEffect, groundHitEffect;

    public AudioSource fireSound;
    public AudioSource reloadSound;


    bool scope;
    Vector3 orginalPosition;
    bool isUIElement;

    // Start is called before the first frame update
    void Start()
    {
        orginalPosition = transform.localPosition; // used move to weapon to org position after weapon shake.
    }

    // Update is called once per frame
    void Update()
    {
        isUIElement = EventSystem.current.currentSelectedGameObject == null;
        if (isUIElement && Time.timeScale > 0)
        { 
            WeaponShaker();
            CursorScaler();
            FireCheck();
            if (Input.GetKeyDown(KeyCode.R) && ammo < 16 && repo > 0)
            {
                if (!isAmmoOver)
                {
                    isAmmoOver = true;
                    reloadSound.Play();
                    Invoke("Reloader", 2f);
                }
            }
        }       
    }

    void FireHit()
    {
        RaycastHit rayHit;
        isHit = Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out rayHit);
   
        if (isHit && rayHit.transform.CompareTag("Enemy"))  //Detecting hit areas
        {
            if (rayHit.collider == rayHit.transform.GetComponents<BoxCollider>()[0])
            {
                rayHit.transform.GetComponentInParent<EnemyManager>().EnemyGetDamage(100);  // Colliders in child object therefore reaching to parent object for calling damage method via EnemyManager Script.
            }
            else if (rayHit.collider == rayHit.transform.GetComponents<BoxCollider>()[1])
            {
                rayHit.transform.GetComponentInParent<EnemyManager>().EnemyGetDamage(25);
            }
            else
            {
                rayHit.transform.GetComponentInParent<EnemyManager>().EnemyGetDamage(10);
            }
            
            Destroy(Instantiate(hitEffect, rayHit.point, Quaternion.LookRotation(rayHit.normal)), 1f);  // creating bullet holes on smitten object and after destroying bulllet hole object.
        }
        else if (isHit)
        {
            Destroy(Instantiate(groundHitEffect, rayHit.point, Quaternion.LookRotation(rayHit.normal, Vector3.forward)), 2f);
        }
    }

    void FireCheck()
    {
        if (Input.GetMouseButton(0) && nextFire < Time.timeSinceLevelLoad && isUIElement && !isAmmoOver)
        {
            nextFire = 0.1f + Time.timeSinceLevelLoad;
            FireHit();
            AmmoUse();
            muzzleFlash.Play();

        }
        if (Input.GetMouseButtonDown(1))
        {
            
            if (scope == false)
            {
                gameObject.transform.localPosition = new Vector3(0, -0.0987f, 0.117f);
                playerCam.fieldOfView = 30f;
                scope = true;
                cursor.gameObject.SetActive(false);
            }
            else
            {
                gameObject.transform.localPosition = new Vector3(0.14f, -0.13f, 0.27f);
                playerCam.fieldOfView = 60f;
                scope = false;
                cursor.gameObject.SetActive(true);
            }            
        }
    }


    void CursorScaler()
    {
        float randvalue = Random.Range(0.1f, 0.5f);
        if (Input.GetMouseButton(0) && nextFire < Time.timeSinceLevelLoad && cursor.transform.localScale.x < 3 && !isAmmoOver)
        {
            cursor.transform.localScale += new Vector3(randvalue, randvalue, 0);
        }
        else if (!Input.GetMouseButton(0) && nextFire < Time.timeSinceLevelLoad){
            if (cursor.transform.localScale.x > 1.1f)
            {
                cursor.transform.localScale -= new Vector3(randvalue/20, randvalue/20, 0);
            }
        }
    }

    void WeaponShaker()
    {

        float shakeRandomX = Random.Range(0.13f, 0.15f);
        float shakeRandomY = Random.Range(-0.12f, -0.14f);
        float shakeRandomZ = Random.Range(0.27f, 0.25f);

        if (Input.GetMouseButton(0) && !scope && nextFire < Time.timeSinceLevelLoad && !isAmmoOver)
        {
            transform.localPosition = new Vector3(shakeRandomX, shakeRandomY, shakeRandomZ);
            playerCam.transform.localRotation = Quaternion.Euler(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0f); //Cam shaking
        }
        else if (!Input.GetMouseButton(0) && !scope && nextFire < Time.timeSinceLevelLoad)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, orginalPosition, 1f);
            playerCam.transform.localRotation = Quaternion.Euler(Vector3.zero);                     // Cam rotation zero
        }
    }

    void AmmoUse()
    {
        if (ammo > 0)
        {
            ammo--;
            fireSound.Play();
        }
        if(ammo == 0)
        {
            print("ammo over");
            isAmmoOver = true;
            reloadSound.Play();
            Invoke("Reloader", 2f);
        }
    }

    void Reloader()
    {
        if(ammo + repo >= 16)
        {
            isAmmoOver= false;
            repo -= 16 - (ammo % 16);
            ammo += 16 - (ammo % 16);

            
        }
        else if(ammo + repo < 16)
        {
            isAmmoOver= false;
            ammo += repo;
            repo = 0; 
            
        }
        else
        {
            print("ammo Over");
            isAmmoOver= true;
        }
    }

    public int[] ReturnAmmo()
    {
        int[] ints = {ammo,repo};
        return ints;
    }

    public void SetRepo(int addAmmo)
    {
        repo += addAmmo;
    }

    public bool ReturnScopeData()
    {
        return scope;
    }

    public bool IsFiring()
    {
        return Input.GetMouseButton(0);
    }

    public bool NowReloading() 
    {
        return isAmmoOver;
    }
}
