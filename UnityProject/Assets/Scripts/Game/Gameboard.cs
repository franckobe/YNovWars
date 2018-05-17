using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XKTools;

/// <summary>
/// 
/// </summary>
public class Gameboard : XKObject, IGameboard
{
    #region Delegates

    /// <summary>
    /// 
    /// </summary>
    /// <param name="leaderboard">Ordered from the winner to the first loser</param>
    public delegate void OnGameOverDlg(List<AIBase> leaderboard);

    /// <summary></summary>
    public OnGameOverDlg OnGameOver { get; set; }

    #endregion


    #region Members

    /// <summary>
    /// 
    /// </summary>
    public const int    c_NeutralTeamId     = -1;
    const int           c_GridSize          = 5;
    /// <summary>
    /// 
    /// </summary>
    public const int    c_MinHomeCount      = c_GridSize * 2;
    const float         c_BoldiSpeed        = 10.0f;
    Camera              m_Camera            = null;

    Pool                m_Pool              = null;

    Transform           m_Root              = null;
    Transform           m_HomeRoot          = null;
    Transform           m_BoldiRoot         = null;
    List<Home>          m_Homes             = new List<Home>();
    List<IHome>         m_TmpHomes          = new List<IHome>();
    List<int>           m_HomelessTeams     = new List<int>();
    List<IBoldi>        m_TmpBoldies        = new List<IBoldi>();
    IHome[]             m_IHomes            = null;
    List<Boldi>         m_Boldies           = new List<Boldi>();
    List<AIBase>        m_AIs               = new List<AIBase>();
    List<AIBase>        m_AliveAIs          = new List<AIBase>();
    List<AIBase>        m_Leaderboard       = new List<AIBase>();

    Text                m_HomeTemplate      = null;
    Text                m_GameTimeText      = null;
    float               m_GameTime          = 0.0f;

    #endregion


    #region Inherited Manipulators

    /// <summary>
    /// 
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        // create gameboard elements
        FindCamera();
        FindHomeTemplate();
        FindGameTime();
        CreateRoots();
        CreatePool();
        CreateMap();

