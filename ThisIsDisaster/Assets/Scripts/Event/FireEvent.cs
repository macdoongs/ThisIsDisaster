using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NPC;

public class FireEvent : EventBase {
    private int _fireGenCount = 2;
    public FireEvent()
    {
        type = WeatherType.Fire;
    }

    public override void OnStart()
    {
        var trees = EnvironmentManager.Manager.GetTrees();
        List<Environment.EnvironmentModel> selected = new List<Environment.EnvironmentModel>();
        for (int i = 0; i < _fireGenCount; i++) {
            if (trees.Count > 0)
            {
                var select = trees[StageGenerator.Instance.ReadNextValue(0, selected.Count)];
                trees.Remove(select);
                selected.Add(select);
            }
            else {
                break;
            }
        }

        foreach (var target in selected) {
            TileUnit tile = target.GetCurrentTile();
            NPCModel npc = NPCManager.Manager.MakeNPC(0);
            npc.UpdatePosition(tile.transform.position);

        }

    }

    public override void OnExecute()
    {
    }

}
/*
public class FireEvent : EventBase
{
    List<FireObject> fireObjects = new List<FireObject>();
    List<CellularAutomata.Coord> randcoords = CellularAutomata.Instance.GetRoomsCoordByRoomNum(0, 2, 3);//화재의 위치. 2레벨 높이의 좌표 3개를 가져옴
    Timer _lifeTimer = new Timer();
    float lifeTime = GameManager.StageClockInfo.EVENT_RUN_TIME;
    public float fireLifeTime = 3f;

    public float damageHealthPerSec = 0.5f; //화재주변 데미지 2f
    public float damageTime = 1f;

    public AttackSender sender;

    //List<CellularAutomata.Coord> coords = null;// new List<CellularAutomata.Coord>();
    //FireEffect _effect = null;
    
    public FireEvent()
    {
        type = WeatherType.Fire;
    }

    public override void OnGenerated()
    {
        FireObject fire = new FireObject();
        fire.effect = new FireEffect();
        fire.timer = new Timer();
        fire.damageTimer = new Timer();
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
            fire.timer.StartTimer(fireLifeTime);
            fire.damageTimer.StartTimer(damageTime);
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
        fire.effect = new FireEffect();
        fire.timer = new Timer();
        fire.damageTimer = new Timer();
        fire.coord = randcoords[fireObjects.Count];
        fire.effect = EventManager.Manager.GetFireEffect();
        fire.tile = RandomMapGenerator.Instance.GetTile(fire.coord.tileX, fire.coord.tileY);
        fire.effect.transform.Translate(fire.tile.transform.position);

        fire.timer.StartTimer(fireLifeTime);
        fire.damageTimer.StartTimer(damageTime);
        fireObjects.Add(fire);
        fire.SetActive(true);
    }

    void FireEnd()
    {

    }

    public override void OnExecute()
    {
        if (_lifeTimer.started)
        {
            _lifeTimer.RunTimer();
            foreach (FireObject fire in fireObjects)
            {
                if(fire.timer.RunTimer())
                {
                    Debug.Log("불번짐");
                    FireSpread();
                    fire.timer.StartTimer(fireLifeTime);
                }
                if(fire.damageTimer.RunTimer())
                {
                    float healthDamageRate = 1f;

                    //CharacterModel.Instance.SubtractHealth(damageHealthPerSec * healthDamageRate);

                    OnGiveDamageToPlayer(damageHealthPerSec * healthDamageRate);
                }
            }



            if(_lifeTimer.elapsed > 120)
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



    struct FireObject
    {
        public CellularAutomata.Coord coord;
        public TileUnit tile;
        public FireEffect effect;
        public Timer timer;
        public Timer damageTimer;

        public void SetActive(bool state)
        {
            this.effect.SetActive(state);
            GenFireNPC(3);
        }

        public void GenFireNPC(int genCount)
        {

            var npcCount = genCount;//0 will be current alive npc
            var npcTile = tile;
            var npcInfo = 0;

            for (int i = 0; i < genCount; i++)
            {
                var model = NPCManager.Manager.MakeNPC(npcInfo);
                var tile = npcTile;
                model.UpdatePosition(tile.transform.position);
            }
        }

        public void SplashDamage()
        {

        }
    }
}  // 화재 이벤트

*/