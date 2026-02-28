using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public AudioClip fireSound;
    public AudioClip enemyFireSound;
    public AudioClip explosionSound;
    public AudioClip wallExplosionSound;
    public AudioClip gameOverSound;
    public AudioClip gameStartSound;
    
    private Dictionary<string, AudioClip> soundDictionary;
    private AudioSource audioSource;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        audioSource = GetComponent<AudioSource>();
        InitializeSoundDictionary();
    }
    
    void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<string, AudioClip>();
        soundDictionary.Add("Fire", fireSound);
        soundDictionary.Add("EnemyFire", enemyFireSound);
        soundDictionary.Add("Explosion", explosionSound);
        soundDictionary.Add("WallExplosion", wallExplosionSound);
        soundDictionary.Add("GameOver", gameOverSound);
        soundDictionary.Add("GameStart", gameStartSound);
    }
    
    public void PlaySound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName) && soundDictionary[soundName] != null)
        {
            audioSource.PlayOneShot(soundDictionary[soundName]);
        }
    }
}
