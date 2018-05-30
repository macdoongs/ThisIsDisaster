using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderRenderEffect : MonoBehaviour {
    public Gradient[] gradients;
    public SpriteRenderer rend;
    public SpriteRenderer black;
    Timer _effectTimer = new Timer();
    public delegate void EffectEnd();
    EffectEnd _endEvent = null;
    int _currentSeleceted = 0;

    public void SetEndEvent(EffectEnd end) {
        _endEvent = end;
    }

    public void StartEffect(float time)
    {
        _effectTimer.StartTimer(time);
        _currentSeleceted = UnityEngine.Random.Range(0, gradients.Length);
        SetColor(0f);
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_effectTimer.started) {
            SetColor(_effectTimer.Rate);
            if (_effectTimer.RunTimer()) {
                if (_endEvent != null) {
                    _endEvent();
                }
                gameObject.SetActive(false);
            }
        }
    }

    Gradient GetGradient() {
        return gradients[_currentSeleceted];
    }

    void SetColor(float rate) {
        Color c = GetGradient().Evaluate(rate);
        rend.color = c;

        var a = c.a;
        var bColor = black.color;
        bColor.a = 0.5f * (1 - a);
        black.color = bColor;
    }
}
