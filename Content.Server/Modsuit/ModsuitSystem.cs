using Content.Shared.Modsuit;

namespace Content.Server.Modsuit;

public sealed class ModsuitSystem : SharedModsuitSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ModsuitComponent, ComponentInit>(OnInit);
    }

    private void OnInit(Entity<ModsuitComponent> ent, ref ComponentInit args)
    {
        foreach (var moduleProto in ent.Comp.StartingModules)
        {
            if (TrySpawnNextTo(moduleProto, ent, out var moduleUid))
                TryInsertModule(moduleUid.Value, ent.Owner);
        }
    }
}
