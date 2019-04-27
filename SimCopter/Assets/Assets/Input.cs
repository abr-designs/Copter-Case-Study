// GENERATED AUTOMATICALLY FROM 'Assets/Input.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class Input : InputActionAssetReference
{
    public Input()
    {
    }
    public Input(InputActionAsset asset)
        : base(asset)
    {
    }
    [NonSerialized] private bool m_Initialized;
    private void Initialize()
    {
        // Helicopter
        m_Helicopter = asset.GetActionMap("Helicopter");
        m_Helicopter_Movement = m_Helicopter.GetAction("Movement");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Helicopter = null;
        m_Helicopter_Movement = null;
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
    private InputAction m_Helicopter_Movement;
    public struct HelicopterActions
    {
        private Input m_Wrapper;
        public HelicopterActions(Input wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement { get { return m_Wrapper.m_Helicopter_Movement; } }
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
