using System.Collections.Generic;
using UnityEngine;

public static class Achievements
{
    /// <summary>
    /// List of all achievements in the game as defined in the steam achievements configuration.
    /// </summary>
    private static readonly Dictionary<string, Achievement> achievementDatabase
        = new Dictionary<string, Achievement>()
        {
            ["GAME_START"] = new Achievement("ACH_GAME_STARTED", "Game Started", "Started the game.", Stat.None, 0),

            ["GAME_COMPLETE"] = new Achievement("ACH_GAME_COMPLETED", "Game Completed", "Finished the game.", Stat.None, 0),

            ["SKELETON_SLAYER_10"] = new Achievement("ACH_SKELETON_SLAYER", "Skeleton Slayer", "Killed a total of 10 skeletons.", Stat.ST_SKELETON_KILLS, 10),

            ["SKELETON_DESTROYER_20"] = new Achievement("ACH_SKELETON_DESTROYER", "Skeleton Destroyer", "Killed a total of 20 skeletons.", Stat.ST_SKELETON_KILLS, 20),

            ["PUZZLE_COMPLETED"] = new Achievement("ACH_PUZZLE_COMPLETED", "Puzzle Solver", "Solved a puzzle.", Stat.None, 0),

            ["PUZZLE_2_COMPLETED"] = new Achievement("ACH_PUZZLE_2_COMPLETED", "Puzzle Genius", "Solved all the puzzles.", Stat.None, 0),

            ["DEMON_SLAYER"] = new Achievement("ACH_DEMON_SLAYER", "Demon Slayer", "Killed the final boss.", Stat.None, 0)
        };

    public static Achievement Get(string achNameInDatabase)
    {
        if (achievementDatabase.TryGetValue(achNameInDatabase, out Achievement value))
        {
            return value;
        }

        Debug.LogError($"Achievement of name {achNameInDatabase} could not be found.");
        return null;
    }
}