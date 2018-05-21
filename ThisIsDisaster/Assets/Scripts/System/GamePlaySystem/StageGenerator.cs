using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator {
    private static StageGenerator _instance = null;
    public static StageGenerator Instance {
        get {
            if (_instance == null) {
                _instance = new StageGenerator();
            }
            return _instance;
        }
    }

    const int _randomMin = 0;
    const int _randomMax = 100000;

    System.Random _stageGenRandom = null;

    public StageGenerator() {
        //SetSeed(3);
    }

    public void SetSeed(int seed) {
        //네트워크 연결이 없다면 null일 것이다
        _stageGenRandom = new System.Random(seed);
    }

    public int ReadNextSeed() {
        if (_stageGenRandom == null)
        {
            _stageGenRandom = new System.Random(UnityEngine.Random.Range(_randomMin, _randomMax));
        }
        return _stageGenRandom.Next(_randomMin, _randomMax);
    }
}
