using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClimateType {
    Island,
    Forest,
    Desert,
    Polar
}

public class StageGenerator {
    public enum ZeroTileType {
        Slow,
        Dead,
        None
    }

    public class ClimateInfo {
        public struct EnvInfo {
            public int height;
            public int min;
            public int max;
            public int id;

            public EnvInfo(int height, int min, int max, int id) {
                this.height = height;
                this.min = min;
                this.max = max;
                this.id = id;
            }
        }

        public struct NpcInfo {
            public int max;
            public int id;

            public NpcInfo( int max, int id) {
                this.max = max;
                this.id = id;
            }
        }
        public ClimateType climateType = ClimateType.Island;
        public List<WeatherType> weatherList = new List<WeatherType>();
        public ZeroTileType zeroTileType = ZeroTileType.None;

        public int MaxHeightLevel = 3;
        public Dictionary<int, string> tileSpriteSrc = new Dictionary<int, string>();
        public List<int> uniqueGenItemList = new List<int>();
        public List<EnvInfo> envInfoList = new List<EnvInfo>();
        public List<NpcInfo> npcInfoList = new List<NpcInfo>();
#if MIDDLE_PRES
        private bool _isEarthquakeGenerated = false;
#endif


        public WeatherType GetNextWeather() {
#if MIDDLE_PRES
            if (!_isEarthquakeGenerated) {
                _isEarthquakeGenerated = true;
                return WeatherType.Yellowdust;
            }
            weatherList.Remove(WeatherType.Earthquake);
#endif
            int randIndexMax = weatherList.Count;
            int index = StageGenerator.Instance.ReadNextValue(randIndexMax);
            WeatherType selected = weatherList[index];
            weatherList.Remove(selected);
            return selected;
        }
    }

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

    private int _seed = 0;

    System.Random _stageGenRandom = null;
    private Dictionary<ClimateType, ClimateInfo> _climateDic = new Dictionary<ClimateType, ClimateInfo>();

    public StageGenerator() {
    }

    public void InitClimateType(List<ClimateInfo> infos) {
        _climateDic.Clear();
        foreach (var info in infos) {
            _climateDic.Add(info.climateType, info);
        }
    }

    public ClimateInfo GetClimateInfo(ClimateType type) {
        if (_climateDic.ContainsKey(type)) return _climateDic[type];
        return null;
    }
    
    public ClimateType GetRandomClimateType() {
        int selected = ReadNextValue(0, 4);
        
        //test code
        //return ClimateType.Forest;

        return (ClimateType)selected;
    }

    public void SetSeed(int seed) {
        //네트워크 연결이 없다면 null일 것이다
        _seed = seed;
        _stageGenRandom = new System.Random(seed);
    }

    public int GetSeed() {
        return _seed;
    }

    public int ReadNextValue() {
        if (_stageGenRandom == null)    
        {
            _stageGenRandom = new System.Random(UnityEngine.Random.Range(_randomMin, _randomMax));
        }
        return _stageGenRandom.Next(_randomMin, _randomMax);
    }

    /// <summary>
    /// 0 ~ max 사잇값
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public int ReadNextValue(int max) {
        if (max <= 0) max = 1;
        int read = ReadNextValue();
        return read % max;
    }

    /// <summary>
    /// Min 에서 Max 사잇값
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public int ReadNextValue(int min, int max) {
        if (min >= max) {
            int temp = max;
            max = min;
            min = max;
            if (min == max) {
                max++;
            }
        }

        int diff = max - min;
        return ReadNextValue(diff) + min;
    }
}
