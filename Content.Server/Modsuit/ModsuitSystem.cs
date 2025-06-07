using Content.Server.Administration.Logs;
using Content.Server.PowerCell;
using Content.Shared.Modsuit.Components;
using Content.Shared.Modsuit.EntitySystems;
using Content.Shared.PowerCell.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using System.Diagnostics.CodeAnalysis;

namespace Content.Server.Modsuit;

public sealed partial class ModsuitSystem : SharedModsuitSystem
{
    [Dependency] private readonly IAdminLogManager _adminLog = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ModsuitComponent, ComponentInit>(OnInit);
    }

    private void OnInit(Entity<ModsuitComponent> ent, ref ComponentInit args)
    {
        ent.Comp.ModuleContainer = _container.EnsureContainer<Container>(ent, ent.Comp.ModuleContainerName);
        foreach (var moduleProto in ent.Comp.StartingModules)
        {
            if (TrySpawnNextTo(moduleProto, ent, out var moduleUid))
                TryInsertModule(moduleUid.Value, ent.Owner);
        }
    }

    public bool TryEjectPowerCell(EntityUid uid, [NotNullWhen(true)] out List<EntityUid>? ents)
    {
        ents = null;

        if (!TryComp<PowerCellSlotComponent>(uid, out var slotComp) ||
            !_container.TryGetContainer(uid, slotComp.CellSlotId, out var container) ||
            container.ContainedEntities is [])
        {
            return false;
        }

        ents = _container.EmptyContainer(container);

        return true;
    }
}
