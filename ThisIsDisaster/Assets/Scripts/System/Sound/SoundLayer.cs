using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLayer : MonoBehaviour {
    const int MAXIMUM_PLAY_CLIP = 10;
    public static SoundLayer CurrentLayer {
        private set;
        get;
    }
    public GameObject SoundPlayObject = null;
    private List<SoundPlayer> _players = new List<SoundPlayer>();
    Queue<SoundPlayer> _usableQueue = new Queue<SoundPlayer>();
    List<SoundPlayer> _usingList = new List<SoundPlayer>();
    public AudioSource BGMSource;
    UnscaledTimer _bgmTimer = new UnscaledTimer();
    UnscaledTimer _bgmRestartTimer = new UnscaledTimer();

    private void Awake()
    {
        CurrentLayer = this;
        Initialize();
    }

    void Initialize() {
        foreach (var player in _players) {
            Destroy(player.gameObject);
        }
        _players.Clear();
        _usableQueue.Clear();

        for (int i = 0; i < MAXIMUM_PLAY_CLIP; i++) {
            GameObject copy = Instantiate(SoundPlayObject);
            copy.transform.SetParent(transform);
            copy.transform.localPosition = Vector3.zero;
            copy.transform.localRotation = Quaternion.Euler(Vector3.zero);

            SoundPlayer player = copy.GetComponent<SoundPlayer>();
            _players.Add(player);
            _usableQueue.Enqueue(player);
        }
    }

    public void PlayBgm(AudioClip clip) {
        BGMSource.clip = clip;
        BGMSource.Play();
        _bgmTimer.StartTimer(clip.length);   
    }

    private void Update()
    {
        if (_bgmTimer.RunTimer()) {
            _bgmRestartTimer.StartTimer(UnityEngine.Random.Range(5f, 10f));
        }

        if (_bgmRestartTimer.started) {
            if (_bgmRestartTimer.RunTimer()) {
                PlayBgm(BGMSource.clip);
            }
        }
    }

    public SoundPlayer PlaySound(string src) {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/"+src);
        return PlaySound(clip);
    }

    public SoundPlayer PlaySound(AudioClip clip, bool oneShot = true) {
        SoundPlayer player = GetIdle();
        if (player == null) {
            //there is no usable player
            return null;
        }

        _usingList.Add(player);
        player.SetClip(clip);
        if (oneShot)
        {
            player.PlayOnce();
        }
        else {
            player.PlayLoop();
        }

        return player;
    }

    SoundPlayer GetIdle() {
        if (_usableQueue.Count == 0) return null;
        return _usableQueue.Dequeue();
    }

    public void ReturnPlayer(SoundPlayer player) {
        _usableQueue.Enqueue(player);
        _usingList.Remove(player);
        player.transform.SetParent(this.transform);
    }
}
