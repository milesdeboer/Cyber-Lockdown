using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{

    [SerializeField]
    private MalwareController malwareController;

    [SerializeField]
    private GameObject selectionWindow;
    [SerializeField]
    private GameObject customizationWindow;

    [SerializeField]
    private GameObject dataCenterButton;

    private List<Attack> attacks;

    private int activeAttack = -1;
    
    public void Start() {
        // center data center

        // circle data centers
        for (int i = 1; i < GameManager.DATA_CENTERS_PER_PLAYER; i++) {
            //Vector2 coords;
            //Instantiate
        }

    }

    private void InitAttacks() {
        attacks = new List<Attack>();
        for(int i = 0; i < 8; i++) {
            attacks.Add(new Attack(i));
        }
        Debug.Log("Attacks Initialized");
    }

    public void OnClick(int i) {
        customizationWindow.SetActive(true);
        activeAttack = i;
        Debug.Log("Opening attack " + activeAttack + ".");
        Reset();
        selectionWindow.SetActive(false);
    }

    public void MalwareClick(int i) {
        attacks[activeAttack].SetMalware(malwareController.GetMalware(i));
        Debug.Log("Switching Malware of Attack " + (activeAttack+1) + " to " + attacks[activeAttack].GetMalware().GetId() + ".");
    }

    public void ObjectiveClick(string objective) {
        attacks[activeAttack].SetObjective(objective);
        Debug.Log("Setting Objective of Attack " + activeAttack + " to " + objective + ".");
    }

    public void DeliveryClick(string delivery) {
        attacks[activeAttack].SetDelivery(delivery);
        Debug.Log("Setting Delivery of Attack " + activeAttack + " to " + delivery + ".");
    }

    private void Reset() {
        
    }
}
