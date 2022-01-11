using UnityEngine;

public class Rotate : MonoBehaviour
{

    [SerializeField]
    private int rotateSpeed = 360;

    private void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}
