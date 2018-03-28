using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    WATER,
    GROUND_WALKABLE,
    GROUND_UNWALKABLE
}

public class TileUnit : MonoBehaviour {
    public TileType type;
    public TempTileModel _model;

    public void SetModel(TempTileModel model){
        _model = model;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
