// Copyright 2018 - 2020 Finch Technologies Ltd. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Finch;

public enum LanguageId
{
    En = 1
}

public class TextImporter : MonoBehaviour
{
    public static Font DefaultFont;

    public LanguageId Language = LanguageId.En;
    public TextAsset[] DataBases = new TextAsset[0];
    public Font TextFont;

    private static TextAsset[] dataBase = new TextAsset[0];
    private static Dictionary<string, string> phrases = new Dictionary<string, string>();

    private void Awake()
    {
        DefaultFont = TextFont;
        dataBase = DataBases;
        ImportData(Language);
    }

    public static void ImportData(LanguageId language)
    {
        phrases.Clear();

        foreach (var currentDataBase in dataBase)
        {
            string text = currentDataBase.text;

            string[] data = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < data.Length; i++)
            {
                string[] args = data[i].Split(';');

                if ((int)language < args.Length && !phrases.ContainsKey(args[0]))
                {
                    phrases.Add(args[0], args[(int)language].Replace("\\n","\n"));
                }
            }
        }
    }

    public static string GetPhrase<T>(T id)
    {
        if (phrases.ContainsKey(id.ToString()))
        {
            return phrases[id.ToString()];
        }

       //TODO:Fix this shit Debug.LogError("Phrase \"" + id.ToString() + "\" not found.");
        return "";
    }
}
