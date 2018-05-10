using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 디버그용
/// </summary>
public class PlayerCurrentTile : MonoBehaviour {
    public Text debugText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (debugText) {
            var tile = RandomMapGenerator.Instance.GetTile(transform.position);
            if (tile != null)
            {
                debugText.text = tile.x + " " + tile.y;
            }
            else {
                debugText.text = "ERROR";
            }
        }
	}
}
