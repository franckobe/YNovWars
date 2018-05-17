/// <summary>
/// 
/// </summary>
public interface IGameboard
{
    /// <summary>
    /// The Id of the Neutral Homes (-1)
    /// </summary>
    /// <remarks>This is a constant value</remarks>
    int NeutralTeamId { get; }

    /// <summary>
    /// Returns all the existing Homes
    /// </summary>
    /// <remarks>You can trust this buffer over time</remarks>
    IHome[] Homes { get; }

    /// <summary>
    /// Selects Homes with the same TeamId
    /// </summary>
    /// <param name="teamId"></param>
    /// <param name="belongToTeam">Says if we want the Home of our Team or the ones from others</param>
    /// <returns></returns>
    /// <remarks>You can not trust this buffer over time, IHomes may see their Id change</remarks>
    IHome[] GetHomes(int teamId, bool belongToTeam);

    /// <summary>
    /// Those are the alive and visible Boldies on the map (not the ones in IHomes)
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    /// <remarks>You can not trust this buffer over time, IBoldies are recycled and reused with a different TeamId after it hits an IHome</remarks>
    IBoldi[] GetBoldies(int teamId);
    
    /// <summary>
    /// Helps to know the move speed of a IBoldi (units per seconds)
    /// </summary>
    /// <remarks>This is a constant value</remarks>
    float BoldiSpeed { get; }

    /// <summary>
    /// Global game time
    /// </summary>
    float GameTime { get; }
}