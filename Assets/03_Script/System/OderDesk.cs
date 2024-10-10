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
            // ���� �մ��� ������
            Customer currentCustomer = GameManager.Instance.currentDayCustomers[0];

            // �մԿ��� �丮�� ����
            currentCustomer.ServeRecipe(Plate.GetComponent<CookPlate>().GiveRecipe());

            // Plate�� Hand�� �ڽ����� ����
            Plate.transform.SetParent(currentCustomer.MyHand.transform);
            Plate.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // Plate�� ���� ��ġ�� ����
            Plate.transform.localPosition = new Vector3(-0.377f, -0.138f, -0.222f); // ���÷� (0, 0, 0)���� ����, �ʿ信 ���� ���� ����
            float xRotation = 70.984f;
            float yRotation = -36.387f;
            float zRotation = -94.968f;

            // Quaternion�� ����
            Quaternion rotation = Quaternion.Euler(xRotation, yRotation, zRotation);

            // Plate�� ���� ȸ�� ����
            Plate.transform.localRotation = rotation;
            // ���� ��ġ�� ȸ���� ���ϴ� ��� ������ �� ����
            // ��: Plate.transform.localPosition = new Vector3(0, 0.1f, 0.1f); �� ���� ��ü���� �� ����
        }
    }



}
