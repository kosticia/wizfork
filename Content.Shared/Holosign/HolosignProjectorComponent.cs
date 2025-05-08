using Robust.Shared.Prototypes;

namespace Content.Shared.Holosign;

/// <summary>
///     Allows entity create other entityes by clicking.
/// </summary>
[RegisterComponent]
public sealed partial class HolosignProjectorComponent : Component
{
    /// <summary>
    ///     ProtoId for entity that will be spawned.
    /// </summary>
    [DataField]
    public EntProtoId SignProto = "HolosignWetFloor";

    /// <summary>
    ///     How much charge a single use expends.
    /// </summary>
    [DataField]
    public float ChargeUse = 50;

    /// <summary>
    ///     Current mode of the component owner.
    /// </summary>
    [ViewVariables]
    public HolosignModes CurrentMod = HolosignModes.Placing;
}

public enum HolosignModes : byte
{
    Placing,
    Removing
}
