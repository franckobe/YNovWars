/// <summary>
/// This represent a unit that are actually moving from an IHome to an other IHome
/// </summary>
public interface IBoldi : IPiece
{
    /// <summary>The destination home</summary>
    IHome Destination { get; }
}