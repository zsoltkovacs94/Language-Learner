using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * A kiv�lasztott sz�t�rf�jlok tartalm�t reprezent�lja
 */
public class DictionaryHandler
{
    // J�t�kban haszn�land� szavak
    public Dictionary<string, string> dictionary;
    // J�t�k mapp�j�ban l�v� csv f�jlok
    public string[] filenames;
    // Kiv�lasztott csv f�jlok
    public bool[] selected;
    // Inicializ�lja a filenames-t a csv f�jlok neveivel
    public DictionaryHandler()
    {
        setFileList();
    }
    /*
     * Beolvassa a kiv�lasztott csv f�jlok tartalm�t, de csakis a helyes form�tum� sorokat
     */
    public void FillDictionary()
    {
        dictionary = new Dictionary<string, string>();
        if(selected == null)
        {
            return;
        }
        for (int i = 0;i < selected.Length;i++) 
        {
            if (selected[i])
            {
                List<string> words = new CSVHandler(filenames[i]).Read();
                foreach (string word in words) 
                {
                    string[] splitted = word.Split(',');
                    if (!dictionary.ContainsKey(splitted[0]) 
                        && splitted.Length == 2
                        && splitted[0] != null && splitted[1] != null && splitted[0].Trim() != "" && splitted[1].Trim() != "")
                    {
                        dictionary.Add(splitted[0], splitted[1]);
                    }
                }
            }
        }
    }
    // Be�ll�tja a filenamest �s az �sszes f�jlt alap�rtelmezetten kiv�lasztja
    private void setFileList()
    {
        filenames = new FileFinder().getFilenames().ToArray();
        if( filenames.Length > 0 )
        {
            selected = new bool[filenames.Length];
            for (int i = 0; i < selected.Length; i++)
            {
                selected[i] = true;
            }
        }
        else
        {
            Debug.Log("No files found");
        }
    }
    // �jrakeresi a j�t�k mapp�j�ban tal�lhat� csv f�jlokat �s kiv�lasztja �ket
    public void RefreshFileList()
    {
        setFileList();
    }
    // Index alapj�n kiv�laszt egy f�jlt
    public void Select(int i)
    {
        selected[i] = true;
    }
    // F�jln�v alapj�n kiv�laszt egy f�jlt
    public void Select(string s)
    {
        for (int i = 0; i < filenames.Length; i++)
        {
            if (filenames[i] == s)
            {
                selected[i] = true;
                break;
            }
        }
    }
    // Index alapj�n kivesz egy f�jlt a kiv�laszt�sb�l
    public void DeSelect(int i)
    {
        selected[i] = false;
    }
    // Az �sszes f�jlt kiveszi a kiv�laszt�sb�l
    public void DeSelectAll()
    {
        if(selected == null)
        { 
            return;
        }
        for (int i = 0; i < selected.Length; i++)
        {
            selected[i] = false;
        }
    }
    // Visszaadja a kiv�lasztott f�jlok neveinek list�j�t
    public List<string> GetSelected()
    {
        List<string> selectedFiles = new List<string>();
        if( selected == null ) 
        {
            return null;
        }
        for (int i = 0; i < selected.Length; i++)
        {
            if (selected[i])
            {
                selectedFiles.Add(filenames[i]);
            }
        }
        return selectedFiles;
    }
}
