using System.Reflection;
using BeauUtil;
using BeauUtil.Editor;
using FieldDay.Assets;
using UnityEditor;
using UnityEngine;

namespace FieldDay.Editor {
    [CustomPropertyDrawer(typeof(AssetNameAttribute), true)]
    public class AssetNamePropertyDrawer  : PropertyDrawer {
        private struct CacheEntry {
            public double LastUpdateTime;
            public NamedItemList<string> Items;
        }

        static private LruCache<uint, CacheEntry> s_ListCache = new LruCache<uint, CacheEntry>(32);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            base.OnGUI(position, property, label);
        }

        static public void Render(Rect position, SerializedProperty property, GUIContent label, FieldInfo field, AssetNameAttribute attribute)
    }
}