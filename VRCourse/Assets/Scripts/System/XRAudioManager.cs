using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAudioManager : MonoBehaviour
{
    [Header("Grab Interactables")]

    [SerializeField]
    XRGrabInteractable[] grabInteractables;

    [SerializeField]
    AudioSource grabSound;

    [SerializeField]
    AudioSource activatedSound;
    
    [SerializeField]
    AudioClip grabClip;

    [SerializeField]
    AudioClip keyClip;

    [SerializeField]
    AudioClip grabActivatedClip;

    [SerializeField] 
    AudioClip wandActivatedClip;


    [Header("Drawer Interactables")]

    [SerializeField]
    DrawerInteractable drawer;

    [SerializeField]
    AudioSource drawerSound;

    [SerializeField]
    AudioClip drawerMoveClip;


    [Header("The Wall")]

    [SerializeField]
    TheWall wall;

    [SerializeField]
    AudioSource wallSound;

    [SerializeField]
    AudioClip destoryWallClip;

    [SerializeField]
    AudioClip fallBackClip;
    const string FALL_BACK_CLIP = "fallBackClip";


    private void OnEnable()
    {
        //Check if there's a fallback clip
        if (fallBackClip == null)
        {
            fallBackClip = AudioClip.Create(FALL_BACK_CLIP, 1, 1, 1000, true);
        }

        //Grabbable Objects
        SetGrabbables();

        //Drawer
        if (drawer != null)
        {
            SetDrawerInteractable();
        }

        //Wall
        if (wall != null)
        {
            SetWall();
        }
    }

    private void OnDisable()
    {
        if (wall != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
        }
    }

    private void OnSelectEnteredGrabbable(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.CompareTag("Key"))
        {
            grabSound.clip = keyClip;
        }
        else
        {
            grabSound.clip = grabClip;
        }

        grabSound.Play();
    }

    private void OnSelectExitedGrabbable(SelectExitEventArgs arg0)
    {
        grabSound.clip = grabClip;
        grabSound.Play();
    }

    private void OnActivatedGrabbable(ActivateEventArgs arg0)
    {
        GameObject tempGameObj = arg0.interactableObject.transform.gameObject;

        if (tempGameObj.GetComponent<WandControl>() != null)
        {
            activatedSound.clip = wandActivatedClip;
        }
        else
        {
            activatedSound.clip = grabActivatedClip;
        }

        activatedSound.Play();
    }

    private void OnDestroyWall()
    {
        //Play audio source
        if (wallSound != null)
        {
            wallSound.Play();
        }
    }

    private void OnDrawerStop(SelectExitEventArgs arg0)
    {
        drawerSound.Stop();
    }

    private void OnDrawerMove(SelectEnterEventArgs arg0)
    {
        drawerSound.Play();
    }
    void SetGrabbables()
    {
        //Find the grabbable objects
        grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);

        //Loop through each grabbable object & add set listeners
        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.AddListener(OnSelectEnteredGrabbable);
            grabInteractables[i].selectExited.AddListener(OnSelectExitedGrabbable);
            grabInteractables[i].activated.AddListener(OnActivatedGrabbable);
        }
    }

    private void SetWall()
    {
        destoryWallClip = wall.GetDestroyClip;
        CheckIfClipIsNull(destoryWallClip);
        wall.OnDestroy.AddListener(OnDestroyWall);
    }

    private void SetDrawerInteractable()
    {
        //Set up audio source
        drawerSound = drawer.transform.AddComponent<AudioSource>();
        drawerMoveClip = drawer.GetMoveClip;
        CheckIfClipIsNull(drawerMoveClip);

        drawerSound.clip = drawerMoveClip;
        drawerSound.loop = true;

        drawer.selectEntered.AddListener(OnDrawerMove);
        drawer.selectExited.AddListener(OnDrawerStop);
    }

    void CheckIfClipIsNull(AudioClip clip)
    {
        if (clip == null)
        {
            clip = fallBackClip;
        }
    }
}
