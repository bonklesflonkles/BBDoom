using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // THIS IS FOR INGAME > NOT MAIN MENU

    public static GameManager Instance;

    [SerializeField] GameObject UI;
    public Slider slide;

    bool open;
    private void Awake()
    {
        open = false;
        Instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("soundVol"))
            LoadVol();
        else
        {
            PlayerPrefs.SetFloat("soundVol", 1);
            LoadVol();
        }
    }
    public void SetVolume()
    {
        AudioListener.volume = slide.value;
        SaveVolume();
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("soundVolGame", slide.value);
    }

    public void LoadVol()
    {
        slide.value = PlayerPrefs.GetFloat("soundVolGame");
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (open)
            {
                Close();
                return;
            }
            
            Open();
        }
    }

    public void Close()
    {
        open = false;
        UI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Open()
    {
        open = true;
        UI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }
}
