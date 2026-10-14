using UnityEngine;

//스티브 걷기 반복노동용 스크립트
public class SteveWalk : MonoBehaviour
{
    public Transform leftArm;
    public Transform rightArm;
    public Transform leftLeg;
    public Transform rightLeg;

    public float speed = 8f;
    public float angle = 35f;
    public bool isWalking = true;

    void Update()
    {
        float swing = isWalking
            ? Mathf.Sin(Time.time * speed) * angle
            : 0f;

        leftArm.localRotation =
            Quaternion.Euler(0, 0, swing);

        rightArm.localRotation =
            Quaternion.Euler(0, 0, -swing);

        leftLeg.localRotation =
            Quaternion.Euler(0, 0, -swing);

        rightLeg.localRotation =
            Quaternion.Euler(0, 0, swing);
    }
}