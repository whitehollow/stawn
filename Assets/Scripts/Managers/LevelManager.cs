using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance {get; private set;}
    public Player Player {get; private set;}
    public CameraController Camera {get; private set;}
    public CheckPoint DebugSpawn;
    public TimeSpan RunningTime { get { return DateTime.UtcNow - started; } }

    public int CurrentTimeBonus
    {
        get
        {
            var secondDifference = (int)(BonusCutOffSeconds - RunningTime.TotalSeconds);
            return Mathf.Max(0, secondDifference) * BonusSecondMultiplier;
        }
    }
    
    public int BonusCutOffSeconds;
    public int BonusSecondMultiplier;

    private List<CheckPoint> checkpoints;
    private DateTime started;
    private int currentCheckPointIndex;
    private int savedPoints;
    private int _currentScore = 0;


    public void Awake(){
        savedPoints = GameManager.Instance.Points;
        Instance = this;
    }

    public void Start()
    {
        checkpoints = FindObjectsOfType<CheckPoint>().OrderBy(t => t.transform.position.x).ToList();
        currentCheckPointIndex = checkpoints.Count > 0 ? 0 : -1;

        Player = FindObjectOfType<Player>();
        Camera = FindObjectOfType<CameraController>();

        started = DateTime.UtcNow;

        var listener = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();

        foreach (var listen in listener)
        {
            for (var i = checkpoints.Count - 1; i >= 0; i--)
            {
                var distance = ((MonoBehaviour)listen).transform.position.x - checkpoints[i].transform.position.x;
                if (distance < 0)
                    continue;
                checkpoints[i].AssignObjectToCheckPoint(listen);
                break;
            }
        }

#if UNITY_EDITOR

        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(Player);
        else if (currentCheckPointIndex != -1)
            checkpoints[currentCheckPointIndex].SpawnPlayer(Player);
#else
        if(currentCheckPointIndex != -1)
            checkpoints[currentCheckPointIndex].SpawnPlayer(Player);
#endif
    }

    public void Update()
    {
        var isAtLastCheckpoint = currentCheckPointIndex + 1 >= checkpoints.Count;

        if (isAtLastCheckpoint)
            return;

        var distanceToNextCheckpoint = checkpoints[currentCheckPointIndex + 1].transform.position.x - Player.transform.position.x;

        if (distanceToNextCheckpoint >= 0)
            return;

        checkpoints[currentCheckPointIndex].PlayerLeftCheckPoint();
        currentCheckPointIndex++;
        checkpoints[currentCheckPointIndex].PlayerCheckPointHit();

        GameManager.Instance.AddPoints(CurrentTimeBonus);
        savedPoints = GameManager.Instance.Points;

        started = DateTime.UtcNow;

    }

    public void GoToNextLevel(string level)
    {
        StartCoroutine(GoToNextLevelCo(level));
    }

    private IEnumerator GoToNextLevelCo(string level)
    {
        Player.FinishLevel();
        GameManager.Instance.AddPoints(CurrentTimeBonus);

        FloatingText.Show("Level Complete !", "PointDiamondText", new CenterTextPositioner(.2f));
        yield return new WaitForSeconds(2);

        FloatingText.Show(string.Format("{0}", GameManager.Instance.Points), "PointDiamondText", new CenterTextPositioner(.2f));
        yield return new WaitForSeconds(5);

        if (string.IsNullOrEmpty(level))
            Application.LoadLevel("StartScreen");
        else
            Application.LoadLevel(level);
    }


    public void KillPlayer(){
        StartCoroutine(KillPlayerCo());
    }
    private IEnumerator KillPlayerCo(){
        Player.Kill();
        Camera.isFollowing = false;
        yield return new WaitForSeconds(2f);

        Camera.isFollowing = true;

        if (currentCheckPointIndex != -1)
            checkpoints[currentCheckPointIndex].SpawnPlayer(Player);

        started = DateTime.UtcNow;
        GameManager.Instance.ResetPoints(savedPoints);
    }

}

