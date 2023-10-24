using System;
using UnityEngine;

namespace WeatherStation
{
    public class DBObjectIdAttribute : PropertyAttribute
    {
        public Type AssetType;
        public virtual bool Filter(DBObject inObject) { return true; }
        public virtual string Name(DBObject inObject) { return inObject.name; }

        public DBObjectIdAttribute(Type inAssetType)
        {
            AssetType = inAssetType;
            order = -10;
        }
    }
}