using System;
using System.Collections;
using System.Collections.Generic;
using Suntail;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject settingPanel;
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public Button resumeButton;
    public Button quitButton;

    public bool isPanelOpen = false;
    [SerializeField]private PlayerController player;
    [SerializeField] private Canvas playerCanvas;

    private void Start()
    {
        playerCanvas.enabled = true;
        isPanelOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Setting Manager Start");
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }

        Debug.Log("SetVolume AddListener");
        volumeSlider.onValueChanged.AddListener(SetVolume);

        Debug.Log("ResumeGame AddListener");
        resumeButton.onClick.AddListener(ResumeGame);
        Debug.Log("QuitGame AddListener");
        quitButton.onClick.AddListener(QuitGame);

        Debug.Log("SetActive False");
        settingPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        if (player.isDialogue || player.isInventoryOpen || ShopManager.Instance.IsOpen) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPanelOpen)
            {
                Debug.Log("isPanelClose");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                settingPanel.SetActive(false);
                isPanelOpen = false;
            }
            else
            {
                Debug.Log("isPanelOpen");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                settingPanel.SetActive(true);
                isPanelOpen = true;
            }
        }
    }
    

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    private void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        settingPanel.SetActive(false);
        isPanelOpen = false;
    }

    public void QuitGame()
    {
        Debug.Log("저장");
        PlayerPrefs.Save();
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
