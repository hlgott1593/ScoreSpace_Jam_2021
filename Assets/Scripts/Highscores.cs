using System.Collections.Generic;


[System.Serializable]
public class ScoreData
{
    public Score highscore;
    public List<Score> scores;
}


[System.Serializable]
public class Score
{
    public string player;
    public int value;
}