using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState")]
public class PlayerState : ScriptableObject
{
    public int[] selectedCharacter = new int[4];

    public bool hasBackstep;
    [System.Serializable]
    public class PlayerData
    {
        public int characterIndex; // index karakter yang dipilih
        public string playername;
        public int health = 5; // health awal
        public int lastDiceResult; // hasil dadu terakhir
        public string lastTypeCard; // tipe kartu terakhir
        public string lastScannedCardID; // ID kartu terakhir yang discan
    }



    [System.Serializable]
    public class PlayerBackup
    {
        public int health;
        public int lastDiceResult;
        public string lastTypeCard;
        public string lastScannedCardID;

        public void CopyFrom(PlayerData p)
        {
            health = p.health;
            lastDiceResult = p.lastDiceResult;
            lastTypeCard = p.lastTypeCard;
            lastScannedCardID = p.lastScannedCardID;
        }

        public void CopyTo(PlayerData p)
        {
            p.health = health;
            p.lastDiceResult = lastDiceResult;
            p.lastTypeCard = lastTypeCard;
            p.lastScannedCardID = lastScannedCardID;
        }
    }
    public PlayerBackup[] backup = new PlayerBackup[4];


    [System.Serializable]
    public class PoliceData
    {
        public int lastDiceResult;
        public string laststepwhellValue;
        public bool isWheel = false;
    }
    public PoliceData[] polices = new PoliceData[4];
    public PlayerData[] players = new PlayerData[4];
    public int currentTurn = 1;
    public int currentPlayerIndex = 0;
    public bool IsPoliceTurn()
    {
        return currentTurn >= 4 && currentPlayerIndex == 4;
    }

    public bool IsPlayerTurn()
    {
        return currentPlayerIndex >= 0 && currentPlayerIndex <= 3;
    }

    public void ResetPlayerData()
    {
        if (players == null || players.Length != 4)
        {
            players = new PlayerData[4];
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new PlayerData();
            players[i].characterIndex = selectedCharacter[i];
            players[i].health = 5;
            players[i].lastDiceResult = 0;
            players[i].lastTypeCard = string.Empty;
            players[i].lastScannedCardID = string.Empty;
        }

        currentTurn = 1;
        currentPlayerIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            backup[i] = new PlayerBackup();
            backup[i].CopyFrom(players[i]);
        }
        for (int i = 0; i < polices.Length; i++)
            polices[i] = new PoliceData();

    }




    public void NextPlayer()
    {
        hasBackstep = false;
        int safety = 0;

        while (safety < 20)
        {
            safety++;

            if (currentTurn < 4)
            {
                currentPlayerIndex++;

                if (currentPlayerIndex >= 4)
                {
                    currentPlayerIndex = 0;
                    currentTurn++;
                }
            }
            else
            {
                currentPlayerIndex++;

                if (currentPlayerIndex > 4)
                {
                    currentPlayerIndex = 0;
                    currentTurn++;
                }
            }

            // skip player mati
            if (IsPlayerTurn() && IsPlayerDead(currentPlayerIndex))
                continue;

            break;
        }

        // RESET NILAI AKSI UNTUK PLAYER BARU
        if (IsPlayerTurn())
            ResetActionData(currentPlayerIndex);


    }


    public void SetDiceResult(int result)
    {
        players[currentPlayerIndex].lastDiceResult = result;
        Debug.Log("Player " + (currentPlayerIndex + 1) + " result of dice is updated : " + result);
    }

    public void SetTypeCard(string type)
    {
        players[currentPlayerIndex].lastTypeCard = type;
    }

    public void SetScannedCardID(string id)
    {
        players[currentPlayerIndex].lastScannedCardID = id;
    }

    public void Damage(int value)
    {
        players[currentPlayerIndex].health -= value;
        if (players[currentPlayerIndex].health < 0)
            players[currentPlayerIndex].health = 0;

        Debug.Log("Player " + (currentPlayerIndex + 1)
            + " Get Damage " + value
            + " HP " + players[currentPlayerIndex].health);


    }

    public void Heal(int value)
    {
        if (players[currentPlayerIndex].health <= 0)
        {
            Debug.Log("Player " + (currentPlayerIndex + 1) + " already 0 HP. Can't be healed.");
            return;
        }

        players[currentPlayerIndex].health += value;
        if (players[currentPlayerIndex].health > 5)
            players[currentPlayerIndex].health = 5;

        Debug.Log("Player " + (currentPlayerIndex + 1)
            + " Heal " + value
            + " HP " + players[currentPlayerIndex].health);


    }
    public void SetPoliceDice(int value, int policeIndex)
    {
        polices[policeIndex].lastDiceResult = value;
    }
    public bool IsPlayerDead(int index)
    {
        if (index < 0 || index >= players.Length) return false;
        return players[index].health <= 0;
    }
    public void SetPoliceWheel(string step, int index)
    {
        if (polices[index].isWheel)
            polices[index].laststepwhellValue = step;
    }

    public void SaveBackup(int playerIndex)
    {
        // backup SEMUA player, bukan cuma satu
        for (int i = 0; i < players.Length; i++)
        {
            if (backup[i] == null)
                backup[i] = new PlayerBackup();

            backup[i].CopyFrom(players[i]);
        }


    }
    public void RestoreBackup(int playerIndex)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (backup[i] != null)
                backup[i].CopyTo(players[i]);
        }
    }

    public void ResetActionData(int index)
    {
        players[index].lastDiceResult = 0;
        players[index].lastTypeCard = "";
        players[index].lastScannedCardID = "";
    }

    //setiap turn diisi 4 player + 1 police
    //turn 1 = p1 turn, p2, p3, p4, police standby
    //turn 2 = p1 turn, p2, p3, p4, police standby
    //turn 3 = p1 turn, p2, p3, p4, police standby
    //turn 4 = p1 turn, p2, p3, p4, police turn
    //turn 5 = p1 turn, p2, p3, p4, police turn


}