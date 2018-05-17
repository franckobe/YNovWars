using XKTools;

/// <summary>
/// 
/// </summary>
public class GameboardCompInterfaced : XKObject
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    protected IGameboard        m_Gameboard         = null;

    #endregion


    #region Inherited Manipulators

    /// <summary>
    /// 
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        m_Gameboard = FindXKParent<Gameboard>();
        m_Gameboard.IsValid("GameboardComp.Gameboard");
    }

    #endregion


    #region Private Manipulators
    #endregion
}