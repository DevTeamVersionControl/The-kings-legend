using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{

    [SerializeField] private GameObject optionsController;
    [SerializeField]  GameObject optionsLeft;
    [SerializeField]  GameObject optionsRight;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider ambianceSlider;


    public const string MIXER_MASTER = "MasterVolume";
    public const string MIXER_MUSIC = "MusicVolume";
    public const string MIXER_SFX = "SFXVolume";
    public const string MIXER_AMBIANCE = "AmbianceVolume";



    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        ambianceSlider.onValueChanged.AddListener(SetAmbianceVolume);
    }


    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 1f);
        masterSlider.value = PlayerPrefs.GetFloat(AudioManager.MASTER_KEY, 1f);
        SFXSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, 1f);
        ambianceSlider.value = PlayerPrefs.GetFloat(AudioManager.AMBIANCE_KEY, 1f);
    }


    private void OnEnable()
    {
        optionsLeft.SetActive(true);
        optionsRight.SetActive(true);
    }

        void SetMusicVolume(float volume)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(volume) * 20);
    }

    void SetSFXVolume(float volume)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(volume) * 20);
    }
    void SetMasterVolume(float volume)
    {
        mixer.SetFloat(MIXER_MASTER, Mathf.Log10(volume) * 20);
    }

    void SetAmbianceVolume(float volume)
    {
        mixer.SetFloat(MIXER_AMBIANCE, Mathf.Log10(volume) * 20);
    }


    private void OnDisable()
    {
        PlayerPrefs.SetFloat(AudioManager.MASTER_KEY, masterSlider.value);
        PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, musicSlider.value);
        PlayerPrefs.SetFloat(AudioManager.SFX_KEY, SFXSlider.value);
        PlayerPrefs.SetFloat(AudioManager.AMBIANCE_KEY, ambianceSlider.value);
    }

    public void GoBack()
    {
        optionsLeft.SetActive(false);
        optionsRight.SetActive(false);
        optionsController.SetActive(false);
    }

}
