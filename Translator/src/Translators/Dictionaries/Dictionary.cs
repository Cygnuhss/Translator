﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Translate.Translators.Utilities;

namespace Translate.Translators.Dictionaries
{
    class Dictionary : TranslatorObject
    {
        protected string sourceLanguage;
        protected string targetLanguage;
        protected string fileName;

        // A dictionary is a list of tuples containing the word in the source language
        // as a string and the translation of it in the target language as a string.
        IList<Tuple<string, string>> entries;

        public Dictionary(string sourceLanguage, string targetLanguage, string id = "")
            : base(id)
        {
            this.sourceLanguage = sourceLanguage;
            this.targetLanguage = targetLanguage;

            // File name is of this format: dict_[sourcelanguage]_[targetlanguage].txt
            this.fileName = string.Format("dict_{0}_{1}.txt", sourceLanguage, targetLanguage);

            entries = new List<Tuple<string, string>>();
            Load();
        }

        public void Load()
        {
            string cd = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string path = Path.Combine(cd, @"res\Dictionaries", fileName);

            Console.WriteLine("Loading dictionary: {0}", fileName);

            try
            {
                string[] lines = File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    string[] pair = line.Split('\t');
                    this.entries.Add(new Tuple<string, string>(pair[0], pair[1]));
                }

                Console.WriteLine("Dictionary loaded: {0}", fileName);
            } catch (IOException)
            {
                Console.WriteLine("File not found: {0}. Dictionary not loaded.", fileName);
            }
        }

        public void Update()
        {
            string cd = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string path = Path.Combine(cd, @"res\Dictionaries", fileName);

            Console.WriteLine("Updating dictionary: {0}", fileName);

            RemoveDuplicates();

            int newLines = entries.Count;
            try
            {
                // Adding the entries in dictionary to the text file.
                string[] lines = File.ReadAllLines(path);
                newLines -= lines.Length;
            }
            catch (IOException)
            {
                Console.WriteLine("File not found: {0}. Creating a new dictionary.", fileName);
            }
            finally
            {     
                // Appending each new entry to the text file.
                for (int i = entries.Count - newLines; i < entries.Count; i++)
                {
                    string sourceWord = entries[i].Item1;
                    string targetWord = entries[i].Item2;
                    AddEntryToFile(sourceWord, targetWord, fileName, path);
                }

                Console.WriteLine("Dictionary updated: {0}", fileName);
            }
        }

        public void RemoveDuplicates()
        {
            // Make sure that even though multiple identical entries might be added,
            // only one will remain.
            entries = new List<Tuple<string, string>>(new HashSet<Tuple<string, string>>(entries));
        }

        public string Standardize(string s)
        {
            s = s.ToLower();
            return s;
        }

        public void AddEntry(string sourceWord, string targetWord)
        {
            // Add the entry to the list.
            this.entries.Add(new Tuple<string, string>(Standardize(sourceWord), Standardize(targetWord)));
        }

        public void AddEntryToFile(string sourceWord, string targetWord, string fileName, string path)
        {
            // Write all of the entries to the file.
            string entry = sourceWord + '\t' + targetWord;

            Console.WriteLine("Adding entry: {0} to dictionary {1}", entry, fileName);

            using (StreamWriter w = File.AppendText(path))
                w.WriteLine(entry);

            Console.WriteLine("Entry added: {0} to dictionary {1}", entry, fileName);
        }

        public Tuple<string, string> Languages
        {
            get { return new Tuple<string, string>(sourceLanguage, targetLanguage); }
        }

        public IList<Tuple<string, string>> Entries
        {
            get { return entries; }
        }
    }
}