using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameTime : MonoBehaviour
{
    [Serializable]
    public struct TimeAndLevel
    {
        /// <summary>
        /// Level 
        /// </summary>
        public int Level;

        /// <summary>
        /// Seconds to complete level
        /// </summary>
        public float TimeInSeconds;
    }

    [SerializeField]
    List<TimeAndLevel> levelSplits;

    [SerializeField]
    MapGenerator generator;

    [SerializeField]
    TMP_Text textBox;

    float elapsed;


    // Start is called before the first frame update
    void Start()
    {
        levelSplits.Sort((a, b) => a.Level.CompareTo(b.Level));
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        //TODO: Pausing?
        elapsed += Time.deltaTime;
        UpdateTime();
    }

    void UpdateTime()
    {
        float maxTime = GetTimeForLevel();
        float remaining = maxTime - elapsed;
        if (remaining < 0)
        {
            remaining = 0;
        }

        int minutes = (int)(remaining / 60);
        int seconds = (int)(remaining % 60);
        int milliseconds = (int)((remaining % 1) * 1000);

        TimeSpan time = new TimeSpan(0, 0, minutes, seconds, milliseconds);

        string timeLeft;
        if (time.TotalSeconds < 10)
        {
            timeLeft = time.ToString(@"ss\.f");
        }
        else
        {
            timeLeft = time.ToString(@"mm\:ss");
        }

        textBox.text = timeLeft;
    }

    float GetTimeForLevel()
    {
        int level = generator.Level;
        for (int i = 0; i < levelSplits.Count; i++)
        {
            if (level < levelSplits[i].Level)
            {
                return levelSplits[i].TimeInSeconds;
            }
        }

        return levelSplits[levelSplits.Count - 1].TimeInSeconds;
    }

    public void ResetTime()
    {
        elapsed = elapsed < 0 ? elapsed : 0;
        UpdateTime();
    }

    public void AddTime(float timeToAdd)
    {
        elapsed -= timeToAdd;
        UpdateTime();
    }
}
