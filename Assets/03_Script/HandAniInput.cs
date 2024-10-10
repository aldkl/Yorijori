using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAniInput : MonoBehaviour
{
    [SerializeField] private InputActionProperty triggerAction;
    [SerializeField] private InputActionProperty gripAction;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        float triggerValue = triggerAction.action.ReadValue<float>();
        float gripValue = gripAction.action.ReadValue<float>();



        //anim.SetFloat("Trigger", triggerValue);
        //anim.SetFloat("Grip", gripValue);


    }
    public void PlayHandAni(string Name)
    {

        anim.Play(Name);
    }

    public void PlayIdle()
    {
        anim.Play("Idle");
    }
}
