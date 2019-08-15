using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Misc
{
    public class LevelSwitchStatisticsManager: Singleton<LevelSwitchStatisticsManager>
    {
        // Prevent non-singleton constructor use
        protected LevelSwitchStatisticsManager() { }

        private readonly int[] _levelTimes = {-1, -1, -1, -1, -1, -1};
        private readonly int[] _resetCounter = {0, 0, 0, 0, 0, 0};
        private readonly int[] _questionCounter = {-2, -2, -2, -2, -2 ,-2, -2, -2, -2, -2, -2, -2};

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
        
        /**
         * Question ID Reference
         * - Array Level
         * 0: If we were to implement BubbleSort in code, how many other helper variables do we need?
         * 1: Given arrays of the same length, which type of array would take the maximum number of iterations
         * - List Level
         * 2: Just so we can tell how much you already know - if we consider the ArrayList implementation of a List, what's a difference between it and a plan Array
         * 3: If we need a list that can get larger, we could just implement it by having a massive array
         * 4: If we have a list that starts at size 0, and we add elements one-by-one until we reach size 100, how many cars do we end up having to perform the (relatively slow) copy operation on?
         * 5: If we perform the same operation (start at 0, add elements until we reach size 100) but we use doubling instead, how many cars do we end up copying?
         * - Queue Level
         * 6: To gauge your existing knowledge - If we had a fixed size array, and we were adding/removing elements to the front (index 0) and shifting things over to accomodate, what could we do to speed this process up?
         * 7: We enqueued A, then B, then C just before. Now if we dequeue, what element do we get back? Recall that a Queue is FIFO (First-in, First-out)
         * 8: What is a possible disadvantage of this naive implementation of a queue using an array that is copied every dequeue?
         * 9: Why are dequeues faster with this new, improved implementation?
         * - Stack Level
         * 10: We pushed A, then B, then C just before. Now if we pop, what element do we get back?
         * 11: Now let's consider performance. If we assume the size of the stack never exceeds the size of the array (i.e. we never have to expand the array), can we make this stack implementation any faster?
         */

        public void QuestionReturn(int questionID, int tries)
        {
            Debug.Log($"Question {questionID} took {tries} attempts.");
            _questionCounter[questionID] = tries;
        }

        public int[] LevelTimes => _levelTimes;

        public int[] ResetCounter => _resetCounter;

        public int[] QuestionCounter => _questionCounter;
    }
}