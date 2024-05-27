using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleConfig : MonoBehaviour
{
    public int redArmyUnits;
    public float redArmyEff;
    public int blueArmyUnits;
    public float blueArmyEff;

    public const string redArmyUnitsKey = "RedArmyUnits";
    public const string redArmyEffKey = "RedArmyEff";
    public const string blueArmyUnitsKey = "BlueArmyUnits";
    public const string blueArmyEffKey = "BlueArmyEff";


    public void SetRedArmyUnits(string s)
    {
        try
        {
            redArmyUnits = int.Parse(s);
        }
        catch (Exception e)
        {
            redArmyUnits = 1;
        }
    }
    public void SetRedArmyEff(string s)
    {
        try
        {
            redArmyEff = float.Parse(s);
        }
        catch (Exception e)
        {
            redArmyEff = 1f;
        }
    }
    public void SetBlueArmyUnits(string s)
    {
        try
        {
            blueArmyUnits = int.Parse(s);
        }
        catch (Exception e)
        {
            blueArmyUnits = 1;
        }
    }
    public void SetBlueArmyEff(string s)
    {
        try
        {
            blueArmyEff = float.Parse(s);
        }
        catch (Exception e)
        {
            blueArmyEff = 1f;
        }
    }

    public void StartBattle()
    {
        PlayerPrefs.SetInt(redArmyUnitsKey, redArmyUnits);
        PlayerPrefs.SetFloat(redArmyEffKey, redArmyEff);
        PlayerPrefs.SetInt(blueArmyUnitsKey, blueArmyUnits);
        PlayerPrefs.SetFloat(blueArmyEffKey, blueArmyEff);
        SceneManager.LoadScene(1);
    }
}
