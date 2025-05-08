using Content.Server.Power.Components;
using Content.Server.PowerCell;
using Content.Server.Power.EntitySystems;

using Content.Shared.Examine;
using Content.Shared.Holosign;

using Robust.Shared.Spawners;

namespace Content.Server.Holosign;

public sealed class HolosignSystem : SharedHolosignSystem
{
    [Dependency] private readonly PowerCellSystem _powerCell = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly BatterySystem _battery = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HolosignProjectorComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<HolosignProjectorComponent, TrySpawnHolosigh>(TrySpawn);
        SubscribeLocalEvent<HolosignProjectorComponent, TryRefreshHolosigh>(TryRefresh);
        SubscribeLocalEvent<HolosignProjectorComponent, TryRemoveHolosigh>(TryRemove);
    }

    private void OnExamine(EntityUid uid, HolosignProjectorComponent component, ExaminedEvent args)
    {
        // TODO: This should probably be using an itemstatus
        // TODO: I'm too lazy to do this rn but it's literally copy-paste from emag.
        _powerCell.TryGetBatteryFromSlot(uid, out var battery);
        var charges = UsesRemaining(component, battery);
        var maxCharges = MaxUses(component, battery);

        using (args.PushGroup(nameof(HolosignProjectorComponent)))
        {
            args.PushMarkup(Loc.GetString("limited-charges-charges-remaining", ("charges", charges)));

            if (charges > 0 && charges == maxCharges)
            {
                args.PushMarkup(Loc.GetString("limited-charges-max-charges"));
            }
        }
    }

    private void TrySpawn(EntityUid uid, HolosignProjectorComponent component, ref TrySpawnHolosigh args)
    {
        if (!_powerCell.TryUseCharge(uid, component.ChargeUse, user: args.User)) // if no battery or no charge, doesn't work)
            return;

        // places the holographic sign at the click location, snapped to grid.
        // overlapping of the same holo on one tile remains allowed to allow holofan refreshes
        var holoUid = EntityManager.SpawnEntity(component.SignProto, args.Coordinates);

        var xform = Transform(holoUid);
        if (!xform.Anchored)
            _transform.AnchorEntity(holoUid, xform); // anchor to prevent any tempering with (don't know what could even interact with it)
    }

    private void TryRefresh(EntityUid uid, HolosignProjectorComponent component, ref TryRefreshHolosigh args)
    {
        if (TryComp<TimedDespawnComponent>(args.Target, out var despawn) && !_powerCell.TryUseCharge(uid, component.ChargeUse - despawn.Lifetime / 2, user: args.User)) // if no battery or no charge, doesn't work)
            return;

        var holoUid = EntityManager.SpawnEntity(component.SignProto, Transform(args.Target).Coordinates);
        QueueDel(args.Target);

        var xform = Transform(holoUid);
        if (!xform.Anchored)
            _transform.AnchorEntity(holoUid, xform); // anchor to prevent any tempering with (don't know what could even interact with it)
    }

    private void TryRemove(EntityUid uid, HolosignProjectorComponent component, ref TryRemoveHolosigh args)
    {
        if (!TryComp<TimedDespawnComponent>(args.Target, out var despawn))
            return;

        QueueDel(args.Target);
        if (_powerCell.TryGetBatteryFromSlot(uid, out var batteryEnt, out var battery))
            _battery.ChangeCharge(batteryEnt.Value, despawn.Lifetime / 2);
    }

    private int UsesRemaining(HolosignProjectorComponent component, BatteryComponent? battery = null)
    {
        if (battery == null ||
            component.ChargeUse == 0f) return 0;

        return (int)(battery.CurrentCharge / component.ChargeUse);
    }

    private int MaxUses(HolosignProjectorComponent component, BatteryComponent? battery = null)
    {
        if (battery == null ||
            component.ChargeUse == 0f) return 0;

        return (int)(battery.MaxCharge / component.ChargeUse);
    }
}
