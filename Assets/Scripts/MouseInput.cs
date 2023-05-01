using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    [SerializeField]
    float mouseSpeed;
    float mouseSpeedAdjust;

    float mouse_X, mouse_Y;
    float rotationY;

    private void Awake()
    {
        Cursor.lockState= CursorLockMode.Locked;
    }
    void Start()
    {
        mouseSpeedAdjust = PlayerPrefs.GetFloat("SensitivityValue");
        mouseSpeed *= mouseSpeedAdjust;
    }

    // Update is called once per frame
    void Update()
    {
        MouseMove();
    }
    void MouseMove()
    {
        mouse_X = (mouse_X + (Input.GetAxis("Mouse X") * mouseSpeed * Time.deltaTime)) % 360;
        mouse_Y = (Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime);

        rotationY -= mouse_Y;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
        transform.rotation = Quaternion.Euler(rotationY, mouse_X, 0);
    }
}
