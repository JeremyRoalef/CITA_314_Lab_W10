using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRAudioManager : MonoBehaviour
{
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
        //
        if (fallBackClip == null)
        {
            fallBackClip = AudioClip.Create(FALL_BACK_CLIP, 1, 1, 1000, true);
        }

        //
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

    private void OnDestroyWall()
    {
        //Play audio source
        if (wallSource != null)
        {
            wallSource.Play();
        }
    }

    private void OnDisable()
    {
        if (wall != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
        }
    }


}
