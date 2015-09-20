using UnityEngine;
using System.Collections;

/// <summary>
/// The shopping class for handling in-app purchases.
/// </summary>
public class Shopping : MonoBehaviour {

    /// <summary>
    /// The player prefs coin balance key.
    /// </summary>
    public const string CoinBalanceKey = "CoinBalance";

    /// <summary>
    /// The coin balance.
    /// </summary>
    private static int balance;

    /// <summary>
    /// Gets the coin balance.
    /// </summary>
    /// <returns>The coin balance.</returns>
    public static int GetCoinBalance() {
        // Get the balance if 0
        if (balance == 0)
            balance = PlayerPrefs.GetInt(CoinBalanceKey);

        return balance;
    }

    /// <summary>
    /// Sets the coin balance.
    /// </summary>
    /// <param name="amount">The amount to set the coin balance to.</param>
    public static void SetCoinBalance(int amount) {
        balance = amount;
        PlayerPrefs.SetInt(CoinBalanceKey, balance);
    }

    /// <summary>
    /// Adds the given number of coins to the users balance.
    /// </summary>
    /// <param name="amount">The amount of coins to add.</param>
    public static void AddCoins(int amount) {
        balance += amount;
        SetCoinBalance(balance);
    }
    
    /// <summary>
    /// Removes the given number of coins from the account if able.
    /// </summary>
    /// <param name="amount">The amount of coins to remove.</param>
    /// <returns>True if the transaction was successful, false if insufficient funds.</returns>
    public bool RemoveCoins(int amount) {
        if (amount > balance
            || balance <= 0
            || amount < 0)
            return false;
        balance -= amount;
        SetCoinBalance(balance);
        return true;
    }

}
