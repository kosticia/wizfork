using Content.Shared.Thermals.Components;

namespace Content.Shared.Thermals.Systems;

public sealed partial class ThermalsSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<ThermalVisionComponent, ComponentInit>(OnCompInit);
    }

    public void OnCompInit(Entity<ThermalVisionComponent> ent, ref ComponentInit args)
    {

    }
}
