using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}