using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip StartClip;
    public AudioClip MusicLoop;
    public AudioClip GameOverClip;

    private GameManager _gameManager;

    private AudioSource _musicAudioSource;

    private void Awake()
    {
        _musicAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _gameManager = GameManager.instance;
    }

    private void Update()
    {
        if(_musicAudioSource.clip == StartClip && !_musicAudioSource.isPlaying)
        {
            _musicAudioSource.Stop();
            _musicAudioSource.clip = MusicLoop;
            _musicAudioSource.loop = true;
            _musicAudioSource.Play();
        }

        if (_gameManager.GameState == GameManager.GameStateType.EndGame)
        {
            if (_musicAudioSource.isPlaying && _musicAudioSource.clip != GameOverClip)
            {
                _musicAudioSource.Stop();
                _musicAudioSource.clip = GameOverClip;
                _musicAudioSource.loop = false;
                _musicAudioSource.Play();
            }
        }
    }
}
