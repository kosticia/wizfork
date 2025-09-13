using Content.Shared.Clothing.Components;
using Content.Shared.Gravity;
using Content.Shared.Inventory;
using Content.Shared.Standing;
using Content.Shared.Clothing.Components;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Shared.Clothing;
using Content.Shared.Actions;

using Content.Shared.Thermals.Components;

namespace Content.Shared.Thermals.Systems;

public sealed partial class ThermalsSystems : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ThermalVisionGrantComponent, GetItemActionsEvent>(OnEquip);
    }

    public void OnEquip(Entity<ThermalVisionGrantComponent> ent, ref GetItemActionsEvent args)
    {
        if (args.InHands)
            return;

        args.AddAction(ent.Comp.ActionEntity);
    }
}
