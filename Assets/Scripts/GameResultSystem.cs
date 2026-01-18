using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultSystem : MonoBehaviour
{
    [Header("Points Settings")]
    public float pointsForAccuracy = 500f;
    public float pointsForSecond = 10f;

    [Header("Sallary Thresholds")]
    public int moneyThreshold = 600;
    public int silverThreshold = 300;

    public void CountResults(float remainingTime, int numberOfErrors, int textLenght, bool onTime)
    {
        int madeMoney = 0;

        if (!onTime)
        {
            madeMoney = 0;
        }
        else
        {
            if (textLenght == 0) textLenght = 1; 
            float accuracy = (float)(textLenght - numberOfErrors) / textLenght;
            if (accuracy < 0) accuracy = 0;

            float scoreForAccuracy = accuracy * pointsForAccuracy;
            float scoreForTime = remainingTime * pointsForSecond;
            float result = scoreForAccuracy + scoreForTime;


            if (result >= moneyThreshold) madeMoney = 100;
            else if (remainingTime >= silverThreshold) madeMoney = 50;
            else madeMoney = 25;
        }

        ClientManager.WonCoins = madeMoney;
        ClientManager.ReturningFromGame = true;

        SceneManager.LoadScene("MainGameScene");
    }
}