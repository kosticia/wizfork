using Robust.Shared.Containers;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Actions;
using Content.Shared.Clothing;
using Content.Shared.Hands.EntitySystems;

namespace Content.Shared.Modsuit;

public abstract class SharedModsuitSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly ActionContainerSystem _actionSystem = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ModsuitModuleComponent, AfterInteractEvent>(OnInteractModule);
        SubscribeLocalEvent<ModsuitComponent, GetItemActionsEvent>(OnGetActions);
        SubscribeLocalEvent<ModsuitComponent, ClothingGotUnequippedEvent>(OnUnequip);
        SubscribeLocalEvent<ModsuitModuleComponent, ComponentInit>(OnModuleInit);
    }

    private void OnModuleInit(Entity<ModsuitModuleComponent> ent, ref ComponentInit args)
    {
        foreach (var actionProto in ent.Comp.ProvidedActions)
        {
            var actionUid = _actionSystem.AddAction(ent, actionProto);
            if (actionUid is null)
                continue;
            ent.Comp.StoredActions.Add(actionUid.Value);
        }
    }

    private void OnInteractModule(Entity<ModsuitModuleComponent> ent, ref AfterInteractEvent args)
    {
        if (!args.CanReach || args.Target is null || args.Handled)
            return;

        args.Handled = TryInsertModule(ent.Owner, args.Target.Value, args.User);
    }

    private void OnGetActions(Entity<ModsuitComponent> ent, ref GetItemActionsEvent args)
    {
        if (args.InHands)
            return;

        foreach (var action in ent.Comp.StoredActions)
            _actionSystem.TransferAction(action, args.Provider);
    }

    private void OnUnequip(Entity<ModsuitComponent> ent, ref ClothingGotUnequippedEvent args)
    {
        foreach (var action in ent.Comp.StoredActions)
            _actionSystem.TransferAction(action, ent);
    }

    public bool TryInsertModule(Entity<ModsuitModuleComponent?> module,
        Entity<ModsuitComponent?> target,
        EntityUid? user = null,
        ContainerManagerComponent? containerComp = null)
    {
        if (!Resolve(target, ref target.Comp) || !Resolve(module, ref module.Comp))
            return false;

        // Inserts module only if max cap of modules is limited and there is avaible space.
        if (target.Comp.MaxModuleCount is not null && target.Comp.Modules.Count < target.Comp.MaxModuleCount
            || target.Comp.MaxModuleCount is null)
        {
            if (Resolve(target, ref containerComp)
                && containerComp.Containers.TryGetValue(target.Comp.ModuleContainer, out var container)
                && container is not null)
            {
                foreach (var contModule in target.Comp.Modules)
                {
                    if (TryComp<ModsuitModuleComponent>(contModule, out var comp)
                        && comp.ProvidedActions is not [] & comp.ProvidedActions == module.Comp.ProvidedActions
                        | comp.ProvidedComponents is not [] & comp.ProvidedComponents == module.Comp.ProvidedComponents)
                    {
                        if (user is not null)
                            _popup.PopupClient("asasada", target, user);
                        return false;
                    }
                }

                _container.Insert(module.Owner, container);

                target.Comp.Modules.Add(module);
                target.Comp.PassiveDrain += module.Comp.PassiveDrain;

                foreach (var action in module.Comp.StoredActions)
                {
                    _actionSystem.TransferAction(action, target);
                    target.Comp.StoredActions.Add(action);
                }

                return true;
            }
        }
        else if (user is not null)
            _popup.PopupClient("many", target, user);

        return false;
    }

    public bool TryDrawModule(Entity<ModsuitModuleComponent?> module,
        Entity<ModsuitComponent?> target,
        EntityUid? user = null,
        ContainerManagerComponent? containerComp = null)
    {
        if (!Resolve(target, ref target.Comp) || !Resolve(module, ref module.Comp))
            return false;

        if (Resolve(target, ref containerComp)
            && containerComp.Containers.TryGetValue(target.Comp.ModuleContainer, out var container)
            && container is not null)
        {
            _xform.DropNextTo(module.Owner, target.Owner);

            if (user is not null)
                _hands.TryPickupAnyHand(target, user.Value);

            foreach (var action in module.Comp.StoredActions)
            {
                _actionSystem.TransferAction(action, module);
                target.Comp.StoredActions.Remove(action);
            }
        }
        return true;
    }
}
