using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Core;

public static class SoundManager 
{
    public enum WeaponSound
    {
        pistolShoot,
        mPistolShoot,
        rifleShoot,
        shotgunShoot
    }

    public static void PlayProjectileSound(WeaponSound projectileSound)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(projectileSound));
    }    

    private static AudioClip GetAudioClip(WeaponSound projectileSound)
    {
        foreach(GameAssets.ProjectileAudioClip projectileClip in GameAssets.Instance.projectileAudioClipArray)
        {
            if(projectileClip.projectileSFX == projectileSound)
            {
                return projectileClip.projectileFire;
            }
        }
        Debug.Log(projectileSound + " not found!");
        return null;
    }
}

