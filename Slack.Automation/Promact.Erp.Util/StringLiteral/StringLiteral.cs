using Newtonsoft.Json;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util.StringConstants;
using System;
using System.Collections.Generic;
using System.IO;

namespace Promact.Erp.Util.StringLiteral
{
    public class StringLiteral : IStringLiteral
    {
        #region Private Variables
        private readonly string stringConstantJsonFilePath;
        private readonly IStringConstantRepository _stringConstant;
        private readonly string directoryPath;
        private readonly ISingletonStringLiteral _stringLiteralSingleton;
        private readonly string StringLiteralJson = "StringLiteral.json";
        #endregion

        #region Constructor
        public StringLiteral(IStringConstantRepository stringConstant, ISingletonStringLiteral stringLiteralSingleton)
        {
            _stringLiteralSingleton = stringLiteralSingleton;
            directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            stringConstantJsonFilePath = directoryPath + StringLiteralJson;
            _stringConstant = stringConstant;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to start file watcher
        /// </summary>
        public void CreateFileWatcher()
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = directoryPath;
            // Watch for changes in LastAccess and LastWrite times, and the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = StringLiteralJson;
            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChange);
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Method to create .json file of StringConstantRepository.cs and inirialize string literal
        /// </summary>
        public void OnInit()
        {
            File.WriteAllText(stringConstantJsonFilePath, GetStringToWriteInJsonFile(ReadJsonFileAndDeserialize(stringConstantJsonFilePath)));
            ReadDataFromJsonAndInitializeInStringLiteral();
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Method used to update data of StringConstantRepository.cs
        /// </summary>
        /// <param name="source">object</param>
        /// <param name="e">FileSystemEventArgs</param>
        private void OnChange(object source, FileSystemEventArgs e)
        {
            ReadDataFromJsonAndInitializeInStringLiteral();
        }

        /// <summary>
        /// Method to read data from json file and initialize in string literal
        /// </summary>
        private void ReadDataFromJsonAndInitializeInStringLiteral()
        {
            var stringConstant = ReadJsonFileAndDeserialize(stringConstantJsonFilePath);
            var stringConstantJsonObject = JsonConvert.SerializeObject(stringConstant);
            var stringConstantRepository = JsonConvert.DeserializeObject<AppStringLiteral>(stringConstantJsonObject);
            _stringLiteralSingleton.Initialize(stringConstantRepository);
        }

        /// <summary>
        /// Method used to read the data from .json file and return a list of variables with key
        /// </summary>
        /// <param name="path">Path of the file to be read</param>
        /// <returns>list of variables</returns>
        private Dictionary<string, string> ReadJsonFileAndDeserialize(string path)
        {
            Dictionary<string, string> stringConstant = new Dictionary<string, string>();
            var read = File.ReadAllText(path);
            StringConstantJsonAC jsonStringConstant = new StringConstantJsonAC();
            jsonStringConstant = JsonConvert.DeserializeObject<StringConstantJsonAC>(read);
            if (jsonStringConstant != null)
            {
                stringConstant = jsonStringConstant.CommonString;
                foreach (var item in jsonStringConstant.Leave)
                {
                    stringConstant.Add(item.Key, item.Value);
                }
                foreach (var item in jsonStringConstant.TaskMail)
                {
                    stringConstant.Add(item.Key, item.Value);
                }
                foreach (var item in jsonStringConstant.Scrum)
                {
                    stringConstant.Add(item.Key, item.Value);
                }
                foreach (var item in jsonStringConstant.User)
                {
                    stringConstant.Add(item.Key, item.Value);
                }
            }
            return stringConstant;
        }

        /// <summary>
        /// Method to read the data from both json and cs and compare and create update string to write in json file
        /// </summary>
        /// <param name="stringLiteral">json string of updated string constant</param>
        /// <returns></returns>
        private string GetStringToWriteInJsonFile(Dictionary<string, string> stringLiteral)
        {
            var read = File.ReadAllText(stringConstantJsonFilePath);
            StringConstantJsonAC jsonStringConstant = JsonConvert.DeserializeObject<StringConstantJsonAC>(read);
            if(jsonStringConstant == null)
                jsonStringConstant = new StringConstantJsonAC();
            foreach (var constant in _stringConstant.GetType().GetProperties())
            {
                if (!stringLiteral.ContainsKey(constant.Name))
                {
                    var variableName = constant.Name;
                    var userFlag = variableName.Contains("User");
                    var scrumFlag = variableName.Contains("Scrum");
                    var taskFlag = variableName.Contains("Task");
                    var leaveFlag = variableName.Contains("Leave");
                    var redmineFlag = variableName.Contains("Redmine");
                    if (userFlag)
                    {
                        jsonStringConstant.User.Add(constant.Name, constant.GetValue(_stringConstant).ToString());
                    }
                    else if (leaveFlag)
                    {
                        jsonStringConstant.Leave.Add(constant.Name, constant.GetValue(_stringConstant).ToString());
                    }
                    else if (taskFlag)
                    {
                        jsonStringConstant.TaskMail.Add(constant.Name, constant.GetValue(_stringConstant).ToString());
                    }
                    else if (scrumFlag)
                    {
                        jsonStringConstant.Scrum.Add(constant.Name, constant.GetValue(_stringConstant).ToString());
                    }
                    else if (redmineFlag)
                    {
                        jsonStringConstant.Redmine.Add(constant.Name, constant.GetValue(_stringConstant).ToString());
                    }
                    else
                        jsonStringConstant.CommonString.Add(constant.Name, constant.GetValue(_stringConstant).ToString());
                }
            }
            return JsonConvert.SerializeObject(jsonStringConstant);
        }
        #endregion
    }
}
