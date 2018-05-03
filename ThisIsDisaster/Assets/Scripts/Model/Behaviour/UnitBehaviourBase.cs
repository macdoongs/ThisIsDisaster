using UnityEngine;
using System.Collections;

public class UnitBehaviourBase : MonoBehaviour
{
    public UnitControllerBase Controller = null;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Initialize() { }

    public virtual void OnStart() { }

    public virtual void OnExecute() { }

    public virtual void OnLateExecute() { }
    
}
