    using UnityEngine;

    public enum CardType
    {
        Bad,
        Lucky,
        Skill
    }

    [CreateAssetMenu(menuName = "DataKartu")]
    public class CardData : ScriptableObject
    {
        public string id;
        public CardType cardType;
        public string nama;
        public int attack;
        public int heal;
        public Sprite spriteCard;
        public Sprite karakter;
        public string informasi;
        public string efekkartu;

        [Header("efek kartu pilih salah satu dan set coldon")]
        public int cooldownroundcard = 0;
        public bool isSkipCard;
        public bool isEfekReDice;
        public bool isRelifePlayer;
        public bool isRerolcard;



    }