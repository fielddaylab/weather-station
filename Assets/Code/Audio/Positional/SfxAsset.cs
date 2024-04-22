using System;
using BeauRoutine.Extensions;
using BeauUtil;
using FieldDay.Assets;
using UnityEngine;

namespace WeatherStation {
    [CreateAssetMenu(menuName = "Weather Station/Sfx Asset")]
    public class SfxAsset : NamedAsset {
        public AudioClip[] Clips;
        public FloatRange Volume = new FloatRange(1);
        public FloatRange Pitch = new FloatRange(1);
        public FloatRange Delay = new FloatRange(0);
        public SerializedHash32 Tag;

        [Header("Positional")]
        [Range(0, 1)] public float SpatialBlend = 1;
        [Range(0, 360)] public float Spread;
        [Range(0.3f, 15)] public float Range = 4;
        public AudioRolloffMode Rolloff = AudioRolloffMode.Linear;

        [NonSerialized] public RandomDeck<AudioClip> Randomizer;
    }

    public class SfxRefAttribute : AssetNameAttribute {
        public SfxRefAttribute() : base(typeof(SfxAsset), true) { }

        protected override string Name(UnityEngine.Object obj) {
            return base.Name(obj).Replace('-', '/');
        }
    }
}