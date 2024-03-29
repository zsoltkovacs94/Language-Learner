using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/*
 * Megkeresi a j�t�k gy�k�rmapp�j�ban a csv f�jlokat
 */
public class FileFinder
{
    // Visszaadand� f�jlnevek list�ja
    private List<string> filenames;
    /*
     * Megkeresi az �sszes csv f�jlt a j�t�k gy�k�rmapp�j�ban
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
    // F�jlnevek visszaad�sa
    public List<string> getFilenames() { return filenames; }
}
