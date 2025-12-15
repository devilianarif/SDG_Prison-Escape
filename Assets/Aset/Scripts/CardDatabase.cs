using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    [Header("Effect Card (CardData)")]
    public CardData[] allCards;

    [Header("Character Card (CardChara)")]
    public CardChara[] allCharacterCards;

    public CardData GetCard(string id)
    {
        foreach (var c in allCards)
        {
            if (c.id == id) return c;
        }
        return null;
    }

    public CardChara GetCharacter(int index)
    {
        if (index < 0 || index >= allCharacterCards.Length) return null;
        return allCharacterCards[index];
    }
}
