using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : MonoBehaviour
{
    public GameObject PlatePerfab;
    public Transform plateBoxpos;


    public void CreatePlate()
    {
        GameObject newcustomer = Instantiate(PlatePerfab, plateBoxpos.position, Quaternion.identity);

    }

}
