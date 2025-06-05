using Robust.Shared.Containers;
using Content.Shared.Interaction;
using Content.Shared.Popups;

namespace Content.Shared.Modsuit;

public abstract class SharedModsuitSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ModsuitModuleComponent, AfterInteractEvent>(OnInteractModule);
    }

    private void OnInteractModule(Entity<ModsuitModuleComponent> ent, ref AfterInteractEvent args)
    {
        if (!args.CanReach || args.Target is null || args.Handled)
            return;

        args.Handled = TryInsertModule(ent.Owner, args.Target.Value, args.User);
    }

    public bool TryInsertModule(Entity<ModsuitModuleComponent?> module, Entity<ModsuitComponent?> target, EntityUid? user = null, ContainerManagerComponent? containerComp = null)
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
                        && comp.ProvidedActions == module.Comp.ProvidedActions
                        | comp.ProvidedComponents == module.Comp.ProvidedComponents)
                    {
                        if (user is not null)
                            _popup.PopupClient("asasada", target, user);
                        return false;
                    }
                }
                _container.Insert(module.Owner, container);
                target.Comp.Modules.Add(module);
                target.Comp.PassiveDrain += module.Comp.PassiveDrain;
                return true;
            }
        }
        else if (user is not null)
            _popup.PopupClient("many", target, user);

        return false;
    }
}
