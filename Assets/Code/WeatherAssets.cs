using System;
using BeauUtil;
using FieldDay;
using FieldDay.Assets;
using FieldDay.Scripting;
using UnityEngine;

namespace WeatherStation {
    public class WeatherAsets : MonoBehaviour {
        [SerializeField] private AssetPack[] m_Packs = Array.Empty<AssetPack>();

        private void Start() {
            foreach (var pack in m_Packs) {
                Game.Assets.LoadPackage(pack);
            }
        }
    }
}