        // deactivate gameboard, will be reactivated from StartGame()
        XKActive = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Update()
    {
        base.Update();

        m_GameTime += Time.deltaTime;
        SetGameTimeText();

        DetectDeadAI();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Shutdown()
    {
        base.Shutdown();

        // delete gameobjects
        Finder.DestroyRoot(ERoot.Gameboard);

        // nullify pointers
        m_Pool = null;
    }

    #endregion


    #region Private Manipulators

    void FindCamera()
    {
        m_Camera = Camera.main;
    }

    void FindHomeTemplate()
    {
        GameObject obj = GameObject.Find("HomeTemplate");
        if (obj.IsValid("Gameboard.HomeTemplate"))
        {
            m_HomeTemplate = obj.GetComponent<Text>();
            if (m_HomeTemplate.IsValid("Gameboard.HomeTemplate.Text"))
                m_HomeTemplate.text = "";
        }
    }

    void FindGameTime()
    {
        GameObject obj = GameObject.Find("GameTime");
        if (obj.IsValid("Gameboard.GameTime"))
        {
            m_GameTimeText = obj.GetComponent<Text>();
            if (m_GameTimeText.IsValid("Gameboard.GameTime.Text"))
                SetGameTimeText();
        }
    }

    void SetGameTimeText()
    {
        if (m_GameTimeText != null)
            m_GameTimeText.text = m_GameTime.ToString("0.0");
    }

    void CreateRoots()
    {
        m_Root = Finder.CreateRoot(ERoot.Gameboard);
        CreateRoot(ref m_HomeRoot, "Homes"); 
        CreateRoot(ref m_BoldiRoot, "Boldies");
    }

    void CreateRoot(ref Transform root, string rootName)
    {
        root = new GameObject(rootName).transform;
        root.parent = m_Root;
    }

    void CreatePool()
    {
        m_Pool = AddXKComponent<Pool>();
    }

    void CreateMap()
    {
        if (m_Camera.orthographic)
        {
            Vector3 bounds = Vector3.zero;
            bounds.y = m_Camera.orthographicSize;
            bounds.x = bounds.y * Screen.width / Screen.height;
            bounds.x *= 0.85f; // keep 15 % off
            bounds.y *= 0.8f; // keep 20 % off

            CreateHomes(bounds);
        }
        else
        {
            XKLog.Log("Error", "Gameboard.CreateMap() failed - camera is not orthographic");
        }
    }

    void CreateHomes(Vector3 bounds)
    {
        // create random buffer
        IntBufferedRandom rnd = new IntBufferedRandom();
        rnd.AddValueRange(0, c_GridSize * c_GridSize);
        rnd.Range = Lehmer.Range;

        // create homes
        int homeCount = Lehmer.Range(c_MinHomeCount, c_GridSize * c_GridSize);
        for (int i = 0; i < homeCount; ++i)
            CreateHome(GetPosition(rnd.DrawValue(), bounds));
    }

    Vector3 GetPosition(int idx, Vector3 bounds)
    {
        Vector3 res = Vector3.zero;

        int h = idx / c_GridSize;
        int w = idx - h * c_GridSize;

        res.x = -bounds.x + w * bounds.x * 2.0f / (c_GridSize - 1);
        res.y = -bounds.y + h * bounds.y * 2.0f / (c_GridSize - 1);

        return res;
    }
    
    void DetectDeadAI()
    {
        IHome[] homes;
        IBoldi[] boldies;

        int i = 0, teamId;
        while (i < m_HomelessTeams.Count)
        {
            teamId = m_HomelessTeams[i];

            // check if the team has revived thanks to its last active boldies
            homes = GetHomes(teamId);
            if (homes.Length > 0)
            {
                m_HomelessTeams.RemoveAt(i);
                continue;
            }

            // check remaining boldies, they could still take a new Home
            boldies = GetBoldies(teamId);
            if (boldies.Length == 0)
            {
                m_HomelessTeams.RemoveAt(i);
                OnAIDied(teamId);
                continue;
            }

            i++;
        }
    }

    #endregion


    #region Public Manipulators

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xkActive"></param>
    /// <returns></returns>
    public T CreatePiece<T>(bool xkActive = true)
        where T : Piece, new()
    {
        T res = AddXKComponent<T>(xkActive);
        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Home CreateHome(Vector3 position)
    {
        Home res = CreatePiece<Home>();
        m_Homes.Add(res);

        res.SetParent(m_HomeRoot);
        res.SetPosition(position);
        res.TeamId = NeutralTeamId;
        res.Id = m_HomeRoot.childCount - 1;

        if (m_HomeTemplate != null)
        {
            GameObject text = Object.Instantiate(m_HomeTemplate.gameObject);
            text.transform.SetParent(m_HomeTemplate.transform.parent, false);
            text.transform.position = position.NoZ();
            text.transform.localPosition = text.transform.localPosition.NoZ();
            text.name = "Counter_" + res.Id.ToString();
            res.SetBoldiCountText(text.GetComponent<Text>());
        }

        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Boldi CreateBoldi()
    {
        Boldi res = CreatePiece<Boldi>();
        m_Boldies.Add(res);

        res.SetParent(m_BoldiRoot);

        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T CreateAI<T>()
        where T : AIBase, new()
    {
        T res = AddXKComponent<T>();
        res.TeamId = m_AIs.Count;
        m_AIs.Add(res);
        m_AliveAIs.Add(res);

        // find a start home!
        IGameboard gb = (this);
        IHome[] homes = gb.GetHomes(-1, true);
        if (homes.Length > 0)
        {
            ((Home)homes[0]).AttributeAI();
            ((Home)homes[0]).TeamId = res.TeamId;
        }

        return res;
    }

    /// <summary>
    /// Called when an ai launches bodies (my self included)
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void OnBoldiLaunch(IHome from, IHome to)
    {
        for (int i = 0; i < m_AliveAIs.Count; ++i)
            m_AliveAIs[i].OnBoldiLaunch(from, to);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="home"></param>
    /// <param name="formerTeamId"></param>
    public virtual void OnHomeChangedOwner(IHome home, int formerTeamId)
    {
        for (int i = 0; i < m_AliveAIs.Count; ++i)
            m_AliveAIs[i].OnHomeChangedOwner(home, formerTeamId);
    }

    /// <summary>
    /// 
    /// </summary>
    public void StartGame()
    {
        XKActive = true;
        for (int i = 0; i < m_AIs.Count; ++i)
            m_AIs[i].OnGameStart();
    }

    #endregion


    #region Public Accessors

    /// <summary>
    /// 
    /// </summary>
    public Camera Camera
    {
        get { return m_Camera; }
    }

    /// <summary>
    /// 
    /// </summary>
    public Pool Pool
    {
        get { return m_Pool; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    public Color GetColor(int teamId)
    {
        switch (teamId)
        {
            case 0:
                return Color.red;
            case 1:
                return Color.green;
            case 2:
                return Color.blue;
            case 3:
                return Color.yellow;
            case 4:
                return Color.magenta;
            case 5:
                return Color.cyan;
            case 6:
                return Color.white;
        }

        return Color.grey;
    }

    #endregion


    #region IGameboard Implementation

    /// <summary>
    /// 
    /// </summary>
    public int NeutralTeamId
    {
        get { return c_NeutralTeamId; }
    }

    /// <summary>
    /// 
    /// </summary>
    public IHome[] Homes
    {
        get
        {
            if (m_IHomes == null)
            {
                List<IHome> homes = new List<IHome>();
                foreach (Home home in m_Homes)
                    homes.Add(home);
                m_IHomes = homes.ToArray();
            }
            return m_IHomes;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="teamId"></param>
    /// <param name="belongToTeam"></param>
    /// <returns></returns>
    public IHome[] GetHomes(int teamId, bool belongToTeam = true)
    {
        m_TmpHomes.Clear();
        for (int i = 0; i < m_Homes.Count; ++i)
        {
            if (belongToTeam)
            {
                if (m_Homes[i].TeamId == teamId)
                    m_TmpHomes.Add(m_Homes[i]);
            }
            else
            {
                if (m_Homes[i].TeamId != teamId)
                    m_TmpHomes.Add(m_Homes[i]);
            }
        }
        return m_TmpHomes.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="teamId"></param>
    /// <returns></returns>
    public IBoldi[] GetBoldies(int teamId)
    {
        m_TmpBoldies.Clear();
        for (int i = 0; i < m_Boldies.Count; ++i)
        {
            if (m_Boldies[i].XKActive
             && m_Boldies[i].TeamId == teamId)
                m_TmpBoldies.Add(m_Boldies[i]);
        }
        return m_TmpBoldies.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="teamId"></param>
    public void OnHomelessTeam(int teamId)
    {
        m_HomelessTeams.Add(teamId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="teamId"></param>
    void OnAIDied(int teamId)
    {
        AIBase ai = m_AIs[teamId];

        // move ai from alive buffer to leaderboard
        m_AliveAIs.Remove(ai);
        m_Leaderboard.Insert(0, ai);

        // deactivate ai
        ai.XKActive = false;
        XKLog.Log("Info", string.Format("An AI has died: {0} ({1}) ", ai.GetType().ToString(), ai.TeamId));

        if (m_AliveAIs.Count == 1)
        {
            // store the winner
            m_Leaderboard.Insert(0, m_AliveAIs[0]);

            // deactivate gameboard, no need to keep it active
            XKActive = false;
            XKLog.Log("Info", string.Format("The last AI has died, the winner is: {0} ({1}) ", m_AliveAIs[0].GetType().ToString(), m_AliveAIs[0].TeamId));

            // announce the game is over to whom ants to listen
            if (OnGameOver != null)
                OnGameOver(m_Leaderboard);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public float BoldiSpeed
    {
        get { return c_BoldiSpeed; }
    }

    /// <summary>
    /// 
    /// </summary>
    public float GameTime
    {
        get { return m_GameTime; }
    }

    #endregion
}