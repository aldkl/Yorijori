using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckCook : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Plate")
            return;

        GameManager.Instance.nowCook = other.gameObject;
    }
}
