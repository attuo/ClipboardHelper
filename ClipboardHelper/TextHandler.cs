using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ClipboardHelper
{
    /// <summary>
    /// This is where the text and list handling happens. In the future this needs 
    /// to be refactored to MVVM pattern.
    /// </summary>
    class TextHandler
    {
        private int maxListLength = 10;
        string directory = @"C:\Users\Public\CopiedContextFolder";
        string filename = "copiedcontexts.json";

        /// <summary>
        /// Checks if directory and file exists for json file.
        /// </summary>
        public void CreateJsonFile()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string path = Path.Combine(directory, filename);
            if (!File.Exists(path))
            {
                File.Create(path).Close(); // need to remember to close
            }
        }


        /// <summary>
        /// Adds new text for the list, puts it in the first index,
        /// also checks plenty of things.
        /// </summary>
        /// <param name="text">Text which will be added, (the text which user has copied in clipboard</param>
        public void AddJson(String text)
        {
            List<string> list = new List<string>();
            if (GetCopiedJsonTexts() != null) // If there is already text in the json file
            {
                list = GetCopiedJsonTexts(); // Gets those texts in list
                if (!CheckIfHasAlready(list, text)) // Checks if the text user has in clipboard is already in file 
                {
                    if (list.Count == maxListLength) // If list's length is same as the maximum setting
                    {
                        MoveTextToFirst(list, text); // Moves the added text to first index, the last index one get deleted.
                    }

                    if (list.Count < maxListLength) // If list's length is lower than the max setting.
                    {
                        addToFirst(list, text); // Adds the text in the first index and also makes list 1 bigger
                    }
                }

            }
            WriteToJson(list); // Overwrites the current json with the new one.
            
        }


        /// <summary>
        /// Overwrites the json file and puts the list in there.
        /// </summary>
        /// <param name="list">list which will be put in the json file</param>
        public void WriteToJson(List<String> list)
        {
            string path = Path.Combine(directory, filename);
            // serialize JSON to a string and then write string to a file
            File.WriteAllText(path, JsonConvert.SerializeObject(list));
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, list);

            }

        }

        /// <summary>
        /// This is for the state that if the list length isn't yet same as the maxlength
        /// Works but maybe not the best code..
        /// </summary>
        /// <param name="list">Current list where the copied text are</param>
        /// <param name="text">new copied text from clipboard which will be added to first index</param>
        /// <returns>List which has the new text in first index and other indexes are shifted 1 to right (list length is 1 bigger)</returns>
        public List<String> addToFirst(List<String> list, String text)
        {
            if (list.Count < 1) // this is for the state if the list is empty (maybe could be in some other place?)
            {
                list.Add(text);
                return list;
            }

            String lastText = list[(list.Count - 1)]; // We need to add that last index later when list shifted right.
            list = MoveTextToFirst(list, text); // shifts list to 1 right and puts the text in first index
            list.Add(lastText); // Adds the last index to new last.
            return list;

        }

        /// <summary>
        /// Shifts indexes to 1 right and adds new text in first index (0), last index gets deleted
        /// </summary>
        /// <param name="list">List which will be shifted (the current list of copied texts)</param>
        /// <param name="text">Text which will be added to first index (new users text from clipboard)</param>
        /// <returns>List that is shifted 1 to right and the new text is added to first index.</returns>
        public List<String> MoveTextToFirst(List<String> list, String text)
        {
            for (int i = list.Count - 2; i >= 0; i--)
            {
                list[i + 1] = list[i];
            }
            list[0] = text;
            return list;
        }


        /// <summary>
        /// Handles the users setting to change how long the clipboard history it collects. (Default 10)
        /// </summary>
        /// <param name="newLength">New lenght, user can select from 5, 10, 15, 20</param>
        public void SetMaxListLength(int newLength)
        {
            List<String> list = GetCopiedJsonTexts(); 
            if (list.Count > newLength) // This is for if user desides to shorten the list from current state (example: from 15 to 5)
            {
                MakeListShorter(list, newLength); // Will make list shorter if user selects (example: from 15 to 5) it makes it 5 length
            }
            this.maxListLength = newLength; // Sets the new max length

        }

        /// <summary>
        /// Will make list shorter so it is as short as the current maxlength
        /// </summary>
        /// <param name="list">List which needs to be shorter</param>
        /// <param name="newLength">New length that will be maxlength</param>
        public void MakeListShorter(List<String> list, int newLength)
        {
            int count = list.Count - newLength;
            list.RemoveRange(newLength - 1, count); 
            WriteToJson(list); // It needs to write the json file again so it overwrites it with the new shorter list.
        }


        /// <summary>
        /// Checks if the textfile already contains that line of text. 
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Returns true if there is already same line of text, false if not.</returns>
        public bool CheckIfHasAlready(List<string> list, String text)
        {
            foreach (string currentText in list)
            {
                if (currentText == text)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This is the reader for json file. 
        /// </summary>
        /// <returns>List of texts from json file</returns>
        public List<String> GetCopiedJsonTexts()
        {
            string path = Path.Combine(directory, filename);
            List<string> list = new List<string>();
            string text = File.ReadAllText(path);
            list = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(path));
            return list;
        }
    }
}
