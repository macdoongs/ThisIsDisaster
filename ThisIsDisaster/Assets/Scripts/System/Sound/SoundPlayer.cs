using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {
    public AudioSource src;
    public bool Loop = false;
    private bool _paused = false;
    private AudioClip _currentClip = null;
    private UnscaledTimer _timer = new UnscaledTimer();

    Transform _following = null;

    public void SetClip(AudioClip clip) {
        _currentClip = clip;
    }

    public void SetVolume(float vol) {
        src.volume = Mathf.Clamp(vol, 0f, 1f);
    }

    public void SetFollowing(Transform t) {
        _following = t;
    }

    public void PlayOnce() {
        Loop = false; _paused = false;
        if (_currentClip != null) {
            gameObject.SetActive(true);
            src.clip = _currentClip;
            src.PlayOneShot(_currentClip);
            _timer.StartTimer(_currentClip.length);
        }
    }

    public void PlayLoop() {
        Loop = true; _paused = false;
        if (_currentClip != null) {
            gameObject.SetActive(true);
            src.clip = _currentClip;
            src.Play();
            _timer.StartTimer();
        }
    }

    public void Stop()
    {
        _timer.StopTimer();
        src.Stop();
        gameObject.SetActive(false);
        SoundLayer.CurrentLayer.ReturnPlayer(this);
    }

    public void Pause() {
        src.Pause();
        _paused = true;
    }

    public void UnPause() {
        src.UnPause();
        _paused = false;
    }

    private void Update()
    {
        if (_timer.started && !_paused) {
            if (_timer.RunTimer()) {
                Stop();
            }
        }
    }

    private void LateUpdate()
    {
        if (_following != null) {
            transform.position = _following.transform.position;
        }
    }
}
