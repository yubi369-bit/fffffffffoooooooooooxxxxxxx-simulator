using UnityEngine;

public class AlexArmRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 90f;

    void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }
}
