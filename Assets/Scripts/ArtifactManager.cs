using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    [HideInInspector]
    public bool isGameWin;

    void Update()
    {
        gameObject.transform.Rotate(0, 180 * Time.deltaTime/5, 0, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isGameWin= true;
        }
    }
}
