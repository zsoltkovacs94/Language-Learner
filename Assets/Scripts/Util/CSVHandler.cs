using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/*
 * CSV f�jlok olvas�s��rt �s �r�s��rt felel�s oszt�ly
 * string list�kkal dolgozik
 */

public class CSVHandler
{
    // Beolvasand� f�jl neve
    private string filename;

    // Inicializ�lja a beolvasand� f�jl nev�t
    public CSVHandler(string filename)
    {
        this.filename = filename;
    }

    /*
     * Beolvassa a megadott f�jl tartalm�t �s string listak�nt visszaadja a sorokat
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
     * Az �tadott string lista tartalm�t ki�rja a megadott f�jlba
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
