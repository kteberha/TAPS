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

    public void SetOrderScore(int _index, int _score)
    {
        orderHighScores[_index] = _score;
    }

    public void SetAllOrderScores(int[] _scores)
    {
        orderHighScores = _scores;
    }

    public void SetPackageHighScore(int _index, int _score)
    {
        packageHighScores[_index] = _score;
    }

    public void SetAllPackageScores(int[] _scores)
    {
        packageHighScores = _scores;
    }

    public void SetRefundHighScore(int _index, int _score)
    {
        refundsHighScores[_index] = _score;
    }

    public void SetAllRefundScores(int[] _scores)
    {
        refundsHighScores = _scores;
    }
}
