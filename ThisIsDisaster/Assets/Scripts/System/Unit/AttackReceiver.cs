using UnityEngine;
using System.Collections;

public class AttackReceiver : MonoBehaviour
{
    public delegate void OnAttackReceived(UnitModel attacker, float damage);
    public Collider2D _reciveCollider;
    public Rigidbody2D _rigiedBody;
    OnAttackReceived _receivedAction = null;

    // Use this for initialization
    void Start()
    {
        if (_reciveCollider == null) {
            EmergencyLoadCollider();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void EmergencyLoadCollider() {
        _reciveCollider = gameObject.AddComponent<BoxCollider2D>();
        _rigiedBody = gameObject.AddComponent<Rigidbody2D>();

        _rigiedBody.bodyType = RigidbodyType2D.Kinematic;
    }

    public void SetReceiveAction(OnAttackReceived action) {
        _receivedAction = action;
    }

    public void OnAttackReceive(UnitModel attacker, float damage) {
        if (_receivedAction != null) {
            _receivedAction(attacker, damage);
        }
    }
}
