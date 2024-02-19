using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using BeauUtil;
using BeauUtil.Variants;
using FieldDay;
using FieldDay.Components;
using FieldDay.Scenes;
using FieldDay.Scripting;
using Leaf.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace WeatherStation.Scripting
{
    /// <summary>
    /// Controls multiple sockets through this one component
    /// </summary>
    public class ScriptSocketHub : ScriptComponent
    {

        #region Inspector

        [SerializeField, Required] private PuzzleSocket[] m_Sockets;

        #endregion // Inspector


        #region Leaf

        [LeafMember("SetSocketsLocked"), Preserve]
        public void SetLocked(bool lockParam)
        {
            foreach (var socket in m_Sockets)
            {
                socket.Locked = lockParam;
            }
        }

        #endregion // Leaf

        #region Unity Events

#if UNITY_EDITOR

#endif // UNITY_EDITOR

        #endregion // Unity Events
    }
}