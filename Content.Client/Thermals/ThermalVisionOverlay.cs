using Content.Client.Movement.Systems;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Content.Shared.Eye.Blinding;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Thermals.Components;
using System.Numerics;
using Content.Client.StatusIcon;
using Content.Client.UserInterface.Systems;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using static Robust.Shared.Maths.Color;

namespace Content.Client.Thermals;

public sealed class ThermalVisionOverlay : Overlay
{
    private static readonly ProtoId<ShaderPrototype> ThermalShader = "ThermalShader";
    private static readonly ProtoId<ShaderPrototype> GreyscaleShader = "GreyscaleShader";

    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly ILightManager _lightManager = default!;

    public override bool RequestScreenTexture => true;
    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    private readonly ShaderInstance _greyscaleShader;
    private readonly ShaderInstance _thermalShader;

    private ThermalVisionComponent _thermalComp = default!;

    public ThermalVisionOverlay()
    {
        IoCManager.InjectDependencies(this);
        _greyscaleShader = _prototypeManager.Index(GreyscaleShader).InstanceUnique();
        _thermalShader = _prototypeManager.Index(ThermalShader).InstanceUnique();
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (!_entMan.TryGetComponent(_playerManager.LocalSession?.AttachedEntity, out EyeComponent? eyeComp))
            return false;

        if (args.Viewport.Eye != eyeComp.Eye)
            return false;

        var playerEntity = _playerManager.LocalSession?.AttachedEntity;

        if (playerEntity is null)
            return false;

        if (!_entMan.TryGetComponent<ThermalVisionComponent>(playerEntity, out var thermalComp))
            return false;

        _thermalComp = thermalComp;

        return true;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture is null)
            return;

        var playerEntity = _playerManager.LocalSession?.AttachedEntity;

        if (playerEntity is null)
            return;

        var handle = args.WorldHandle;
        var rotation = args.Viewport.Eye?.Rotation ?? Angle.Zero;
        var xformQuery = _entMan.GetEntityQuery<TransformComponent>();

        const float scale = 1f;
        var scaleMatrix = Matrix3Helpers.CreateScale(new Vector2(scale, scale));
        var rotationMatrix = Matrix3Helpers.CreateRotation(-rotation);

        var query = _entMan.AllEntityQueryEnumerator<TemperatureComponent, SpriteComponent>();
        while (query.MoveNext(out var uid,
            out var tempComp,
            out var spriteComp))
        {
            if (statusIcon != null && !_statusIconSystem.IsVisible((uid, _entMan.GetComponent<MetaDataComponent>(uid)), statusIcon))
                continue;

            // We want the stealth user to still be able to see his health bar himself
            if (!xformQuery.TryGetComponent(uid, out var xform) ||
                xform.MapID != args.MapId)
                continue;

            if (damageableComponent.DamageContainerID == null || !DamageContainers.Contains(damageableComponent.DamageContainerID))
                continue;

            // we use the status icon component bounds if specified otherwise use sprite
            var bounds = _entMan.GetComponentOrNull<StatusIconComponent>(uid)?.Bounds ?? _spriteSystem.GetLocalBounds((uid, spriteComponent));
            var worldPos = _transform.GetWorldPosition(xform, xformQuery);

            if (!bounds.Translated(worldPos).Intersects(args.WorldAABB))
                continue;

            // we are all progressing towards death every day
            if (CalcProgress(uid, mobStateComponent, damageableComponent, mobThresholdsComponent) is not { } deathProgress)
                continue;

            var worldPosition = _transform.GetWorldPosition(xform);
            var worldMatrix = Matrix3Helpers.CreateTranslation(worldPosition);

            var scaledWorld = Matrix3x2.Multiply(scaleMatrix, worldMatrix);
            var matty = Matrix3x2.Multiply(rotationMatrix, scaledWorld);

            handle.SetTransform(matty);

            var yOffset = bounds.Height * EyeManager.PixelsPerMeter / 2 - 3f;
            var widthOfMob = bounds.Width * EyeManager.PixelsPerMeter;

            var position = new Vector2(-widthOfMob / EyeManager.PixelsPerMeter / 2, yOffset / EyeManager.PixelsPerMeter);
            var color = GetProgressColor(deathProgress.ratio, deathProgress.inCrit);

            // Hardcoded width of the progress bar because it doesn't match the texture.
            const float startX = 8f;
            var endX = widthOfMob - 8f;

            var xProgress = (endX - startX) * deathProgress.ratio + startX;

            var boxBackground = new Box2(new Vector2(startX, 0f) / EyeManager.PixelsPerMeter, new Vector2(endX, 3f) / EyeManager.PixelsPerMeter);
            boxBackground = boxBackground.Translated(position);
            handle.DrawRect(boxBackground, Black.WithAlpha(192));

            var boxMain = new Box2(new Vector2(startX, 0f) / EyeManager.PixelsPerMeter, new Vector2(xProgress, 3f) / EyeManager.PixelsPerMeter);
            boxMain = boxMain.Translated(position);
            handle.DrawRect(boxMain, color);

            var pixelDarken = new Box2(new Vector2(startX, 2f) / EyeManager.PixelsPerMeter, new Vector2(xProgress, 3f) / EyeManager.PixelsPerMeter);
            pixelDarken = pixelDarken.Translated(position);
            handle.DrawRect(pixelDarken, Black.WithAlpha(128));
        }

        if (_entMan.TryGetComponent<ThermalVisionComponent>(playerEntity, out var comp))
        {
            _thermalShader?.SetParameter("MinTemp", (int)comp.MinTemperatureThreshold);
            _thermalShader?.SetParameter("MinTempColor", comp.MinTemperatureColor);
            _thermalShader?.SetParameter("MaxTemp", (int)comp.MaxTemperatureThreshold);
            _thermalShader?.SetParameter("MaxTempColor", comp.MaxTemperatureColor);
        }

        _greyscaleShader?.SetParameter("SCREEN_TEXTURE", ScreenTexture);

        var viewport = args.WorldBounds;
        handle.UseShader(_greyscaleShader);
        handle.UseShader(_thermalShader);
    }
}
