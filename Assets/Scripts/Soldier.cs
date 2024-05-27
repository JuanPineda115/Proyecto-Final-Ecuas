using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoldierState { Idle, Move, Seeking, Attack, Dead, Victorious }

public class Soldier : MonoBehaviour
{
    public string soldierId;
    public float moveSpeed;
    public float sphereRadius;
    public SoldierState state;

    private Animator anim;
    public Soldier myTarget;
    private const string soldierTag = "soldier";
    private const string attackParam = "attack";
    private const string moveParam = "move";
    private const string deadParam = "dead";
    private const string victoriousParam = "victory";
    private bool moving;
    private bool hasTarget;
    private bool isRedArmy;

    void Awake()
    {
        anim = GetComponent<Animator>();
        hasTarget = false;
        state = SoldierState.Idle;
    }

    void Update()
    {
        Vector3 dir;
        if (state == SoldierState.Idle || state == SoldierState.Dead) return;
        else if (state == SoldierState.Move)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }
        else if (state == SoldierState.Seeking)
        {
            if (myTarget == null || myTarget.state == SoldierState.Dead) 
            {
                state = SoldierState.Move;
                anim.SetBool(moveParam, true);
                return;
            }

            if (isRedArmy) dir = (myTarget.transform.position - transform.position).normalized;
            else dir = (transform.position - myTarget.transform.position).normalized;
            transform.Translate(dir * Time.deltaTime * moveSpeed);
            if (Vector3.Distance(transform.position, myTarget.transform.position) < 5.0f)
            {
                state = SoldierState.Attack;
                anim.SetBool(moveParam, false);
                anim.SetBool(attackParam, true);
            }
        }
        else if (state == SoldierState.Attack)
        {
            if (myTarget == null || myTarget.state == SoldierState.Dead) 
            {
                state = SoldierState.Move;
                anim.SetBool(moveParam, true);
                anim.SetBool(attackParam, false);
                return;
            }

            if (Vector3.Distance(transform.position, myTarget.transform.position) > 5.0f)
            {
                if (isRedArmy) dir = (myTarget.transform.position - transform.position).normalized;
                else dir = (transform.position - myTarget.transform.position).normalized;
                transform.Translate(dir * Time.deltaTime * moveSpeed);
            }
        }
    }

    public Soldier Initialize(bool isRedArmy, int id)
    {
        this.isRedArmy = isRedArmy;
        string name = isRedArmy ? "RedArmy: " : "BlueArmy: ";
        soldierId = string.Concat(name, id);
        return this;
    }

    public void SetTarget(Soldier target)
    {
        myTarget = target;
    }

    public void Move()
    {
        state = SoldierState.Move;
        anim.SetBool(moveParam, true);
    }

    public void SetAttack(bool attack, bool notifyTarget)
    {
        state = SoldierState.Seeking;
        if (notifyTarget && myTarget != null) myTarget.SetAttack(attack, false);
    }

    public void Kill()
    {
        state = SoldierState.Dead;
        anim.SetBool(deadParam, true);
        Destroy(gameObject);
    }

    public void SetVictorious()
    {
        state = SoldierState.Victorious;
        anim.SetBool(victoriousParam, true);
    }

    public void OnHit()
    {
        Kill();
    }

    private void OnTriggerEnter(Collider other)
    {
        Soldier enemy = other.GetComponent<Soldier>();
        if (enemy != null && enemy.isRedArmy != this.isRedArmy)
        {
            OnHit();
        }
    }
}
