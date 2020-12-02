using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public StarType[] levelStars;
    public string date;

    public GameData ()
    {
        levelStars = new StarType[50];
        for (int i = 0; i < levelStars.Length; i++)
        {
            levelStars[i] = StarType.None;
        }

        date = "";
    }

    public GameData (GameData gameData)
    {
        levelStars = new StarType[50];
        for (int i = 0; i < levelStars.Length; i++)
        {
            levelStars[i] = gameData.levelStars[i];
        }
        date = gameData.date;
    }

    public bool IsSolvedCompletely()
    {
        for (int i = 0; i < 35; i++)
        {
            if (levelStars[i] != StarType.Gold)
                return false;
        }

        return true;
    }

    public bool IsSolvedLevelsForGettingSmarterAchievement()
    {
        if (levelStars[0] == StarType.Gold &&
            levelStars[1] == StarType.Gold &&
            levelStars[2] == StarType.Gold &&
            levelStars[3] == StarType.Gold &&
            levelStars[4] == StarType.Gold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsSolvedLevelsForHalfwayToSuccessAchievement()
    {
        if (levelStars[17] == StarType.Gold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsSolvedLevelsForUrbanistAchievement()
    {
        if (levelStars[3] == StarType.Gold &&
            levelStars[12] == StarType.Gold &&
            levelStars[14] == StarType.Gold &&
            levelStars[28] == StarType.Gold &&
            levelStars[30] == StarType.Gold &&
            levelStars[34] == StarType.Gold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

[System.Serializable]
public enum StarType
{
    None,
    Silver,
    Gold,
}
