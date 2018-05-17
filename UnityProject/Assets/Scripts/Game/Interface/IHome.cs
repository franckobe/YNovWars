/// <summary>
/// This represents a basement from which we can launch IBoldies
/// </summary>
public interface IHome : IPiece
{
    /// <summary>The amount of launchabe IBoldies stored at this IHome</summary>
    int BoldiCount { get; }
    /// <summary>The IBoldies production rate (this does not depends on the amount of IBoldies in IHomes)</summary>
    float GrowRate { get; }
    /// <summary>
    /// This functions launches Boldies from a Home to an other
    /// </summary>
    /// <param name="destination">The destination place, could be neutral, friend or enemy</param>
    /// <param name="amount">The amount of IBoldies to launch to the destination</param>
    /// <param name="ai">Helps detecting cheaters AI</param>
    /// <returns></returns>
    bool LaunchBoldies(IHome destination, EAmount amount, AIBase ai);
}