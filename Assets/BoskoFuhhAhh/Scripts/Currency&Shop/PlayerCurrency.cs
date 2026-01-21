using System;

public class PlayerCurrency
{
    public int Amount { get; private set; }

    public PlayerCurrency(int initialAmount = 0)
    {
        Amount = initialAmount;
    }

    public void AddCurrency(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount to add cant be negative");
        }
        Amount += amount;
    }
}