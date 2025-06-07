using Robust.Shared.Prototypes;

namespace Content.Shared.Modsuit.Components;

[RegisterComponent]
public sealed partial class ModsuitModuleComponent : Component
{
    [DataField]
    public float PassiveDrain = 5;

    [DataField]
    public List<EntProtoId> ProvidedActions = [];

    [DataField]
    public List<EntityUid> StoredActions = [];

    [DataField]
    public List<Component> ProvidedComponents = [];
}
