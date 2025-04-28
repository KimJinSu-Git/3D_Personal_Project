using System.Collections;
using System.Collections.Generic;
using Suntail;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public GameObject settingPanel;
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public Button resumeButton;
    public Button quitButton;

    private bool isPanelOpen = false;
    [SerializeField]private PlayerController player;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }

        volumeSlider.onValueChanged.AddListener(SetVolume);

        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);

        // 처음에는 설정 패널 비활성화
        settingPanel.SetActive(false);
    }

    private void Update()
    {
        if (player.isDialogue || player.isInventoryOpen || ShopManager.Instance.IsOpen) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPanelOpen)
            {
                settingPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isPanelOpen = false;
            }
            else
            {
                settingPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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
        settingPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Debug.Log("저장");
        PlayerPrefs.Save();
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
