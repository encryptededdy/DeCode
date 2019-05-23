using System;
using System.Text;
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
        public Text logText;
        public Text outputText;
        // TODO: Move these to a library or something, this is ugly.
        private const string PythonClassDefs = @"class AccessType:
    READ = 1
    WRITE = 2

class ListLogEntry:
    def __init__(self, index, value, accessType):
        self.index = index
        self.value = value
        self.accessType = accessType
    def __repr__(self):
        if (self.accessType == AccessType.READ):
            return ""Read index ""+str(self.index)+"" (value: ""+str(self.value)+"")""
        else:
            return ""Wrote index ""+str(self.index)+"" (value: ""+str(self.value)+"")""

class MonitoredList(list):
    def __init__(self, items):
        self.log = []
        for item in items:
            self.append(item)
    def append(self, value):
        list.append(self, value)
    def __setitem__(self, index, value):
        list.__setitem__(self, index, value)
        self.log.append(ListLogEntry(index, value, AccessType.WRITE))
    def __getitem__(self, index):
        value = list.__getitem__(self, index)
        self.log.append(ListLogEntry(index, value, AccessType.READ))
        return value
";

        private const string PythonExecutionCode = @"monitoredList = MonitoredList([10, 12, 18, 5, 17, 29, 18])
insertion_sort(monitoredList)
resultOutput = ""Output: ""+str(monitoredList)
resultLog = '\n'.join([ str(e) for e in monitoredList.log ])";

        private void Start()
        {
            executeButton.onClick.AddListener(RunCodeFromField);
        }

        private void RunCodeFromField()
        {
            var engine = Python.CreateEngine();
            var scope = engine.CreateScope();
            engine.CreateScriptSourceFromString(BuildCodeToExecute(codeField.text)).Execute(scope);
            var pythonResult = scope.GetVariable<string>("resultOutput");
            var pythonLog = scope.GetVariable<string>("resultLog");
            outputText.text = pythonResult;
            logText.text = pythonLog;
        }

        private string BuildCodeToExecute(string userInput)
        {
            var builder = new StringBuilder();
            builder.Append(PythonClassDefs);
            builder.AppendLine(userInput);
            builder.AppendLine(PythonExecutionCode);
            return builder.ToString();
        }
        
    }
}