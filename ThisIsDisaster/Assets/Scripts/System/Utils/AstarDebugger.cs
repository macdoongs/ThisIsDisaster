using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarDebugger : MonoBehaviour {
    public GameObject Origin;
    AstarCalculator.PathInfo path = null;
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            TileUnit originTile = RandomMapGenerator.Instance.GetTile(Origin.transform.position);
            TileUnit destTile = RandomMapGenerator.Instance.GetTile(transform.position);

            path = AstarCalculator.Instance.GetDestinationPath(originTile, destTile);
        }
	}

    private void OnDrawGizmos()
    {
        if (path != null) {
            //draw
        }
    }
}
