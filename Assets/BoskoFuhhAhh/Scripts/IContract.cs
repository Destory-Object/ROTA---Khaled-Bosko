using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public interface IContract
{
    void ExecuteAction();

   
}

public interface IHealth
{
    public void RegenHealth(int amount);
    public void TakeDamage(int amount);
    public int GetHealth();
    public void Kill();
}

public interface HealthPickUp
{
    public int GetHealth();
}