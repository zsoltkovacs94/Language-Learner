using UnityEngine;
/*
 * Segédosztály WordTile reprezentálására
 */
public class WordTile
{
    // Megjelenítendõ karakter
    private string character;
    // Megjelenítendõ kiejtés
    private string pronunciation;
    // Kiejtés és karakter inicializálása
    public WordTile(string character, string pronunciation)
    {
        this.character = character;
        this.pronunciation = pronunciation;
    }
    // Karakter visszaadása
    public string GetCharacter() { return character; }
    // Kiejtés visszaadása
    public string GetPronunciation() {  return pronunciation; }
}
