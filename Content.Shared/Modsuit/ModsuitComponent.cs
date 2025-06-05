using Robust.Shared.Prototypes;

namespace Content.Shared.Modsuit;

[RegisterComponent]
public sealed partial class ModsuitComponent : Component
{
    [DataField]
    public List<EntityUid> Modules = [];

    [ViewVariables]
    public List<EntityUid> StoredActions = [];

    [DataField]
    public List<EntProtoId> StartingModules = [];

    [DataField]
    public string ModuleContainer = "ModsuitModuleContainer";

    /// <summary>
    ///     Maximum amount of modules in modsuit. Leave null for unlimited amount.
    /// </summary>
    [DataField]
    public int? MaxModuleCount = 6;

    [DataField]
    public float PassiveDrain = 5;
}
