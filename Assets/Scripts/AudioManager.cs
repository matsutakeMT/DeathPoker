using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip scream;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = scream;
    }

    public void SoundScream()
    {
        audioSource.Play();
    }
}
