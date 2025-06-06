using Content.Shared.Modsuit.EntitySystems;
using Content.Shared.Modsuit;
using Content.Server.Actions;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Body.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Hands.Systems;
using Content.Server.PowerCell;
using Content.Shared.Alert;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Pointing;
using Content.Shared.PowerCell;
using Content.Shared.PowerCell.Components;
using Content.Shared.Roles;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Content.Shared.Wires;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Content.Server.Modsuit;

public sealed partial class ModsuitSystem : SharedModsuitSystem
{
    [Dependency] private readonly IAdminLogManager _adminLog = default!;
    [Dependency] private readonly IBanManager _banManager = default!;
    [Dependency] private readonly IConfigurationManager _cfgManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly DeviceNetworkSystem _deviceNetwork = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly TriggerSystem _trigger = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifier = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelistSystem = default!;
    [Dependency] private readonly ISharedPlayerManager _player = default!;

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
            !container.ContainedEntities.Any())
        {
            return false;
        }

        ents = _container.EmptyContainer(container);

        return true;
    }
}
