using Content.Shared.Examine;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Interaction;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Holosign;

public abstract class CreateEntityOnUseSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;


    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HolosignProjectorComponent, BeforeRangedInteractEvent>(OnBeforeInteract);
    }

    private void OnBeforeInteract(EntityUid uid, HolosignProjectorComponent comp, BeforeRangedInteractEvent args)
    {

        if (args.Handled
            || !args.CanReach // prevent placing out of range
            || HasComp<StorageComponent>(args.Target) // if it's a storage component like a bag, we ignore usage so it can be stored
            )
            return;

        switch (comp.CurrentRegime)
        {
            case HolosignModes.Placing:
            {
                if (TryComp<MetaDataComponent>(args.Target, out var meta) && meta.EntityPrototype is not null && meta.EntityPrototype == comp.SignProto)
                {
                    var incraseEvent = new TryIncraseHolosighLifetime(args.Target.Value);
                    RaiseLocalEvent(uid, ref incraseEvent);
                }
                else
                {
                    var spawnEvent = new TrySpawnHolosigh(args.ClickLocation.SnapToGrid(EntityManager));
                    RaiseLocalEvent(uid, ref spawnEvent);
                }

                break;
            }

            case HolosignModes.Removing:
            {
                if (TryComp<MetaDataComponent>(args.Target, out var meta) && meta.EntityPrototype is not null && meta.EntityPrototype == comp.SignProto)
                {
                    var incraseEvent = new TryRemoveHolosigh(args.Target.Value);
                    RaiseLocalEvent(uid, ref incraseEvent);
                }
                else
                {
                    var spawnEvent = new TrySpawnHolosigh(args.ClickLocation.SnapToGrid(EntityManager));
                    RaiseLocalEvent(uid, ref spawnEvent);
                }

                break;
            }
        }



        args.Handled = true;
    }
}
