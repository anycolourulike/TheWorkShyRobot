using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rambler.Core
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets _i;

        public static GameAssets i
        {
            get
            {
                if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
                return _i;
            }
        }

        public WeaponAudioClip[] weaponAudioClipArray;
        public EnviromentAudioClip[] enviromentAudioClipArray;
        public HumanAudioClip[] humanAudioClipArray;
  

        [System.Serializable]
        public class WeaponAudioClip
        {
            public AudioManager.WeaponSound weaponEnum;
            public AudioClip weaponClip;
        }

        [System.Serializable]
        public class EnviromentAudioClip
        {
            public AudioManager.EnviromentSound enviromentSFX;
            public AudioClip enviromentClip;
        }

        [System.Serializable]
        public class HumanAudioClip
        {
            public AudioManager.HumanSound humanSFX;
            public AudioClip humanClip;
        }
    }
}