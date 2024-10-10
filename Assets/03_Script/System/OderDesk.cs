using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OderDesk : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plate"))
        {
            GameObject Plate = other.gameObject;
            Plate.GetComponent<XRGrabInteractable>().enabled = false;
            Plate.GetComponent<BoxCollider>().enabled = false;
            Plate.GetComponent<Rigidbody>().useGravity = false;
            // 현재 손님을 가져옴
            Customer currentCustomer = GameManager.Instance.currentDayCustomers[0];

            // 손님에게 요리를 서빙
            currentCustomer.ServeRecipe(Plate.GetComponent<CookPlate>().GiveRecipe());

            // Plate를 Hand의 자식으로 설정
            Plate.transform.SetParent(currentCustomer.MyHand.transform);
            Plate.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // Plate의 로컬 위치를 설정
            Plate.transform.localPosition = new Vector3(-0.377f, -0.138f, -0.222f); // 예시로 (0, 0, 0)으로 설정, 필요에 따라 변경 가능
            float xRotation = 70.984f;
            float yRotation = -36.387f;
            float zRotation = -94.968f;

            // Quaternion을 생성
            Quaternion rotation = Quaternion.Euler(xRotation, yRotation, zRotation);

            // Plate의 로컬 회전 설정
            Plate.transform.localRotation = rotation;
            // 로컬 위치와 회전을 원하는 대로 설정할 수 있음
            // 예: Plate.transform.localPosition = new Vector3(0, 0.1f, 0.1f); 와 같이 구체적인 값 설정
        }
    }



}
