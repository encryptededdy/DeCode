using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Misc
{
    public class LevelSwitchManager: Singleton<LevelSwitchManager>
    {
        // Prevent non-singleton constructor use
        protected LevelSwitchManager() { }

        //private Stopwatch _stopwatch = new Stopwatch();
        private readonly int[] _levelTimes= new int[]{-1, -1, -1, -1, -1, -1};

        public void SwitchLevel(int thisLevelID, int nextLevelID)
        {
            int thisLevelTime = (int) Math.Round(Time.timeSinceLevelLoad);
            _levelTimes[thisLevelID] = thisLevelTime;
            Debug.Log($"Level {thisLevelID} took {thisLevelTime} seconds.");
            // TODO: Transmit data async?
            SceneManager.LoadScene(nextLevelID);
        }
    }
}