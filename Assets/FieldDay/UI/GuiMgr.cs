using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.HID;
using FieldDay.Rendering;
using UnityEngine;

using PanelIndex = BeauUtil.TypeIndex<FieldDay.UI.IGuiPanel>;

namespace FieldDay.UI {
    /// <summary>
    /// Interface manager.
    /// </summary>
    public sealed class GuiMgr {
        private ISharedGuiPanel[] m_SharedPanelMap = new ISharedGuiPanel[PanelIndex.Capacity];
        private readonly HashSet<IGuiPanel> m_PanelSet = new HashSet<IGuiPanel>(32);

        private readonly Dictionary<StringHash32, RectTransform> m_NamedElementMap = new Dictionary<StringHash32, RectTransform>(16, CompareUtils.DefaultEquals<StringHash32>());

        private readonly RingBuffer<IOnGuiUpdate> m_UpdateCallbacks = new RingBuffer<IOnGuiUpdate>(32, RingBufferMode.Expand);

        private InputMgr m_InputMgr;
        private Camera m_PrimaryUICamera;

        internal GuiMgr(InputMgr inputMgr) {
            Assert.NotNull(inputMgr);
            m_InputMgr = inputMgr;
        }

        #region Gui Camera

        /// <summary>
        /// Primary UI camera.
        /// </summary>
        public Camera PrimaryCamera {
            get { return m_PrimaryUICamera; }
        }

        /// <summary>
        /// Sets the primary UI camera.
        /// </summary>
        public void SetPrimaryCamera(Camera camera) {
            if (m_PrimaryUICamera == camera) {
                return;
            }

            if (m_PrimaryUICamera != null) {
                Log.Warn("[GuiMgr] Primary Gui camera already set to '{0}' - make sure to deregister it first", m_PrimaryUICamera);
            }
            m_PrimaryUICamera = camera;
            Log.Msg("[GuiMgr] Assigned primary Gui camera as '{0}'", camera);
        }

        /// <summary>
        /// Removes the given camera as the primary UI camera.
        /// </summary>
        public void RemovePrimaryCamera(Camera camera) {
            if (camera == null || m_PrimaryUICamera != camera) {
                return;
            }

            m_PrimaryUICamera = null;
            Log.Msg("[GuiMgr] Removed primary Gui camera");
        }

        #endregion // Gui Camera

        #region Add/Remove

        #region Panels

        /// <summary>
        /// Registers the given IGuiPanel instance.
        /// </summary>
        public void RegisterPanel(IGuiPanel panel) {
            Assert.NotNull(panel);
            Type panelType = panel.GetType();
            int index = PanelIndex.Get(panelType);

            if (m_PanelSet.Add(panel)) {
                ISharedGuiPanel shared = panel as ISharedGuiPanel;
                if (shared != null) {
                    Assert.True(m_SharedPanelMap[index] == null, "[GuiMgr] Shared panel of type '{0}' already registered", panelType);
                    m_SharedPanelMap[index] = shared;
                }

                RegistrationCallbacks.InvokeRegister(panel);
                Log.Msg("[GuiMgr] Panel '{0}' registered", panelType.FullName);
            }
        }

        /// <summary>
        /// Deregisters the given IGuiPanel instance.
        /// </summary>
        public void DeregisterPanel(IGuiPanel panel) {
            Assert.NotNull(panel);

            if (m_PanelSet.Remove(panel)) {
                Type panelType = panel.GetType();
                int index = PanelIndex.Get(panelType);

                if (m_SharedPanelMap[index] == panel) {
                    m_SharedPanelMap[index] = null;
                }

                RegistrationCallbacks.InvokeDeregister(panel);
                Log.Msg("[GuiMgr] Panel '{0}' deregistered", panelType.FullName);
            }
        }

        #endregion // Panels

        #region Named

        /// <summary>
        /// Registers a transform with the given name.
        /// </summary>
        public void RegisterNamed(StringHash32 name, RectTransform transform) {
            Assert.NotNull(transform);
            Assert.False(name.IsEmpty);

            if (m_NamedElementMap.ContainsKey(name)) {
                Log.Warn("[GuiMgr] Element with name '{0}' already registered", name);
                return;
            }

            m_NamedElementMap.Add(name, transform);
            Log.Msg("[GuiMgr] RectTransform with name '{0}' registered", name);
        }

        /// <summary>
        /// Deregisters a transform with the given name.
        /// </summary>
        public void DeregisterNamed(StringHash32 name, RectTransform transform) {
            Assert.NotNull(transform);
            Assert.False(name.IsEmpty);

            if (m_NamedElementMap.TryGetValue(name, out RectTransform existing) && existing == transform) {
                m_NamedElementMap.Remove(name);
                Log.Msg("[GuiMgr] RectTransform with name '{0}' deregistered", name);
            }
        }

        #endregion // Named

        #region Updates

        /// <summary>
        /// Registers an element for update callbacks.
        /// </summary>
        public void RegisterUpdate(IOnGuiUpdate updater) {
            Assert.NotNull(updater);

            m_UpdateCallbacks.PushBack(updater);
        }

