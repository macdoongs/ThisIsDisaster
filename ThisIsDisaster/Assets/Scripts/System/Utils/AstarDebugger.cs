using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarDebugger : MonoBehaviour {
    public static AstarDebugger Debugger {
        private set;
        get;
    }

    public GameObject Origin;
    public NPC.NPCUnit DebugUnit;

    AstarCalculator.PathInfo path = null;
    TileUnit prev = null;
    public bool clickPointDebug = false;
    private void Awake()
    {
        Debugger = this;
    }

    void Update () {
        if (Input.GetMouseButtonDown(0)) {
            //if (Origin == null) {
            //    Origin = GameManager.CurrentGameManager.GetLocalPlayer().gameObject;
            //}

            if (prev != null && clickPointDebug) {
                prev.spriteRenderer.color = Color.white;
            }
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //TileUnit originTile = RandomMapGenerator.Instance.GetTile(Origin.transform.position);
            TileUnit destTile = RandomMapGenerator.Instance.GetTile(transform.position);

            //path = AstarCalculator.Instance.GetDestinationPath(originTile, destTile);
            prev = destTile;
            if (clickPointDebug)
                prev.spriteRenderer.color = Color.red;
            if (DebugUnit != null) {
                DebugUnit.Model.MoveToTile(destTile);
                path = DebugUnit.Model.MoveControl._currentPath;
            }
        }
        
	}

    private void OnDrawGizmos()
    {
        if (path != null) {
            //draw
            Vector3 start = path.Origin.transform.position;
            Vector3 end = path.Origin.transform.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(start, (end - start));
            Vector3 prev = start;
            Gizmos.color = Color.red;
            for (int i = 1; i < path.path.Count-1; i++) {
                Vector3 dir = path.path[i].transform.position - prev;
                Gizmos.DrawRay(prev, dir);
                prev = path.path[i].transform.position;
            }
        }
    }
}
