using System;
using System.Collections.Generic;
using UnityEngine;
using XKTools;

/// <summary>
/// 
/// </summary>
public class Piece : GameboardComp, IPiece
{
    #region Members
    
    static Dictionary<Type, int>        s_CreatedObjectCounter          = new Dictionary<Type, int>();

    Transform                           m_Root                          = null;
    Renderer                            m_Renderer                      = null;

    int                                 m_TeamId                        = -1;

    #endregion


    #region Inherited Manipulators

    /// <summary>
    /// 
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        
        CreateRenderer();
    }

    #endregion


    #region Public Manipulators

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Material GetMaterial()
    {
        if (m_Renderer != null)
           return m_Renderer.sharedMaterial;
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mat"></param>
    public void ApplyMaterial(Material mat)
    {
        if (m_Renderer != null)
            m_Renderer.sharedMaterial = mat;
    }

    #endregion


    #region Protected Manipulators

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected GameObject LoadGameObject()
    {
        string path = string.Format("Game/Piece/{0}", GetType().ToString().ClassNameClean());
        GameObject source = Resources.Load<GameObject>(path);
        if (source.IsValid("Piece.LoadGameObject() - " + path))
        {
            GameObject res = GameObject.Instantiate(source);
            res.name = source.name + "_" + GetCreatedObjectCount();
            return res;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void CreateRenderer()
    {
        GameObject obj = LoadGameObject();
        if (obj != null)
        {
            m_Root = obj.transform;
            m_Renderer = m_Root.GetComponent<Renderer>();
            m_Renderer.IsValid("Piece.CreateRenderer().Renderer");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="materials"></param>
    protected void SetMaterial(Dictionary<int, Material> materials)
    {
        // add material if needed
        if (!materials.ContainsKey(TeamId))
        {
            Material mat = GetMaterial();
            if (mat != null)
            {
                mat = GameObject.Instantiate(mat);
                mat.color = m_Gameboard.GetColor(TeamId);
                materials[TeamId] = mat;
            }
            else
                return;
        }

        ApplyMaterial(materials[TeamId]);
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnSetTeamId()
    {

    }

    #endregion


    #region Private Manipulators

    int GetCreatedObjectCount(bool increment = true)
    {
        if (!s_CreatedObjectCounter.ContainsKey(GetType()))
            s_CreatedObjectCounter[GetType()] = 0;

        if (increment)
            s_CreatedObjectCounter[GetType()]++;

        return s_CreatedObjectCounter[GetType()];
    }

    #endregion


    #region Public Accessors

    /// <summary>
    /// 
    /// </summary>
    public int TeamId
    {
        get { return m_TeamId; }
        set
        {
            m_TeamId = value;
            OnSetTeamId();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public Transform Root
    {
        get { return m_Root; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void SetActive(bool b)
    {
        XKActive = b;

        if (m_Root != null)
            m_Root.gameObject.SetActive(b);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    public void SetParent(Transform parent)
    {
        if (m_Root != null)
            m_Root.parent = parent;
    }

    /// <summary>
    /// 
    /// </summary>
    public Vector3 Position
    {
        get
        {
            if (m_Root != null)
                return m_Root.position;

            return Vector3.zero;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(Vector3 position)
    {
        if (m_Root != null)
            m_Root.position = position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scale"></param>
    public void SetScale(Vector3 scale)
    {
        if (m_Root != null)
            m_Root.localScale = scale;
    }

    #endregion
}