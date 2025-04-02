using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAudioManager : MonoBehaviour
{
    [SerializeField]
    XRGrabInteractable[] grabInteractables;

    [SerializeField]
    AudioSource grabSound;

    [SerializeField]
    AudioClip grabClip;

    [SerializeField]
    AudioClip keyClip;

    [SerializeField]
    AudioSource activatedSound;

    [SerializeField]
    AudioClip grabActivatedClip;

    [SerializeField] 
    AudioClip wandActivatedClip;

    [SerializeField]
    TheWall wall;

    [SerializeField]
    AudioSource wallSource;

    [SerializeField]
    AudioClip destoryWallClip;

    [SerializeField]
    AudioClip fallBackClip;
    const string FALL_BACK_CLIP = "fallBackClip";


    private void OnEnable()
    {
        grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);
        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.AddListener(OnSelectEnteredGrabbable);
            grabInteractables[i].selectExited.AddListener(OnSelectExitedGrabbable);
            grabInteractables[i].activated.AddListener(OnActivatedGrabbable);
        }


        //Check if there's a fallback clip
        if (fallBackClip == null)
        {
            fallBackClip = AudioClip.Create(FALL_BACK_CLIP, 1, 1, 1000, true);
        }

        //Check if wall exists
        if (wall != null)
        {
            destoryWallClip = wall.GetDestroyClip;
            if (destoryWallClip == null)
            {
                destoryWallClip = fallBackClip;
            }
            wall.OnDestroy.AddListener(OnDestroyWall);
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
        if (wallSource != null)
        {
            wallSource.Play();
        }
    }
}
