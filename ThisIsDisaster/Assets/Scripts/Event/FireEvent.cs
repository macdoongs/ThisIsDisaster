using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireEvent : EventBase
{
    List<FireObject> fireObjects = new List<FireObject>();
    //List<CellularAutomata.Coord> coords = null;// new List<CellularAutomata.Coord>();
                                               //FireEffect _effect = null;

    public FireEvent()
    {
        type = WeatherType.Fire;
    }

    public override void OnGenerated()
    {
        FireObject fire = new FireObject();
        fire.coord = CellularAutomata.Instance.GetRoomsCoord(3, 1)[0]; //첫 화재의 위치. 2레벨 높이의 좌표 한개 가져옴
        fire.effect = EventManager.Manager.GetFireEffect();
        fire.tile = RandomMapGenerator.Instance.GetTile(fire.coord.tileX, fire.coord.tileY);
        fire.effect.transform.Translate(fire.tile.transform.position);
        fireObjects.Add(fire);
    }
    public override void OnStart()
    {
        foreach(FireObject fire in fireObjects)
        {
            fire.SetActive(true);

        }
    }

    private void Update()
    {
        foreach (FireObject fire in fireObjects)
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

