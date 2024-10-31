using System;

namespace Entitas;

/// Base exception used by Entitas.
public class BaseEntitasException(string message, string hint)
    : Exception(hint != null ? $"{message}\n{hint}" : message);