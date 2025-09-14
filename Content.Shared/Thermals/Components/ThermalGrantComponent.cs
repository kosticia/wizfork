using Robust.Shared.GameStates;
using Content.Shared.Thermals.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared.Thermals.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(ThermalsSystem))]
public sealed partial class ThermalVisionGrantComponent : Component
{
    [DataField]
    public EntProtoId ActionProto = "ThermalVisionToggleAction";

    [ViewVariables]
    public EntityUid ActionEntity;
}
