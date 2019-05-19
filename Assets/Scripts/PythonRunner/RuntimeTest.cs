using System;
using UnityEngine;
using UnityEngine.UI;

using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace PythonRunner
{
    public class RuntimeTest : MonoBehaviour
    {
        public Button executeButton;
        public InputField codeField;
        public Text resultText;

        private void Start()
        {
            executeButton.onClick.AddListener(RunCodeFromField);
        }

        private void RunCodeFromField()
        {
            var engine = Python.CreateEngine();
            var scope = engine.CreateScope();
            engine.Execute(codeField.text);
            var pythonResult = scope.GetVariable<string>("result");
            resultText.text = pythonResult;
        }
        
    }
}