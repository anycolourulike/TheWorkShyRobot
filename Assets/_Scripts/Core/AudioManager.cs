using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rambler.Core 
{
  public static class AudioManager
  {
    public enum WeaponSound
    {
        pistolShoot, 
        SMGShoot, 
        RifleShoot, 
        ShotgunShoot, 
        outOfAmmo,
        shieldHit,
        humanImpact,
        robotImpact,
        weaponReload,
    }
    
    public enum EnviromentSound
    {        
        ambientMusic,
        SpaceShipBackground,
        RigBackground,
        CaveBackground,
        SurfaceBackground,  
        ObjectiveRecevived,    
        ObjectiveComplete,
        OnGameOver,
        RadioMessageStart,
        RadioMessageFinish,
        RadioTalk1,
        RadioTalk2,
        RadioTalk3,
        DoorOpen,
        DoorClosed,
        Explosion1,
        Explosion2,
        alarm,

        footInterior1,
        footInterior2,
        footInterior3,
        footSand1,
        footSand2,
        footSand3,
        footStone1,
        footStone2,
        footStone3,
    }

    public enum HumanSound
    {
        Death1,
        Death2,
        Death3,
        Death4,

        Hit1,
        Hit2,
        Hit3,
        Hit4,
    }
    
       
   public static void PlayWeaponSound(WeaponSound weaponSFX, Vector3 position)
    {
        GameObject soundGameObject = new GameObject("WeaponSFX");
        soundGameObject.transform.position = position;
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetWeaponClip(weaponSFX);
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;        
        audioSource.Play();
    }    

    private static AudioClip GetWeaponClip(WeaponSound weaponSound)
    {
        foreach(GameAssets.WeaponAudioClip weaponSFX in GameAssets.i.weaponAudioClipArray)
        {
            if(weaponSFX.weaponEnum == weaponSound)
            {
                return weaponSFX.weaponClip;
            }
        }
        Debug.Log(weaponSound + " not found!");
        return null;
    }

    public static void PlayEnviromentSound(EnviromentSound enviromentSound)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetEnviromentClip(enviromentSound));
    }    

    private static AudioClip GetEnviromentClip(EnviromentSound enviromentSound)
    {
        foreach(GameAssets.EnviromentAudioClip enviromentClip in GameAssets.i.enviromentAudioClipArray)
        {
            if(enviromentClip.enviromentSFX == enviromentSound)
            {
                return enviromentClip.enviromentClip;
            }
        }
        Debug.Log(enviromentSound + " not found!");
        return null;
    }

    public static void PlayHumanSound(HumanSound humanSound)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetHumanClip(humanSound));
    }   

    private static AudioClip GetHumanClip(HumanSound humanSound)
    {
        foreach(GameAssets.HumanAudioClip humanClip in GameAssets.i.humanAudioClipArray)
        {
            if(humanClip.humanSFX == humanSound)
            {
                return humanClip.humanClip;
            }
        }
        Debug.Log(humanSound + " not found!");
        return null;
    }
  }
} 