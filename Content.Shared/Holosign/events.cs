using Robust.Shared.Map;

namespace Content.Shared.Holosign;

/// <summary>
/// Called just before we actually gib the target entity
/// </summary>
[ByRefEvent]
public record struct TrySpawnHolosigh(EntityCoordinates Coordinates, EntityUid User);

/// <summary>
/// Called just before we actually gib the target entity
/// </summary>
[ByRefEvent]
public record struct TryRemoveHolosigh(EntityUid Target);

/// <summary>
/// Called just before we actually gib the target entity
/// </summary>
[ByRefEvent]
public record struct TryRefreshHolosigh(EntityUid Target, EntityUid User);
