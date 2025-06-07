using Content.Shared.Database;
using Content.Shared.Modsuit.Components;
using Content.Shared.Silicons.Borgs;
using Content.Shared.UserInterface;

namespace Content.Server.Modsuit;

/// <inheritdoc/>
public sealed partial class ModsuitSystem
{
    public void InitializeUI()
    {
        SubscribeLocalEvent<ModsuitComponent, BeforeActivatableUIOpenEvent>(OnBeforeBorgUiOpen);
        SubscribeLocalEvent<ModsuitComponent, BorgEjectBatteryBuiMessage>(OnEjectBatteryBuiMessage);
        SubscribeLocalEvent<ModsuitComponent, BorgRemoveModuleBuiMessage>(OnRemoveModuleBuiMessage);
    }

    private void OnBeforeBorgUiOpen(Entity<ModsuitComponent> ent, ref BeforeActivatableUIOpenEvent args)
    {
        UpdateUI(ent);
    }

    private void OnEjectBatteryBuiMessage(Entity<ModsuitComponent> ent, ref BorgEjectBatteryBuiMessage args)
    {
        if (TryEjectPowerCell(ent, out var battery))
            _hands.TryPickupAnyHand(args.Actor, battery[0]);
    }

    private void OnRemoveModuleBuiMessage(Entity<ModsuitComponent> ent, ref BorgRemoveModuleBuiMessage args)
    {
        var module = GetEntity(args.Module);

        if (!ent.Comp.ModuleContainer.Contains(module))
            return;

        _adminLog.Add(LogType.Action, LogImpact.Medium,
            $"{ToPrettyString(args.Actor):player} removed module {ToPrettyString(module)} from modsuit {ToPrettyString(ent)}");
        _container.Remove(module, ent.Comp.ModuleContainer);
        _hands.TryPickupAnyHand(args.Actor, module);

        UpdateUI(ent);
    }

    public void UpdateUI(EntityUid uid)
    {
        var chargePercent = 0f;
        var hasBattery = false;
        if (_powerCell.TryGetBatteryFromSlot(uid, out var battery))
        {
            hasBattery = true;
            chargePercent = battery.CurrentCharge / battery.MaxCharge;
        }

        var state = new BorgBuiState(chargePercent, hasBattery);
        _ui.SetUiState(uid, BorgUiKey.Key, state);
    }
}
