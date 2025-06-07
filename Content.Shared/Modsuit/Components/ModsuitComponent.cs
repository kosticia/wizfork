using Robust.Shared.Prototypes;
using Robust.Shared.Containers;

namespace Content.Shared.Modsuit.Components;

[RegisterComponent]
public sealed partial class ModsuitComponent : Component
{
    [ViewVariables]
    public Container ModuleContainer;

    [DataField]
    public string ModuleContainerName = "ModsuitModuleContainer";

    [ViewVariables]
    public List<EntityUid> StoredActions = [];

    [DataField]
    public List<EntProtoId> StartingModules = [];

    /// <summary>
    ///     Maximum amount of modules in modsuit. Leave null for unlimited amount.
    /// </summary>
    [DataField]
    public int MaxModuleCount = 6;

    [DataField]
    public float PassiveDrain = 5;
}
