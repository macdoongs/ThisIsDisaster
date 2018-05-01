using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderController : MonoBehaviour
{
    public Vector3 ScreenVector = new Vector3(0, 0, -1f);
    public float RaycastLength = 100f;

    public Transform RaycastPivot;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CheckRaycast();
	}

    void CheckRaycast() {
        RaycastHit[] hits = Physics.RaycastAll(RaycastPivot.transform.position, ScreenVector, RaycastLength);
        if (hits.Length == 0) {
            return;
        }
        Debug.Log(hits.Length);
        foreach (var hit in hits) {
            SpriteRenderer renderer = hit.transform.GetComponent<SpriteRenderer>();
            if (!renderer) continue;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(RaycastPivot.transform.position, ScreenVector * RaycastLength);

    }
}
