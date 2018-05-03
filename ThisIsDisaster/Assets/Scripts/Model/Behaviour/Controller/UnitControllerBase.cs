using UnityEngine;
using System.Collections;

public class UnitControllerBase : MonoBehaviour
{
    public UnitBehaviourBase behaviour = null;
    private ItemManager _itemManager = null;

    private void Awake()
    {
        _itemManager = ItemManager.Manager;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public float GetDirection() {
        return transform.localScale.x;
    }


}
