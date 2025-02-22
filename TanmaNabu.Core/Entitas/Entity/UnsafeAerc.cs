﻿namespace Entitas;

/// Automatic Entity Reference Counting (AERC)
/// is used internally to prevent pooling retained entities.
/// If you use retain manually you also have to
/// release it manually at some point.
/// UnsafeAERC doesn't check if the entity has already been
/// retained or released. It's faster, but you lose the information
/// about the owners.
public sealed class UnsafeAerc : IAerc
{
    public int RetainCount { get; private set; }

    public void Retain(object owner) => RetainCount += 1;

    public void Release(object owner) => RetainCount -= 1;
}