        /// <summary>
        /// Deregisters an element for update callbacks.
        /// </summary>
        public void DeregisterUpdate(IOnGuiUpdate updater) {
            Assert.NotNull(updater);

            m_UpdateCallbacks.FastRemove(updater);
        }

        #endregion // Updates

        #endregion // Add/Remove

        #region Lookup

        #region Shared

        /// <summary>
        /// Returns the shared panel object of the given type.
        /// This will assert if none is found.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISharedGuiPanel GetShared(Type type) {
            int index = PanelIndex.Get(type);
            ISharedGuiPanel panel = m_SharedPanelMap[index];
            if (panel == null) {
                panel = (ISharedGuiPanel) GameObject.FindAnyObjectByType(type, FindObjectsInactive.Include);
                if (panel != null) {
                    RegisterPanel(panel);
                }
            }
#if DEVELOPMENT
            if (panel == null) {
                Assert.Fail("No shared panel object found for type '{0}'", type.FullName);
            }
#endif // DEVELOPMENT
            return panel;
        }

        /// <summary>
        /// Returns the shared panel object for the given type.
        /// This will assert if none is found.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetShared<T>() where T : class, ISharedGuiPanel {
            int index = PanelIndex.Get<T>();
            ISharedGuiPanel panel = m_SharedPanelMap[index];
            if (panel == null) {
                panel = (ISharedGuiPanel) GameObject.FindAnyObjectByType(typeof(T), FindObjectsInactive.Include);
                if (panel != null) {
                    RegisterPanel(panel);
                }
            }
#if DEVELOPMENT
            if (panel == null) {
                Assert.Fail("No shared panel object found for type '{0}'", typeof(T).FullName);
            }
#endif // DEVELOPMENT
            return (T) panel;
        }

        /// <summary>
        /// Fast unchecked retrieve.
        /// </summary>
        internal T FastGetShared<T>() where T : class, ISharedGuiPanel {
            return (T) m_SharedPanelMap[PanelIndex.Get<T>()];
        }

        /// <summary>
        /// Attempts to return the shared panel object for the given type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetShared(Type type, out IGuiPanel panel) {
            int index = PanelIndex.Get(type);
            panel = index < m_SharedPanelMap.Length ? m_SharedPanelMap[index] : null;
            return panel != null;
        }

        /// <summary>
        /// Attempts to return the shared panel object for the given type.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetShared<T>(out T panel) where T : class, IGuiPanel {
            int index = PanelIndex.Get<T>();
            panel = (T) (index < m_SharedPanelMap.Length ? m_SharedPanelMap[index] : null);
            return panel != null;
        }

        /// <summary>
        /// Looks up all panels that pass the given predicate.
        /// </summary>
        public int LookupSharedAll(Predicate<IGuiPanel> predicate, List<IGuiPanel> sharedPanels) {
            int found = 0;
            foreach (var panel in m_PanelSet) {
                if (predicate(panel)) {
                    sharedPanels.Add(panel);
                    found++;
                }
            }
            return found;
        }

        /// <summary>
        /// Looks up all panels that implement the given interface or class.
        /// </summary>
        public int LookupSharedAll<T>(List<T> sharedPanels) where T : class {
            int found = 0;
            foreach (var panel in m_PanelSet) {
                T casted = panel as T;
                if (casted != null) {
                    sharedPanels.Add(casted);
                    found++;
                }
            }
            return found;
        }

        /// <summary>
        /// Looks up all panels that pass the given predicate.
        /// </summary>
        public int LookupSharedAll<U>(Predicate<IGuiPanel, U> predicate, U predicateArg, List<IGuiPanel> sharedPanels) {
            int found = 0;
            foreach (var panel in m_PanelSet) {
                if (predicate(panel, predicateArg)) {
                    sharedPanels.Add(panel);
                    found++;
                }
            }
            return found;
        }

        #endregion // Shared

        #region Named

        /// <summary>
        /// Looks up a registered RectTransform by name.
        /// </summary>
        public RectTransform LookupNamed(StringHash32 name) {
            m_NamedElementMap.TryGetValue(name, out RectTransform rect);
            return rect;
        }

        #endregion // Named

        #endregion // Lookup

        #region Events

        internal void Initialize() {
            if (m_PrimaryUICamera == null) {
                SetPrimaryCamera(CameraUtility.FindMostSpecificCameraForLayer(LayerMask.NameToLayer("UI")));
            }
        }

        internal void ProcessUpdate() {
            for(int i = 0; i < m_UpdateCallbacks.Count; i++) {
                m_UpdateCallbacks[i].OnGuiUpdate();
            }
        }

        internal void Shutdown() {
            Array.Clear(m_SharedPanelMap, 0, m_SharedPanelMap.Length);
            m_PanelSet.Clear();
            m_NamedElementMap.Clear();
            m_UpdateCallbacks.Clear();
            m_InputMgr = null;
        }

        #endregion // Events
    }
}