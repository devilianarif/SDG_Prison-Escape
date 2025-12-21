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
    public bool bufvdice; // jika chara ada ini hasil dice yg  dpilih abis reroldice ditmabah oleh value dice 
    public float valuedice;

    [Header("buff rerol card")]
    public bool bufrerolcard; // jika dpt rerolcard rol lagi 

    [Header("buff attack")]
    public bool buffattack; //jika dpt attack intinya yg kasih damage akan bertambah dengan damage disini
    public float valuedamage = 1;

    [Header("buff heal")]
    public bool buffheal;//jika dpt heal intinya yg kasih heal akan bertambah dengan heal disini ada 2 kondisi jika ke diri sendiri pakai value heal diri + efek card nya misal heal 1 chara ini ada buf jadi 2 jika ke lain jika ke diri jadi 3 gitu efekcard + buff intinya
    public float valuehealdiri = 2;
    public float valueheallain = 1;
}
