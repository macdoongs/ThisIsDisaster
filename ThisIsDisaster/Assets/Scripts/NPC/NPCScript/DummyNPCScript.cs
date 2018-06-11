using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class DummyNPCScript : NPCScriptBase
    {

        public override void Initialize()
        {
            //Debug.Log("Make Dummy NPC");
        }

        public override void OnDefeated()
        {
            Debug.Log("Dummy dead");
            Unit.animTarget.SetActive(false);
        }
    }

    public class FireNPCScript : NPCScriptBase {
        public class FireSpread
        {
            const int _INDEX_MIN = -2;
            const int _INDEX_MAX = 2;
            public NPCModel targetNPC;
            public Queue<Environment.TreeScript> nextBurnning = new Queue<Environment.TreeScript>();

            public void SpreadNext()
            {
                if (targetNPC.CurrentHp <= 0f) return;
                Queue<Environment.TreeScript> newQueue = new Queue<Environment.TreeScript>();

                if (nextBurnning.Count > 0)
                {
                    var next = nextBurnning.Dequeue();
                    while (next != null)
                    {
                        TileUnit tile = next.Model.GetCurrentTile();

                        next.OnBurn();
                        CheckTile(tile, newQueue);
                        try
                        {
                            next = nextBurnning.Dequeue();
                        }
                        catch
                        {
                            next = null;
                        }
                    }
                }

                TileUnit npcCurrent = targetNPC.GetCurrentTile();

                CheckTile(npcCurrent, newQueue);

                nextBurnning = newQueue;
            }

            void CheckTile(TileUnit tile, Queue<Environment.TreeScript> queue)
            {
                for (int i = _INDEX_MIN; i <= _INDEX_MAX; i++)
                {
                    for (int j = _INDEX_MIN; j <= _INDEX_MAX; j++)
                    {
                        TileUnit check = RandomMapGenerator.Instance.GetTile(tile.x + i, tile.y + j);
                        if (check == tile) continue;
                        var search = check._currentEnteredUnits.Find((x => x is Environment.EnvironmentModel));
                        if (search != null)
                        {
                            Environment.EnvironmentModel env = search as Environment.EnvironmentModel;
                            if (env.Script is Environment.TreeScript)
                            {
                                Environment.TreeScript ts = env.Script as Environment.TreeScript;
                                if (ts.IsBronkenOrBurnned()) continue;
                                else
                                {
                                    queue.Enqueue(ts);
                                }
                            }
                        }
                    }
                }
            }
        }
        Timer spreadTimer = new Timer();
        const float _SPREAD_TIME = 1f;
        FireSpread spread = new FireSpread();

        public override void Initialize()
        {
            spread.targetNPC = this.Model;
            spreadTimer.StartTimer(_SPREAD_TIME);
        }

        public override void OnExecute()
        {
            if (spreadTimer.RunTimer()) {

                spreadTimer.StartTimer(_SPREAD_TIME);
                spread.SpreadNext();
            }
        }

        public override void OnDefeated()
        {
            Debug.Log("Dummy dead");
            Unit.animTarget.SetActive(false);
        }
    }
}