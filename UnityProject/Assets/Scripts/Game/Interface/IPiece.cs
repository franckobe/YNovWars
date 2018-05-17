using UnityEngine;

/// <summary>
/// This represents a visible object of the map (IBoldi or IHome at the time)
/// </summary>
public interface IPiece
{
    /// <summary>The Id of the owner</summary>
    int TeamId { get; }
    /// <summary>The current world position of the IPiece</summary>
    Vector3 Position { get; }
}