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
    public string buffname;
    public int coldoncharabuf = 3;
    [Header("buff dice")]
    public bool bufvdice; 
    public float valuedice;

    [Header("buff rerol card")]
    public bool bufrerolcard; 

    [Header("buff attack")]
    public bool buffattack; 
    public float valuedamage = 1;

    [Header("buff heal")]
    public bool buffheal;
    public float valuehealdiri = 2;
    public float valueheallain = 1;
}
