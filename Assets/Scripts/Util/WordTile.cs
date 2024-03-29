using UnityEngine;
/*
 * Seg�doszt�ly WordTile reprezent�l�s�ra
 */
public class WordTile
{
    // Megjelen�tend� karakter
    private string character;
    // Megjelen�tend� kiejt�s
    private string pronunciation;
    // Kiejt�s �s karakter inicializ�l�sa
    public WordTile(string character, string pronunciation)
    {
        this.character = character;
        this.pronunciation = pronunciation;
    }
    // Karakter visszaad�sa
    public string GetCharacter() { return character; }
    // Kiejt�s visszaad�sa
    public string GetPronunciation() {  return pronunciation; }
}
