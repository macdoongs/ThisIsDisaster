using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireEvent : EventBase
{
    List<FireObject> fireObjects = new List<FireObject>();
    List<CellularAutomata.Coord> randcoords = CellularAutomata.Instance.GetRoomsCoord(2, 3);//화재의 위치. 2레벨 높이의 좌표 3개를 가져옴
    Timer _lifeTimer = new Timer();
    float lifeTime = GameManager.StageClockInfo.EVENT_RUN_TIME;
    public int spreadTime;// 여기서부터 시작

    //List<CellularAutomata.Coord> coords = null;// new List<CellularAutomata.Coord>();
    //FireEffect _effect = null;

    public FireEvent()
    {
        type = WeatherType.Fire;
    }

    public override void OnGenerated()
    {
        FireObject fire = new FireObject();
        fire.coord = randcoords[0]; 
        fire.effect = EventManager.Manager.GetFireEffect();
        fire.tile = RandomMapGenerator.Instance.GetTile(fire.coord.tileX, fire.coord.tileY);
        fire.effect.transform.Translate(fire.tile.transform.position);
        fireObjects.Add(fire);
    }
    public override void OnStart()
    {
        _lifeTimer.StartTimer();
        foreach(FireObject fire in fireObjects)
        {
            fire.SetActive(true);

        }
    }
    

    public override void OnEnd()
    {
        foreach (FireObject fire in fireObjects)
        {
            fire.SetActive(false);

        }
    }

    void FireSpread()
    {
        FireObject fire = new FireObject();
        fire.coord = randcoords[fireObjects.Count];
        fire.effect = EventManager.Manager.GetFireEffect();
        fire.tile = RandomMapGenerator.Instance.GetTile(fire.coord.tileX, fire.coord.tileY);
        fire.effect.transform.Translate(fire.tile.transform.position);
        fireObjects.Add(fire);
    }

    void FireEnd()
    {

    }

    public override void OnExecute()
    {
        if (_lifeTimer.started)
        {
            _lifeTimer.RunTimer();
            if(_lifeTimer.elapsed > 5)
            {
                foreach (FireObject fire in fireObjects)
                {
                    var fireEffect = fire.effect.fireObject.GetComponent<ParticleSystem>();
                    if (fireEffect != null)
                    {
                        fireEffect.maxParticles -= 1;
                    }

                }
            }
        }
    }

    public override void OnDestroy()
    {

    }

    /*public CellularAutomata.Coord getRandomCoord()
    {
        randCoord = CellularAutomata.Instance.GetRoomsCoord(2, 1)[0];
        return randCoord;
    }*/

    struct FireObject
    {
        public CellularAutomata.Coord coord;
        public TileUnit tile;
        public FireEffect effect;
        

        public void SetActive(bool state)
        {
            this.effect.SetActive(state);
        }
    }
}  // 화재 이벤트

