using Content.Shared.Modsuit.Ui;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.Modsuit;

#pragma warning disable IDE0290

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

        _menu.EjectBatteryButtonPressed += () =>
        {
            SendMessage(new ModsuitEjectBatteryBuiMessage());
        };

        _menu.RemoveModuleButtonPressed += module =>
        {
            SendMessage(new ModsuitRemoveModuleBuiMessage(EntMan.GetNetEntity(module)));
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not ModsuitBuiState msg)
            return;
        _menu?.UpdateState(msg);
    }
}
