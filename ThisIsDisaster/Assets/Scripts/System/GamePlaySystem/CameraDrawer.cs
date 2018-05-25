using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrawer : MonoBehaviour {
    new Camera camera;
    public bool isEnabled = true;
    public float dist = 2f;
    float displayRate = 1f;
    float cameraSize = 1f;

    float _maxDist = 0f;
    public GameObject ActiveControl;

	// Use this for initialization
	void Start () {
        camera = Camera.main;
        cameraSize = camera.orthographicSize;
        displayRate = Screen.width / Screen.height;

        _maxDist = cameraSize * cameraSize + displayRate + dist;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LateUpdate()
    {
        if (!IsInCamera())
        {
            if (ActiveControl.activeInHierarchy)
            {
                ActiveControl.SetActive(false);
            }
        }
        else {
            if (!ActiveControl.activeInHierarchy) {
                ActiveControl.SetActive(true);
            }
        }
    }

    bool IsInCamera() {
        Vector3 pos = transform.position;
        Vector3 dist = camera.transform.position - pos;
        return dist.magnitude <= _maxDist;
    }
}
