using Content.Shared.Actions;
using Content.Shared.Thermals.Components;
using Content.Shared.Inventory.Events;

namespace Content.Shared.Thermals.Systems;

public sealed partial class ThermalsSystem : EntitySystem
{
    [Dependency] private readonly ActionContainerSystem _actions = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ThermalVisionGrantComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<ThermalVisionGrantComponent, GetItemActionsEvent>(OnEquip);
        SubscribeLocalEvent<ThermalVisionGrantComponent, GotUnequippedEvent>(OnUnequip);
    }

    public void OnMapInit(Entity<ThermalVisionGrantComponent> ent, ref MapInitEvent args)
    {
        if (_actions.AddAction(ent, ent.Comp.ActionProto) is { } actionUid)
            ent.Comp.ActionEntity = actionUid;
    }

    public void OnEquip(Entity<ThermalVisionGrantComponent> ent, ref GetItemActionsEvent args)
    {
        if (!args.InHands)
            _actions.TransferAction(ent.Comp.ActionEntity, args.User);
    }

    public void OnUnequip(Entity<ThermalVisionGrantComponent> ent, ref GotUnequippedEvent args)
    {
        _actions.TransferAction(ent.Comp.ActionEntity, ent);
    }
}
