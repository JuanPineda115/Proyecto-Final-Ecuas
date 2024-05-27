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
    public Text battleOutcomeText;
    public GameObject redSoldierPrefab;
    public GameObject blueSoldierPrefab;

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
        
        if (battleStarted && !battleFinished)
        {
            battleTime += Time.deltaTime * battleSpeed;
            RedArmyDiffEq(battleTime);
            BlueArmyDiffEq(battleTime);
            UpdateVisualArmies();
        }

        if (battleFinished && !doneSimulation)
        {
            doneSimulation = true;
            if (redArmyWon)
            {
                foreach (Soldier s in redArmy)
                {
                    s.SetVictorious();
                }
                battleOutcomeText.text = "Red Army won!";
            }
            else
            {
                foreach (Soldier s in blueArmy)
                {
                    s.SetVictorious();
                }
                battleOutcomeText.text = "Blue Army won!";
            }
            Debug.Log("Battle finished!");
            Time.timeScale = 0;
        }

        if (curRedArmyUnits <= 0 && !battleFinished)
        {
            Debug.Log("Blue Army won!");
            battleOutcomeText.text = "Blue Army won!";
            curRedArmyUnits = 0;  
            redArmyText.text = "Ejército Rojo: 0";
            battleFinished = true;
            battleStarted = false;
            redArmyWon = false;
        }

        if (curBlueArmyUnits <= 0 && !battleFinished)
        {
            Debug.Log("Red Army won!");
            battleOutcomeText.text = "Red Army won!";
            curBlueArmyUnits = 0;  
            blueArmyText.text = "Ejército Azul: 0";
            battleFinished = true;
            battleStarted = false;
            redArmyWon = true;
        }

        Debug.Log("left red: " + curRedArmyUnits + " left blue: " + curBlueArmyUnits);
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

    private void InstantiateArmies()
    {
        Vector3 pos0 = new Vector3(transform.position.x, transform.position.y, transform.position.z - 20f);
        Vector3 pos1 = new Vector3(transform.position.x, transform.position.y, transform.position.z + 20f);
        for (int r = 0; r < redArmyUnits; r++)
        {
            if (r % (redArmyUnits / 10) == 0)
            {
                pos0 -= Vector3.forward * 3f;
                pos0 = new Vector3(transform.position.x, transform.position.y, pos0.z);
            }
            redArmy.Add(Instantiate(redSoldierPrefab, pos0, redSoldierPrefab.transform.rotation).GetComponent<Soldier>().Initialize(true, r));
            pos0 += Vector3.right * 3f;
        }
        for (int r = 0; r < blueArmyUnits; r++)
        {
            if (r % (blueArmyUnits / 10) == 0)
            {
                pos1 += Vector3.forward * 3f;
                pos1 = new Vector3(transform.position.x, transform.position.y, pos1.z);
            }
            blueArmy.Add(Instantiate(blueSoldierPrefab, pos1, blueSoldierPrefab.transform.rotation).GetComponent<Soldier>().Initialize(false, r));
            pos1 += Vector3.right * 3f;
        }
    }

    private void UpdateVisualArmies()
    {
        // Actualizar visualmente la cantidad de soldados en cada ejército
        for (int i = 0; i < redArmy.Count; i++)
        {
            redArmy[i].gameObject.SetActive(i < curRedArmyUnits);
        }
        for (int i = 0; i < blueArmy.Count; i++)
        {
            blueArmy[i].gameObject.SetActive(i < curBlueArmyUnits);
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
                int rand = UnityEngine.Random.Range(0, aux.Count - 1);
                Soldier target = aux[rand];
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
                int rand = UnityEngine.Random.Range(0, aux.Count - 1);
                Soldier target = aux[rand];
                blueArmy[i].SetTarget(target);
                target.SetTarget(blueArmy[i]);
                aux.RemoveAt(rand);
            }
        }
    }

    private void RandomSoldierDeath(bool isRedArmy)
    {
        Soldier s = null;
        if (isRedArmy && redArmy.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, redArmy.Count);
            s = redArmy[randomIndex];
            redArmy.RemoveAt(randomIndex);
            s.gameObject.SetActive(false); 
            if (redArmy.Count > 0)
            {
                s.myTarget.SetTarget(redArmy[UnityEngine.Random.Range(0, redArmy.Count)]);
            }
        }
        else if (!isRedArmy && blueArmy.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, blueArmy.Count);
            s = blueArmy[randomIndex];
            blueArmy.RemoveAt(randomIndex);
            s.gameObject.SetActive(false);  
            if (blueArmy.Count > 0)
            {
                s.myTarget.SetTarget(blueArmy[UnityEngine.Random.Range(0, blueArmy.Count)]);
            }
        }

        if (s == null)
        {
            Debug.LogWarning("Intento de eliminar un soldado de un ejército vacío.");
        }
    }


    private void RedArmyDiffEq(float inputTime)
    {
        double number = redArmyUnits * Math.Cosh(d * inputTime) + ((-blueArmyEff * blueArmyUnits) / d) * Math.Sinh(d * inputTime);
        number = Math.Ceiling(number);
        if (number < curRedArmyUnits)
        {
            curRedArmyUnits = Convert.ToInt32(number);
            if (curRedArmyUnits <= 0)
            {
                curRedArmyUnits = 0;
                Debug.Log("Blue Army won!");
                battleOutcomeText.text = "Blue Army won!";
                battleFinished = true;
                redArmyWon = false;
                redArmyText.text = "Ejército Rojo: 0";
                return;
            }
            RandomSoldierDeath(true);
            redArmyText.text = String.Concat("Ejército Rojo: ", curRedArmyUnits);
        }
    }

private void BlueArmyDiffEq(float inputTime)
    {
        double number = blueArmyUnits * Math.Cosh(d * inputTime) + ((-redArmyEff * redArmyUnits) / d) * Math.Sinh(d * inputTime);
        number = Math.Ceiling(number);
        if (number < curBlueArmyUnits)
        {
            curBlueArmyUnits = Convert.ToInt32(number);
            if (curBlueArmyUnits <= 0)
            {
                curBlueArmyUnits = 0;
                Debug.Log("Red Army won!");
                battleOutcomeText.text = "Red Army won!";
                battleFinished = true;
                redArmyWon = true;
                blueArmyText.text = "Ejército Azul: 0";
                return;
            }
            RandomSoldierDeath(false);
            blueArmyText.text = String.Concat("Ejército Azul: ", curBlueArmyUnits);

        }
    }

}
