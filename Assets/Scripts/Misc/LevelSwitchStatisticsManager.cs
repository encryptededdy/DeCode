using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Misc
{
    public class LevelSwitchStatisticsManager: Singleton<LevelSwitchStatisticsManager>
    {
        // Prevent non-singleton constructor use
        protected LevelSwitchStatisticsManager() { }

        private readonly int[] _levelTimes= {-1, -1, -1, -1, -1, -1};
        private readonly int[] _resetCounter= {0, 0, 0, 0, 0, 0};

        public void SwitchLevel(int thisLevelID, int nextLevelID)
        {
            int thisLevelTime = (int) Math.Round(Time.timeSinceLevelLoad);
            _levelTimes[thisLevelID] = thisLevelTime;
            Debug.Log($"Level {thisLevelID} took {thisLevelTime} seconds with {_resetCounter[thisLevelID]} resets.");
            // TODO: Transmit data async?
            SceneManager.LoadScene(nextLevelID);
        }

        public void LevelReset(int thisLevelID)
        {
            _resetCounter[thisLevelID]++;
        }
    }
}