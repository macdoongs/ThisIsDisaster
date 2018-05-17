using UnityEngine;
using System.Collections;

public class AttackReceiver : MonoBehaviour
{
    public delegate void OnAttackReceived(UnitModel attacker, float damage);

    public UnitModel Owner { get; private set; }
    public Collider2D _reciveCollider;
    public Rigidbody2D _rigiedBody;
    protected OnAttackReceived _receivedAction = null;

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

    public void SetOwner(UnitModel owner) {
        Owner = owner;
    }

    protected void EmergencyLoadCollider() {
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
        Owner.OnTakeDamage(attacker, damage);
    }
}
