using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    public float preRollHeight = 0.4f;
    public float preRollSpin = 800f;
    public float throwForce = 5f;
    public float throwTorque = 10f;
    public DiceValueReader valueReader;
    public int lastResult;
    public Button rollButton;
    Vector3 startPos;
    Quaternion startRot;

    public float shakeForce = 3f;
    Vector3 lastAcc;

    Rigidbody rb;
    public bool allowRead;

    bool isRolling;

    void Start()
    {
        lastAcc = Input.acceleration;
        startPos = transform.position;
        startRot = transform.rotation;

        rb = GetComponent<Rigidbody>();
        if (valueReader != null) valueReader.ForceValue(0);

        if (rollButton != null)
            rollButton.onClick.AddListener(RollButton);
    }
    public void Update()
    {
        Vector3 acc = Input.acceleration;
        Vector3 delta = acc - lastAcc;

        if (delta.sqrMagnitude > 0.02f && !isRolling)
        {
            if (rb != null)
            {
                rb.AddForce(new Vector3(delta.x, Mathf.Abs(delta.y), delta.z) * shakeForce, ForceMode.Impulse);
            }
        }

        lastAcc = acc;
    }
    public void RollButton()
    {
        if (isRolling) return;
        StartCoroutine(RollSequence());

    }

    IEnumerator RollSequence()
    {

        isRolling = true;
        allowRead = false;
        valueReader.ResetRecord();

        foreach (var face in GetComponentsInChildren<DiceFace>())
            face.ResetFace();

        rb.isKinematic = true;

        transform.position = startPos;
        transform.rotation = startRot;

        transform.position += Vector3.up * preRollHeight;

        transform.Rotate(Vector3.up * preRollSpin * Time.deltaTime);

        yield return new WaitForSeconds(0.15f);

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
        rb.AddForce(dir * throwForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * throwTorque, ForceMode.Impulse);

        // tunggu sampai benar-benar diam
        yield return new WaitUntil(() =>
        rb.linearVelocity.sqrMagnitude < 0.01f &&
        rb.angularVelocity.sqrMagnitude < 0.01f
        );
        yield return new WaitForSeconds(0.2f);
        // aktifkan pembacaan face setelah dice stop
        allowRead = true;

        // tunggu sampai ada face yang terbaca
        yield return new WaitUntil(() => valueReader.GetValue() != 0);

        lastResult = valueReader.GetValue();
        Debug.Log("Hasil dadu: " + lastResult);



        if (valueReader.gameManager != null && !valueReader.isPoliceDice)
        {
            valueReader.gameManager.playerState.SetDiceResult(lastResult);
            valueReader.gameManager.UpdateChecklist();
            valueReader.gameManager.UpdateLatestDice(lastResult);
        }
        isRolling = false;


    }

}