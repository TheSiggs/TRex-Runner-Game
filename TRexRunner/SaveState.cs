using System;

namespace TRexRunner;

[Serializable]
public class SaveState
{
    public int HighScore { get; set; }
    public DateTime HighscoreDate { get; set; }
}