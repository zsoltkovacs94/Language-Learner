using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/*
 * Megkeresi a játék gyökérmappájában a csv fájlokat
 */
public class FileFinder
{
    // Visszaadandó fájlnevek listája
    private List<string> filenames;
    /*
     * Megkeresi az összes csv fájlt a játék gyökérmappájában
     */
    public FileFinder() 
    {
        List<string> everyFilename = Directory.GetFiles(Directory.GetCurrentDirectory()).ToList<string>();
        List<string> csvFilenames = new List<string>();
        foreach (string filename in everyFilename)
        {
            if (filename.Contains(".csv"))
            {
                List<string> words = new CSVHandler(filename).Read();
                int added = 0;
                foreach (string word in words)
                {
                    string[] splitted = word.Split(',');
                    if (splitted.Length == 2
                        && splitted[0] != null && splitted[1] != null && splitted[0].Trim() != "" && splitted[1].Trim() != "")
                    {
                        added++;
                    }
                }
                if(added > 0)
                {
                    csvFilenames.Add(filename);
                }
            }
        }
        filenames = csvFilenames;
    }
    // Fájlnevek visszaadása
    public List<string> getFilenames() { return filenames; }
}
