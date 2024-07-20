using UnityEngine;

public class BGM_Music : MonoBehaviour
{
    public AudioClip music;
    [Range(0, 1)]
    public float volume = 1;
    public string display_name = "";
}
