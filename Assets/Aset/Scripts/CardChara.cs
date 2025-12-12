using UnityEngine;

[CreateAssetMenu(menuName = "DataKartuCharacter")]
public class CardChara : ScriptableObject
{
    public string id;
    public string role;
    public string ability_as;
    public string efek;
    public Sprite cardfull;
    public Sprite karakter;
    public int health;
}
