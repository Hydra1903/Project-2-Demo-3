using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat Instance { get; private set; }

    [Header("Money")]
    public int coins = 0;
    [Header("Energy")]
    public int energy = 100;
    public int maxEnergy = 100;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void RestoreEnergy(int amount)
    {
        energy -= amount;
        if (energy > maxEnergy) energy = maxEnergy;
        Debug.Log("Energy: " + energy);
    }
    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Coins: " + coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log("Spent " + amount + " coins. Remaining: " + coins);
            return true;
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false;
        }
    }
}
