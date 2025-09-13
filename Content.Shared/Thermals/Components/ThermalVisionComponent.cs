namespace Content.Shared.Thermals.Components;

[RegisterComponent]
public sealed partial class ThermalVisionComponent : Component
{
    [DataField]
    public float MinTemperatureThreshold = 260f;

    [DataField]
    public Color MinTemperatureColor = (0, 126, 255);

    [DataField]
    public float MaxTemperatureThreshold = 360f;

    [DataField]
    public Color MaxTemperatureColor = (255, 126, 0);
}
