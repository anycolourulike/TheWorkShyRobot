using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rambler.Core
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets _i;

        public static GameAssets Instance
        {
            get
            {
                if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
                return _i;
            }
        }

        public ProjectileAudioClip[] projectileAudioClipArray;

        [System.Serializable]
        public class ProjectileAudioClip
        {
            public SoundManager.WeaponSound projectileSFX;
            public AudioClip projectileFire;
        }
    }
}