using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ArmyManager : MonoBehaviour
{
    public float battleSpeed = 1.0f;
    public Text blueArmyText;
    public Text redArmyText;
    public GameObject redSoldierPrefab;
    public GameObject blueSoldierPrefab;

    /// <summary>
    /// Used by lanchester equation.
    /// </summary>
    private bool battleStarted = false;
    private bool battleFinished = false;
    private bool doneSimulation = false;
    private bool redArmyWon = false;
    private float battleTime = 0f;
    private double d;
    private int redArmyUnits;
    private int curRedArmyUnits;
    private float redArmyEff;
    private int blueArmyUnits;
    private int curBlueArmyUnits;
    private float blueArmyEff;
    private List<Soldier> redArmy;
    private List<Soldier> blueArmy;

	private void Awake()
    {
        redArmy = new List<Soldier>();
        blueArmy = new List<Soldier>();
    }

    private void Start()
    {
        redArmyUnits = PlayerPrefs.GetInt(BattleConfig.redArmyUnitsKey);
        redArmyEff = PlayerPrefs.GetFloat(BattleConfig.redArmyEffKey);
        blueArmyUnits = PlayerPrefs.GetInt(BattleConfig.blueArmyUnitsKey);
        blueArmyEff = PlayerPrefs.GetFloat(BattleConfig.blueArmyEffKey);
        redArmyText.text = String.Concat("Ejército Rojo: ", redArmyUnits);
        blueArmyText.text = String.Concat("Ejército Azul: ", blueArmyUnits);
        d = Math.Sqrt(redArmyEff * blueArmyEff);
        curRedArmyUnits = redArmyUnits;
        curBlueArmyUnits = blueArmyUnits;
        InstantiateArmies();
        AssignTargets();
        StartBattle();
    }

    private void Update()
    {
        if (doneSimulation) return;
        else if (battleStarted)
        {
            battleTime += Time.deltaTime;
            RedArmyDiffEq(battleTime * Time.deltaTime * battleSpeed);
            BlueArmyDiffEq(battleTime * Time.deltaTime * battleSpeed);
            if (curRedArmyUnits <= 0)
            {
                Debug.Log("Blue Army won!");
                battleFinished = true;
                battleStarted = false;
                redArmyWon = false;
                // Consider handling post-battle logic here instead of quitting.
            }

            if (curBlueArmyUnits <= 0)
            {
                Debug.Log("Red Army won!");
                battleFinished = true;
                battleStarted = false;  
                redArmyWon = true;
                // Consider handling post-battle logic here instead of quitting.
            }
            Debug.Log("left red: " + curRedArmyUnits + " left blue: " + curBlueArmyUnits);
        }
        else if (battleFinished)
        {
            Debug.Log("si entro 66");
            if (redArmyWon) foreach (Soldier s in redArmy) s.SetVictorious();
            else foreach (Soldier s in blueArmy) s.SetVictorious();
            doneSimulation = true;
            Debug.Log("Battle finished!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Soldier s = other.GetComponent<Soldier>();
        if (s != null)
        {
            s.SetAttack(true, true);
            battleStarted = true;
        }
    }

    public void StartBattle()
    {
        foreach (Soldier s in redArmy)
        {
            s.Move();
        }
        foreach (Soldier s in blueArmy)
        {
            s.Move();
        }
    }

    public void ResetBattle()
    {

    }

    private void InstantiateArmies()
    {
        int redWidth = redArmyUnits / 10;
        int blueWidth = blueArmyUnits / 10;
        Vector3 pos0 = new Vector3(transform.position.x, transform.position.y, transform.position.z - 20f);
        Vector3 pos1 = new Vector3(transform.position.x, transform.position.y, transform.position.z + 20f);
        for (int r = 0; r < redArmyUnits; r++)
        {
            if (r % redWidth == 0)
            {
                pos0 -= Vector3.forward * 3f;
                pos0 = new Vector3(transform.position.x, transform.position.y, pos0.z);
            }
            redArmy.Add(Instantiate(redSoldierPrefab, pos0, redSoldierPrefab.transform.rotation).GetComponent<Soldier>().Initialize(true, r));
            pos0 += Vector3.right * 3f;
        }
        for (int r = 0; r < blueArmyUnits; r++)
        {
            if (r % blueWidth == 0)
            {
                pos1 += Vector3.forward * 3f;
                pos1 = new Vector3(transform.position.x, transform.position.y, pos1.z);
            }
            blueArmy.Add(Instantiate(blueSoldierPrefab, pos1, blueSoldierPrefab.transform.rotation).GetComponent<Soldier>().Initialize(false, r));
            pos1 += Vector3.right * 3f;
        }
    }

    private void AssignTargets()
    {
        List<Soldier> aux = new List<Soldier>();
        if (redArmy.Count >= blueArmy.Count)
        {
            for (int i = 0; i < redArmyUnits; i++) aux.Add(blueArmy[i % blueArmy.Count]);
            for (int i = 0; i < redArmyUnits; i++)
            {
                /// Generate random number
                int rand = UnityEngine.Random.Range(0, aux.Count - 1);
                Soldier target = aux[rand];
                /// Assign soldier objectives
                redArmy[i].SetTarget(target);
                target.SetTarget(redArmy[i]);
                aux.RemoveAt(rand);
            }
        }
        else
        {
            for (int i = 0; i < blueArmyUnits; i++) aux.Add(redArmy[i % redArmy.Count]);
            for (int i = 0; i < blueArmyUnits; i++)
            {
                /// Generate random number
                int rand = UnityEngine.Random.Range(0, aux.Count - 1);
                Soldier target = aux[rand];
                /// Assign soldier objectives
                blueArmy[i].SetTarget(target);
                target.SetTarget(blueArmy[i]);
                aux.RemoveAt(rand);
            }
        }
    }

    private void RandomSoldierDeath(bool isRedArmy)
    {
        Soldier s;
        if (isRedArmy)
        {
            s = redArmy[UnityEngine.Random.Range(0, redArmy.Count)];
            s.Kill();
            s.myTarget.SetTarget(redArmy[UnityEngine.Random.Range(0, redArmy.Count)]);
        }
        else
        {
            s = blueArmy[UnityEngine.Random.Range(0, blueArmy.Count)];
            s.Kill();
            s.myTarget.SetTarget(blueArmy[UnityEngine.Random.Range(0, blueArmy.Count)]);
        }
    }
    
    /// <summary>
    /// RedArmy Differential Equation
    /// </summary>
    /// <param name="inputTime"></param>
    private void RedArmyDiffEq(float inputTime)
    {
        double number = redArmyUnits * Math.Cosh(d * inputTime) + ((-blueArmyEff * blueArmyUnits) / d) * Math.Sinh(d * inputTime);
        number = Math.Ceiling(number);
        if (number < curRedArmyUnits)
        {
            curRedArmyUnits = Convert.ToInt32(number);
            if (curRedArmyUnits <= 0)
            {
                Debug.Log("Blue Army won!");
                battleFinished = true;
                redArmyWon = false;
                // Consider handling post-battle logic here instead of quitting.
                return;
            }
            RandomSoldierDeath(true);
            redArmyText.text = String.Concat("Ejército Rojo: ", curRedArmyUnits);
        }
    }

    /// <summary>
    /// RedArmy Differential Equation
    /// </summary>
    /// <param name="inputTime"></param>
    private void BlueArmyDiffEq(float inputTime)
    {
        double number = blueArmyUnits * Math.Cosh(d * inputTime) + ((-redArmyEff * redArmyUnits) / d) * Math.Sinh(d * inputTime);
        number = Math.Ceiling(number);
        if (number < curBlueArmyUnits)
        {
            curBlueArmyUnits = Convert.ToInt32(number);
            if (curBlueArmyUnits <= 0)
            {
                Debug.Log("Red Army won!");
                battleFinished = true;
                redArmyWon = true;
                // Consider handling post-battle logic here instead of quitting.
                return;
            }
            RandomSoldierDeath(false);
            blueArmyText.text = String.Concat("Ejército Azul: ", curBlueArmyUnits);
        }
    }

}
