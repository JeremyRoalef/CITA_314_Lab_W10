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
    XRSocketInteractor drawerSocket;

    [SerializeField]
    AudioSource drawerSound;

    [SerializeField]
    AudioSource drawerSocketSound;

    [SerializeField]
    AudioClip drawerMoveClip;

    [SerializeField]
    AudioClip drawerSocketClip;

    [Header("The Wall")]

    [SerializeField]
    TheWall wall;

    [SerializeField]
    XRSocketInteractor wallSocket;

    [SerializeField]
    AudioSource wallSound;

    [SerializeField]
    AudioSource wallSocketSound;

    [SerializeField]
    AudioClip destoryWallClip;

    [SerializeField]
    AudioClip fallBackClip;

    [SerializeField]
    AudioClip wallSocketClip;

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
        CheckIfClipIsNull(ref destoryWallClip);
        wall.OnDestroy.AddListener(OnDestroyWall);

        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
        {
            wallSocketSound = wallSocket.transform.AddComponent<AudioSource>();
            wallSocketClip = wall.GetSocketClip;
            CheckIfClipIsNull(ref wallSocketClip);
            wallSocketSound.clip = wallSocketClip;
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }

    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        wallSocketSound.Play();
    }

    private void SetDrawerInteractable()
    {
        //Set up audio source
        drawerSound = drawer.transform.AddComponent<AudioSource>();
        drawerMoveClip = drawer.GetMoveClip;
        CheckIfClipIsNull(ref drawerMoveClip);

        drawerSound.clip = drawerMoveClip;
        drawerSound.loop = true;

        drawer.selectEntered.AddListener(OnDrawerMove);
        drawer.selectExited.AddListener(OnDrawerStop);

        drawerSocket = drawer.GetSocketIntractor;
        if (drawerSocket != null)
        {
            drawerSocketSound = drawerSocket.transform.AddComponent<AudioSource>();
            drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
            drawerSocketClip = drawer.GetSocketedClip;
            CheckIfClipIsNull(ref drawerSocketClip);
            drawerSound.clip = drawerSocketClip;

        }
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        drawerSocketSound.Play();
    }

    /*
     * Using ref in the parameter passes the exact reference of the given argument. Without it,
     * changing the reference inside the function will not chage the reference outside the function.
     * The parameter is referencing the same object in memory as the passed argument reference, but they
     * are different references. Using ref means passing the exact reference to the function, meaning
     * changes to the parameter reference will change the argument's reference too.
     * 
     * This also means that when you call the function, you need to add ref before passing the argument
     * (CheckIfClipIsNull(ref myAudioClip))
     */
    void CheckIfClipIsNull(ref AudioClip clip)
    {
        if (clip == null)
        {
            clip = fallBackClip;
        }
    }
}
