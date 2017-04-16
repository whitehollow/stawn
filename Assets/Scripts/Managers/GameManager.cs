using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance ?? (_instance = new GameManager()); } }

    private GameManager() { }

    public int Points { get; private set; }

    public void Reset()
    {
        Points = 0;
    }

    public void ResetPoints(int points)
    {
        Points = points;
    }

    public void AddPoints(int points)
    {
        Points += points;
    }
}
