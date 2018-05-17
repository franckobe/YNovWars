using XKTools;

/// <summary>
/// 
/// </summary>
public class GameboardComp : XKObject
{
    #region Members

    /// <summary>
    /// 
    /// </summary>
    protected Gameboard         m_Gameboard         = null;

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