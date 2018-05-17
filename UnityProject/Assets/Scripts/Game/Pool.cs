using System.Collections.Generic;
using UnityEngine;
using XKTools;

/// <summary>
/// 
/// </summary>
public class Pool : GameboardComp
{
    #region Members
    
    Stack<Boldi>                    m_Boldies           = new Stack<Boldi>();

    #endregion


    #region Inherited Manipulators

    /// <summary>
    /// 
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

    }

    #endregion


    #region Public Manipulators

    /// <summary>
    /// 
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    public Boldi GetBoldi(int teamId)
    {
        Boldi res = GetBoldi();

        // prepare boldi
        res.TeamId = teamId;

        res.SetActive(true);
        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="boldi"></param>
    public void ReturnBoldi(Boldi boldi)
    {
        boldi.SetActive(false);
        m_Boldies.Push(boldi);
    }

    #endregion


    #region Private Manipulators

    Boldi GetBoldi()
    {
        if (m_Boldies.Count > 0)
            return m_Boldies.Pop();
        return m_Gameboard.CreateBoldi();
    }
    
    #endregion
}