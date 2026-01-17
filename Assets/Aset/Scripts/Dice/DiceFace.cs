using UnityEngine;

public class DiceFace : MonoBehaviour
{
    public int faceValue;

    DiceValueReader reader;
    DiceRoller roller;
    Rigidbody rb;

    float groundTimer;
    const float requiredTime = 0.5f;

    bool locked;
    Vector3 lastUp;

    void Start()
    {
        reader = GetComponentInParent<DiceValueReader>();
        roller = GetComponentInParent<DiceRoller>();
        rb = GetComponentInParent<Rigidbody>();
    }

    void OnTriggerStay(Collider other)
    {
        if (locked) return;
        if (!roller.allowRead) return;
        if (!other.CompareTag("Ground")) return;

        // 1️⃣ kalau masih gerak dikit, anggap belum stabil
        if (!rb.IsSleeping())
        {
            ResetTimer();
            return;
        }

        // 2️⃣ cek orientasi face konsisten
        float angle = Vector3.Angle(transform.up, lastUp);
        if (groundTimer > 0f && angle > 3f)
        {
            ResetTimer();
            return;
        }

        lastUp = transform.up;
        groundTimer += Time.deltaTime;

        if (groundTimer >= requiredTime)
        {
            locked = true;
            reader.SetValue(faceValue);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
            ResetTimer();
    }

    void ResetTimer()
    {
        groundTimer = 0f;
        lastUp = transform.up;
    }

    public void ResetFace()
    {
        locked = false;
        ResetTimer();
    }
}
