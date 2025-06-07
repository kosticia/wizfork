using Robust.Shared.Serialization;

namespace Content.Shared.Modsuit.Ui;

#pragma warning disable IDE0290

[Serializable, NetSerializable]
public enum ModsuitUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class ModsuitBuiState : BoundUserInterfaceState
{
    public float ChargePercent;

    public bool HasBattery;

    public ModsuitBuiState(float chargePercent, bool hasBattery)
    {
        ChargePercent = chargePercent;
        HasBattery = hasBattery;
    }
}

[Serializable, NetSerializable]
public sealed class ModsuitEjectBatteryBuiMessage : BoundUserInterfaceMessage
{

}

[Serializable, NetSerializable]
public sealed class ModsuitRemoveModuleBuiMessage : BoundUserInterfaceMessage
{
    public NetEntity Module;

    public ModsuitRemoveModuleBuiMessage(NetEntity module)
    {
        Module = module;
    }
}

