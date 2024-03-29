using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LearnedCharManager
{
    public void Add(List<string> newChars)
    {
        StreamReader sr = new StreamReader("charlog");
        List<string> lines = new List<string>();
        while (!sr.EndOfStream) 
        { 
            lines.Add(sr.ReadLine());
        }
        sr.Close();
        List<string> addLater = lines;
        bool learned = false;
        foreach (string c in newChars)
        {
            foreach (string learnedChar in lines)
            {
                if (learnedChar[0] == c[0])
                {
                    learned = true;
                    break;
                }
            }
            if(!learned)
            {
                addLater.Add(c);
            }
            else
            {
                learned = false;
            }
        }
        StringBuilder sb = new StringBuilder();
        foreach (string line in addLater)
        {
            sb.Append(line).Append("\n");
        }
        StreamWriter sw = new StreamWriter("charlog");
        sw.Write(sb.ToString());
        sw.Close();
        PlayerPrefs.SetInt("CharactersLearned",addLater.Count);
    }
    public void CheckCharLog()
    {
        if (!File.Exists(Directory.GetCurrentDirectory() + "\\charlog"))
        {
            File.Create(Directory.GetCurrentDirectory() + "\\charlog");
        }
    }
    public void Reset()
    {
        StreamWriter sw = new StreamWriter("charlog");
        sw.Write("");
        sw.Close();
    }
}
