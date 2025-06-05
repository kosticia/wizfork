using Robust.Shared.Prototypes;

namespace Content.Shared.Modsuit;

[RegisterComponent]
public sealed partial class ModsuitModuleComponent : Component
{
    [DataField]
    public float PassiveDrain = 5;

    [DataField]
    public float ActiveDrain = 10;

    [DataField]
    public List<EntProtoId> ProvidedActions = [];

    [DataField]
    public List<Component> ProvidedComponents = [];
}
