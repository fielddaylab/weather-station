using System;
using UnityEngine;

namespace FieldDay.Assets {
    /// <summary>
    /// Property attribute marking a string/StringHash32/SerializedHash32 field as an asset identifier.
    /// This will store the name of the asset.
    /// </summary>
    public class AssetNameAttribute : PropertyAttribute {
        public Type AssetType;

        protected virtual bool Predicate(UnityEngine.Object obj) { return true; }
        protected virtual string Name(UnityEngine.Object obj) { return obj.name; }

        public AssetNameAttribute(Type assetType) {
            AssetType = assetType;
            order = -10;
        }
    }
}