using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class UIHandler : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Slider masterVolSLDR;
    [SerializeField] Slider musicVolSLDR;
    [SerializeField] Slider sfxVolSLDR;

    float ogMasterVol;
    float ogMusicVol;
    float ogSfxVol;

    MixerManager mixerMngr;

    private void Awake()
    {
        mixerMngr = GetComponent<MixerManager>();
    }

    private void Start()
    {
        //Read original values
        ogMasterVol = mixerMngr.GetMasterVolume();
        ogMusicVol = mixerMngr.GetMusicVolume();
        ogSfxVol = mixerMngr.GetSFXVolume();

        //Set ui to show these values
        masterVolSLDR.value = ogMasterVol;
        musicVolSLDR.value = ogMusicVol;
        sfxVolSLDR.value = ogSfxVol;

    }

    public void OkButtonHandler()
    {
        //update mixer manager value
        mixerMngr.SetMasterVolume(masterVolSLDR.value);
        mixerMngr.SetMusicVolume(musicVolSLDR.value);
        mixerMngr.SetSFXVolume(sfxVolSLDR.value);

        //update original value
        ogMasterVol = masterVolSLDR.value;
        ogMusicVol = musicVolSLDR.value;
        ogSfxVol = sfxVolSLDR.value;

        //set audioMixer values
        mixerMngr.SetMixer_Master();
        mixerMngr.SetMixer_Music();
        mixerMngr.SetMixer_SFX();

        panel.SetActive(false);
    }

    public void CancelButtonHandler() 
    {
        //update mixer manager value
        mixerMngr.SetMasterVolume(ogMasterVol);
        mixerMngr.SetMusicVolume(ogMusicVol);
        mixerMngr.SetSFXVolume(ogSfxVol);

        //reset slider ui to original
        masterVolSLDR.value = ogMasterVol;
        musicVolSLDR.value = ogMusicVol;
        sfxVolSLDR.value = ogSfxVol;

        panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panel.SetActive(true);
        }
    }

    public void UpdateMasterVol()
    {
        //Get slider value
        

    }
}
