using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int startingWorkday;
    public int[] orderHighScores;
    public int[] packageHighScores;
    public int[] refundsHighScores;

    public GameData()
    {
        startingWorkday = GameManager.workDayActual;
        orderHighScores = GameManager.orderScores;
        packageHighScores = GameManager.packageScores;
        refundsHighScores = GameManager.refundsScores;
    }

    /// <summary>
    /// Sets the order score of a particular workday
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_score"></param>
    public void SetOrderScore(int _index, int _score)
    {
        orderHighScores[_index] = _score;
    }
    /// <summary>
    /// sets the order scores of all workdays
    /// </summary>
    /// <param name="_scores"></param>
    public void SetAllOrderScores(int[] _scores)
    {
        orderHighScores = _scores;
    }
    /// <summary>
    /// sets the package score of a particular workday
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_score"></param>
    public void SetPackageHighScore(int _index, int _score)
    {
        packageHighScores[_index] = _score;
    }
    /// <summary>
    /// sets the package scores of all workdays
    /// </summary>
    /// <param name="_scores"></param>
    public void SetAllPackageScores(int[] _scores)
    {
        packageHighScores = _scores;
    }
    /// <summary>
    /// sets refund score of a particular workday
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_score"></param>
    public void SetRefundHighScore(int _index, int _score)
    {
        refundsHighScores[_index] = _score;
    }
    /// <summary>
    /// sets all refund scores of all workdays
    /// </summary>
    /// <param name="_scores"></param>
    public void SetAllRefundScores(int[] _scores)
    {
        refundsHighScores = _scores;
    }

}
