// GENERATED AUTOMATICALLY FROM 'Assets/HelicopterControls.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class HelicopterControls : InputActionAssetReference
{
    public HelicopterControls()
    {
    }
    public HelicopterControls(InputActionAsset asset)
        : base(asset)
    {
    }
    [NonSerialized] private bool m_Initialized;
    private void Initialize()
    {
        // Helicopter
        m_Helicopter = asset.GetActionMap("Helicopter");
        m_Helicopter_Move = m_Helicopter.GetAction("Move");
        m_Helicopter_Turn = m_Helicopter.GetAction("Turn");
        m_Helicopter_Climb = m_Helicopter.GetAction("Climb");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Helicopter = null;
        m_Helicopter_Move = null;
        m_Helicopter_Turn = null;
        m_Helicopter_Climb = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Helicopter
    private InputActionMap m_Helicopter;
    private InputAction m_Helicopter_Move;
    private InputAction m_Helicopter_Turn;
    private InputAction m_Helicopter_Climb;
    public struct HelicopterActions
    {
        private HelicopterControls m_Wrapper;
        public HelicopterActions(HelicopterControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move { get { return m_Wrapper.m_Helicopter_Move; } }
        public InputAction @Turn { get { return m_Wrapper.m_Helicopter_Turn; } }
        public InputAction @Climb { get { return m_Wrapper.m_Helicopter_Climb; } }
        public InputActionMap Get() { return m_Wrapper.m_Helicopter; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(HelicopterActions set) { return set.Get(); }
    }
    public HelicopterActions @Helicopter
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new HelicopterActions(this);
        }
    }
    private int m_NewcontrolschemeSchemeIndex = -1;
    public InputControlScheme NewcontrolschemeScheme
    {
        get

        {
            if (m_NewcontrolschemeSchemeIndex == -1) m_NewcontrolschemeSchemeIndex = asset.GetControlSchemeIndex("New control scheme");
            return asset.controlSchemes[m_NewcontrolschemeSchemeIndex];
        }
    }
}
