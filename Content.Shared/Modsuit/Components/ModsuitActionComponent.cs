namespace Content.Shared.Modsuit.Components;

[RegisterComponent]
public sealed partial class ModsuitActionComponent : Component
{
    [DataField]
    public float ActiveDrain = 10;

    [ViewVariables]
    public EntityUid Modsuit;
}
