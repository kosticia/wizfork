using Content.Shared.Coordinates.Helpers;
using Content.Shared.Interaction;
using Content.Shared.Storage;

namespace Content.Shared.Holosign;

public abstract class SharedHolosignSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HolosignProjectorComponent, BeforeRangedInteractEvent>(OnBeforeInteract);
        SubscribeLocalEvent<HolosignProjectorComponent, ActivateInWorldEvent>(OnCurrentModChanged);
    }

    private void OnBeforeInteract(EntityUid uid, HolosignProjectorComponent comp, BeforeRangedInteractEvent args)
    {
        if (args.Handled
            || !args.CanReach // prevent placing out of range
            || HasComp<StorageComponent>(args.Target) // if it's a storage component like a bag, we ignore usage so it can be stored
            )
            return;

        if (TryComp<MetaDataComponent>(args.Target, out var meta) && meta.EntityPrototype is not null && meta.EntityPrototype == comp.SignProto)
        {
            switch (comp.CurrentMod)
            {
                case HolosignModes.Placing:
                    var incraseEvent = new TryRefreshHolosigh(args.Target.Value, args.User);
                    RaiseLocalEvent(uid, ref incraseEvent);
                    break;
                case HolosignModes.Removing:
                    var removeEvent = new TryRemoveHolosigh(args.Target.Value);
                    RaiseLocalEvent(uid, ref removeEvent);
                    break;
            }
        }

        else
        {
            var spawnEvent = new TrySpawnHolosigh(args.ClickLocation.SnapToGrid(EntityManager), args.User);
            RaiseLocalEvent(uid, ref spawnEvent);
        }

        args.Handled = true;
    }

    private void OnCurrentModChanged(EntityUid uid, HolosignProjectorComponent comp, ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        if (comp.CurrentMod == HolosignModes.Placing)
            comp.CurrentMod = HolosignModes.Removing;
        else
            comp.CurrentMod = HolosignModes.Placing;
    }
}
