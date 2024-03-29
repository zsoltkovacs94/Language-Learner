using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/*
 * CSV fájlok olvasásáért és írásáért felelõs osztály
 * string listákkal dolgozik
 */

public class CSVHandler
{
    // Beolvasandó fájl neve
    private string filename;

    // Inicializálja a beolvasandó fájl nevét
    public CSVHandler(string filename)
    {
        this.filename = filename;
    }

    /*
     * Beolvassa a megadott fájl tartalmát és string listaként visszaadja a sorokat
     */
    public List<string> Read()
    {
        StreamReader sr = new StreamReader(filename);
        List<string> lines = new List<string>();
        while (!sr.EndOfStream) 
        { 
            lines.Add(sr.ReadLine());
        }
        sr.Close();
        return lines;
    }
    /*
     * Az átadott string lista tartalmát kiírja a megadott fájlba
     */
    public void Write(List<string> lines)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string line in lines)
        {
            sb.Append(line).Append("\n");
        }
        StreamWriter sw = new StreamWriter(filename);
        sw.Write(sb.ToString());
        sw.Close();
    }
}
