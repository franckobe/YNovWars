using System.Collections.Generic;
using UnityEngine;
using XKTools;

/// <summary>
/// 
/// </summary>
public class MatchMaker : XKBehaviour
{
    #region Members

    const int                       c_AICount           = 5;//27;

    Gameboard                       m_Gameboard         = null;

    [SerializeField]
    int                             m_Seed              = -1;

    [SerializeField]
    float                           m_TimeScale         = 1.0f;
    
    [SerializeField]
    int                             m_GamesPerAI        = 3;

    IntBufferedRandom               m_AIPicker          = null;
    int                             m_AIPerMatch        = 3;
    Dictionary<int, int>            m_TeamIdToId        = new Dictionary<int, int>();
    Dictionary<int, int>            m_IdToScore         = new Dictionary<int, int>();
    Dictionary<int, string>         m_IdToName          = new Dictionary<int, string>();

    int                             m_PlayedAIs         = 0;
    float                           m_AskRestart        = -1.0f;

    #endregion


    #region Inherited Manipulators

    /// <summary>
    /// 
    /// </summary>
    protected override void Start()
    {
        base.Start();

        m_AIPicker = ComponentContainer.AddXKComponent<IntBufferedRandom>();
        m_AIPicker.AddValueRange(0, c_AICount);

        EnableLogs();
        InitSeed();

        CreateMatch();
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // reset time scale
        Time.timeScale = m_TimeScale;

        // wait for timer to end and restart
        if (m_AskRestart >= 0.0f)
        {
            m_AskRestart -= Time.deltaTime;
            if (m_AskRestart < 0.0f)
                CreateMatch();
        }

        // restart game with space button
        if (m_Gameboard != null && !m_Gameboard.XKActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CreateMatch();
            }
        }
    }

    #endregion


    #region Private Manipulators

    void InitSeed()
    {
        // set seed for easy tests
        if (m_Seed > 0)
            Lehmer.Seed = m_Seed;
    }

    void LogSeed()
    {
        XKLog.LogWithContext("Info", "YnovWars.Seed: " + Lehmer.Seed, this);
    }

    void EnableLogs()
    {
        XKLog.EnableLogType("Error", true);
        XKLog.EnableLogType("Info", true);
    }

    void CreateGameboard()
    {
        LogSeed();
        m_Gameboard = ComponentContainer.AddXKComponent<Gameboard>();
        m_Gameboard.OnGameOver += OnGameOver;
    }

    void DeleteGameboard()
    {
        if (m_Gameboard != null)
            ComponentContainer.RemoveXKComponent(ref m_Gameboard);
    }

    void CreateAIs()
    {
        if (m_Gameboard == null)
            return;

        m_TeamIdToId.Clear();

        if (m_AIPerMatch < Gameboard.c_MinHomeCount)
        {
            // create a few AIs randomly
            for (int i = 0; i < m_AIPerMatch; ++i)
            {
                int aiId = m_AIPicker.DrawValue();
                m_TeamIdToId.Add(i, aiId); 
                CreateAI(aiId);
                m_PlayedAIs++;
            }
        }

        m_Gameboard.StartGame();
    }

    void CreateAI(int aiId)
    {
        AIBase ai = null;

        switch (aiId)
        {
            case 0:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 1:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 2:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 3:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 4:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 5:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 6:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 7:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 8:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 9:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 10:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 11:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 12:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 13:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 14:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 15:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 16:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 17:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 18:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 19:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 20:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 21:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 22:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 23:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 24:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 25:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 26:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;
            case 27:
                ai = m_Gameboard.CreateAI<YW.NicoJ.AITester>();
                break;

            default:
                XKLog.Log("Error", "MatchMaker.CreateAi(" + aiId + ") failed - aiId unknown");
                break;
        }

        // nothing to do?
        if (ai != null)
        {
            if (!m_IdToName.ContainsKey(aiId))
                m_IdToName.Add(aiId, ai.GetType().ToString());
        }
    }

    void CreateMatch()
    {
        DeleteGameboard();
        CreateGameboard();
        CreateAIs();
    }
    
    void OnGameOver(List<AIBase> leaderboard)
    {
        // compute scores from results
        for (int i = 0; i < leaderboard.Count; ++i)
        {
            int aiId = m_TeamIdToId[leaderboard[i].TeamId];
            IncrementScore(aiId, leaderboard.Count - i);
        }

        // restart a new match
        if (m_PlayedAIs < m_GamesPerAI * c_AICount)
        {
            m_AskRestart = 2.0f;
        }
        else
        {
            // display scores
            LogScores();

            Debug.LogError("Game is over!");
            Debug.Break();
        }
    }

    void IncrementScore(int aiId, int points)
    {
        if (!m_IdToScore.ContainsKey(aiId))
            m_IdToScore.Add(aiId, points);
        else
            m_IdToScore[aiId] += points;

        Debug.Log(aiId + " - " + points);
    }
    
    void LogScores()
    {
        for (int i = 0; i < c_AICount; ++i)
        {
            XKLog.Log("Info", string.Format("AI_{0:00}: {1:00} - {2}", i, m_IdToScore[i], m_IdToName[i]));
        }
    }

    #endregion


    #region Public Accessors

    /// <summary>
    /// 
    /// </summary>
    public Gameboard Gameboard
    {
        get { return m_Gameboard; }
    }

    #endregion
}