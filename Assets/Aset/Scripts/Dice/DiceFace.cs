using UnityEngine;

public class DiceFace : MonoBehaviour
{
public int faceValue;
DiceValueReader reader;
DiceRoller roller;
bool counted;
void Start()
{
    reader = GetComponentInParent<DiceValueReader>();
    roller = GetComponentInParent<DiceRoller>();
}

    void OnTriggerEnter(Collider other)
    {
        if (!roller.allowRead) return;
        if (counted) return;

        if (other.CompareTag("Ground"))
        {
            counted = true;
            reader.SetValue(faceValue);
        }


    }

public void ResetFace()
{
counted = false;
}

}