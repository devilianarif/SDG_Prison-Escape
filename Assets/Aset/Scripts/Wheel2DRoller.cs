using UnityEngine;

public class Wheel2DRoller : MonoBehaviour
{
    public Transform wheelObject;

    public float rotatePower;
    public float stopPower;
    public bool IsStopped => spinning == 0;
    public bool snapDone = false;
    float currentSpeed;
    int spinning;
    float t;

    public int setRX = 0;
    public int setRY = 0;

    public string[] reward;
    public string lastWheelValue;

    void Update()
    {
        // wheelObject ikut canvas tapi XY-nya dikunci di bawah
        HandleSpin();
        LockXY();
    }

    void HandleSpin()
    {
        if (currentSpeed > 0)
        {
            currentSpeed -= stopPower * Time.deltaTime;
            if (currentSpeed < 0.5f) currentSpeed = 0;
        }

        if (currentSpeed > 0)
            wheelObject.Rotate(0, 0, currentSpeed * Time.deltaTime);

        if (currentSpeed == 0 && spinning == 1)
        {
            t += Time.deltaTime;
            if (t >= 0.2f)
            {
                Snap();
                spinning = 0;
                t = 0;
            }
        }
    }

    void LockXY()
    {
        // kunci XY selalu tidak berubah
        Vector3 e = wheelObject.localEulerAngles;
        wheelObject.localEulerAngles = new Vector3(setRX, setRY, e.z);
    }

    public void Rotate()
    {
        if (spinning == 0)
        {
            snapDone = false;
            currentSpeed = rotatePower;
            spinning = 1;

        }
    }

    void Snap()
    {
        float rot = wheelObject.localEulerAngles.z;
        rot %= 360;

        float adj = rot + 22.5f;
        if (adj >= 360) adj -= 360;

        int sector = Mathf.FloorToInt(adj / 45f);
        float snapAngle = sector * 45f;

        wheelObject.localEulerAngles = new Vector3(setRX, setRY, snapAngle);
        lastWheelValue = reward[sector];

        snapDone = true;

    }



}
