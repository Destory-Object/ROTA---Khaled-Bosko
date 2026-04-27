using System;
using TMPro;

[System.Serializable]
public class PlayerCurrency
{
    public int Amount { get; private set; }
    public TMP_Text text;
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

        text.text = $"Current currency: {Amount}";
    }
}