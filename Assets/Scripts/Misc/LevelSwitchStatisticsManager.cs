using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Misc
{
    public class LevelSwitchStatisticsManager: Singleton<LevelSwitchStatisticsManager>
    {
        // Prevent non-singleton constructor use
        protected LevelSwitchStatisticsManager() { }

        private const string ApiEndpoint = "https://decode-d2cec.firebaseapp.com/api/submit";
        private const string UserApiEndpoint = "https://decode-d2cec.firebaseapp.com/api/new-user";

        private readonly int[] _levelTimes = {-1, -1, -1, -1, -1, -1};
        private readonly int[] _resetCounter = {0, 0, 0, 0, 0, 0};
        private readonly int[] _questionCounter = {-2, -2, -2, -2, -2 ,-2, -2, -2, -2, -2, -2, -2, -2};
        private string _userId;
        private Boolean _userIdRequested = false;
        private Text _userIdTextView;

        public void SwitchLevel(int thisLevelId, int nextLevelId)
        {
            var thisLevelTime = (int) Math.Round(Time.timeSinceLevelLoad);
            _levelTimes[thisLevelId] = thisLevelTime;
            Debug.Log($"Level {thisLevelId} took {thisLevelTime} seconds with {_resetCounter[thisLevelId]} resets.");
            //Transmit data async
            if (nextLevelId != 0)
            {
                StartCoroutine(PostLevelData(thisLevelId, thisLevelTime, _resetCounter[thisLevelId], true));
            }
            else
            {
                StartCoroutine(PostLevelData(thisLevelId, thisLevelTime, _resetCounter[thisLevelId], false));
            }

            SceneManager.LoadScene(nextLevelId);
        }

        public void SendSubData(int thisLevelId, int resets, string subLevel, string log = "")
        {
            var thisLevelTime = (int) Math.Round(Time.timeSinceLevelLoad);
            StartCoroutine(PostSubLevelData(thisLevelId, resets, subLevel, thisLevelTime, log.Replace("\n", "\\n")));
        }

        public void LevelReset(int thisLevelId)
        {
            _resetCounter[thisLevelId]++;
        }

        public void UpdateUserId(Text textView)
        {
            _userIdTextView = textView;
            if (_userIdRequested == false)
            {
                _userIdRequested = true;
                StartCoroutine(RequestUserID());
            } else if (_userId != null)
            {
                _userIdTextView.text = $"ID: {_userId}";
            }
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
            //Transmit data async
            StartCoroutine(PostQuestionData(questionID, tries));
        }

        public int[] LevelTimes => _levelTimes;

        public int[] ResetCounter => _resetCounter;

        public int[] QuestionCounter => _questionCounter;

        IEnumerator RequestUserID()
        {
            var uwr = UnityWebRequest.Get(UserApiEndpoint);

            // Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
                if (uwr.downloadHandler.text.Length > 0)
                {
                    _userId = uwr.downloadHandler.text;
                    _userIdTextView.text = $"ID: {_userId}";
                }
            }
        }

        IEnumerator PostSubLevelData(int level, int resets, string subLevel, int time, string log)
        {
            var json = _userId != null ? $"{{ \"user\": \"{_userId}\", \"type\": \"level\", \"id\": {level}, \"reset\": {resets}, \"subLevel\": \"{subLevel}\", \"time\": {time}, \"log\": \"{log}\" }}" : $"{{ \"type\": \"level\", \"id\": {level}, \"reset\": {resets}, \"subLevel\": \"{subLevel}\", \"time\": {time}, \"log\": \"{log}\" }}";
            var uwr = new UnityWebRequest(ApiEndpoint, "POST");
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending: " + json);

            // Send the request then wait here until it returns
            yield return uwr.SendWebRequest();
            
            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
            }
        }

        IEnumerator PostQuestionData(int id, int attempts)
        {
            var json = _userId != null ? $"{{ \"user\": \"{_userId}\", \"type\": \"question\", \"id\": {id}, \"attempts\": {attempts} }}" : $"{{ \"type\": \"question\", \"id\": {id}, \"attempts\": {attempts} }}";
            var uwr = new UnityWebRequest(ApiEndpoint, "POST");
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            // Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
            }
        }
        
        IEnumerator PostLevelData(int level, int timeTaken, int resets, bool complete)
        {
            var json = _userId != null ? $"{{ \"user\": \"{_userId}\", \"type\": \"level\", \"id\": {level}, \"time\": {timeTaken}, \"reset\": {resets}, \"complete\": {complete.ToString().ToLower()} }}" : $"{{ \"type\": \"level\", \"id\": {level}, \"time\": {timeTaken}, \"reset\": {resets}, \"complete\": {complete.ToString().ToLower()} }}";
            var uwr = new UnityWebRequest(ApiEndpoint, "POST");
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            // Send the request then wait here until it returns
            yield return uwr.SendWebRequest();
            
            Debug.Log("Sent: " + json);

            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
            }
        }

    }
}