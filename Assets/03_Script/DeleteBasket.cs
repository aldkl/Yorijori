using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteBasket : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ingredient"))
        {
            GameObject.Destroy(collision.gameObject);
        }
    }
}
