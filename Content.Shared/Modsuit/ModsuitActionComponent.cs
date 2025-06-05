namespace Content.Shared.Modsuit;

[RegisterComponent]
public sealed partial class ModsuitActionComponent : Component
{
    [DataField]
    public float PassiveDrain = 5;

    [DataField]
    public float ActiveDrain = 10;
}
