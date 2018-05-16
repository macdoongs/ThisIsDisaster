using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackSender : MonoBehaviour
{
    public UnitModel Owner { get; private set; }
    public Collider2D AttackRange;

    Timer _attackActivateTimer = new Timer();
    float _attackActivateTime = 0.2f;

    private List<AttackReceiver> _attackedTargets = new List<AttackReceiver>();

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetOwner(UnitModel owner) {
        Owner = owner;
    }

    public void SetAttackActivateTime(float time) {
        _attackActivateTime = time;
    }

    public void OnAttack() {
        _attackedTargets.Clear();
        gameObject.SetActive(true);
        _attackActivateTimer.StartTimer(_attackActivateTime);
    }

    void EndAttack() {
        _attackedTargets.Clear();
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_attackActivateTimer.started) {
            if (_attackActivateTimer.RunTimer()) {
                EndAttack();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_attackActivateTimer.started) return;
        AttackReceiver recv = collision.GetComponent<AttackReceiver>();
        if (recv == null) return;
        if (_attackedTargets.Contains(recv)) return;

        _attackedTargets.Add(recv);
        recv.OnAttackReceive(Owner, Owner.GetAttackDamage());
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (!_attackActivateTimer.started) return;
    //    AttackReceiver recv = collision.GetComponent<AttackReceiver>();
    //    if (recv == null) return;

    //}
}
