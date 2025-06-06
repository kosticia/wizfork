using Content.Shared.Silicons.Borgs;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.Modsuit;

[UsedImplicitly]
public sealed class ModsuitBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private ModsuitMenu? _menu;

    public ModsuitBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<ModsuitMenu>();
        _menu.SetEntity(Owner);

        _menu.BrainButtonPressed += () =>
        {
            SendMessage(new BorgEjectBrainBuiMessage());
        };

        _menu.EjectBatteryButtonPressed += () =>
        {
            SendMessage(new BorgEjectBatteryBuiMessage());
        };

        _menu.NameChanged += name =>
        {
            SendMessage(new BorgSetNameBuiMessage(name));
        };

        _menu.RemoveModuleButtonPressed += module =>
        {
            SendMessage(new BorgRemoveModuleBuiMessage(EntMan.GetNetEntity(module)));
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not BorgBuiState msg)
            return;
        _menu?.UpdateState(msg);
    }
}
