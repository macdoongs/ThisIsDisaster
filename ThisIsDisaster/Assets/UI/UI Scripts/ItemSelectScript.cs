using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour {

    public Button itemSelectButton;
    public Vector3 itemButtonPosition;
    public Sprite item;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Instantiate(itemSelectButton, itemButtonPosition, Quaternion.identity);
            Debug.Log("Collider");
        }

    }


}
