using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Slider healtBar;
    public Slider reloadBar;
    public GameObject cursor;
    public Text ammo;

    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject deathMenu;
    public GameObject damageScreen;

    float timer = 0f;
    PlayerController playerController;
    WeaponManager weaponManager;
    ArtifactManager artifactManager;

    public AudioSource gameOverSound;
    int healthValue;
    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        weaponManager = FindObjectOfType<WeaponManager>();
        artifactManager = FindObjectOfType<ArtifactManager>();
        healthValue = playerController.ReturnHealth();
    }

    // Update is called once per frame
    void Update()
    {
        healtBar.value = playerController.ReturnHealth();
        AmmoText();
        ReloadBar();
        WinMenu();
        DeathMenu();
        DamageScreenCaller();

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            PauseMenu();
        }
    }

    private void ReloadBar()
    {
        if (weaponManager.NowReloading() && timer < Time.timeSinceLevelLoad  && weaponManager.ReturnAmmo()[1] > 0)
        {
            timer = 0.1f + Time.timeSinceLevelLoad;
            reloadBar.gameObject.SetActive(true);
            reloadBar.value += 6f;
            if (reloadBar.value >= 100)
            {
                reloadBar.gameObject.SetActive(false);
            }
        }
        if(!weaponManager.NowReloading())
        {
            if (reloadBar.value == 100)
            {
                reloadBar.value = 0;
            }
        }
    }

    public void PauseMenu()
    {
        if (!pauseMenu.activeSelf)
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);            
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void WinMenu()
    {
        if (artifactManager.isGameWin)
        {
            Time.timeScale = 0;
            winMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            artifactManager.isGameWin = false;
        }
    }

    void DeathMenu()
    {
        if(playerController.ReturnHealth() == 0 && !deathMenu.activeSelf)
        {
            Time.timeScale = 0;
            deathMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            gameOverSound.enabled = true;
        }
    }

    void DamageScreenCaller()
    {
        
        if(playerController.ReturnHealth() < healthValue)
        {
            damageScreen.GetComponent<CanvasGroup>().alpha = 1f;
            healthValue = playerController.ReturnHealth();
        }
        else if(damageScreen.GetComponent<CanvasGroup>().alpha > 0)
        {
            damageScreen.GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
        }
    }

    void AmmoText()
    {
        ammo.text = string.Format("Ammo : {0} / {1}", weaponManager.ReturnAmmo()[0], weaponManager.ReturnAmmo()[1]);
    }


    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }
    public void RestartButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    private void CursorCheck()
    {
        if (weaponManager.ReturnScopeData())
        {
            cursor.gameObject.SetActive(false);
        }
        else
        {
            cursor.gameObject.SetActive(true);
        }
    }
    
    void CursorScale()
    {
        if(weaponManager.IsFiring() && cursor.transform.localScale.magnitude < 5) 
        {
            cursor.transform.localScale += new Vector3(0.05f, 0.05f, 0);
        }
        else
        {
            if(cursor.transform.localScale.magnitude > 1.5f)
            {
                cursor.transform.localScale -= new Vector3(0.05f, 0.05f, 0);
            }
        }
        
    }
}
