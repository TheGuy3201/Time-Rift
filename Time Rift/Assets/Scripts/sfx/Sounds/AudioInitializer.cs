using UnityEngine;

public class AudioInitializer : MonoBehaviour
{
    [SerializeField] Sound[] sounds;
    private void Awake()
    {
        foreach (var sound in sounds)
        {
            AudioManager.AddSound(sound);
        }
    }

    private void Start()
    {
        AudioManager.Play("bg");
    }

}
