using UnityEngine;

public class GameState : Singleton<GameState>
{
    private int currentScore = 0;
    private int lives = 3;

    void Awake()
    {
        if(Application.isMobilePlatform) Application.targetFrameRate = 90;
    }

    void Start()
    {
        EventHandler.Instance.hub.Publish<GameStart>(new GameStart());

        EventHandler.Instance.hub.Subscribe<Score>(s => 
        {
            currentScore += s.score;
            EventHandler.Instance.hub.Publish<ScoreTotal>(new ScoreTotal(currentScore));
        });

        EventHandler.Instance.hub.Subscribe<Fail>(f =>
        {
            lives -= 1;
            EventHandler.Instance.hub.Publish<Lives>(new Lives(lives));
            if(lives <= 0)
            {
                EventHandler.Instance.hub.Publish<GameOver>(new GameOver());
            }
            else
            {
                EventHandler.Instance.hub.Publish<GameStart>(new GameStart());
            }
        });

        EventHandler.Instance.hub.Subscribe<Restart>(r => 
        {
            lives = 3;
            EventHandler.Instance.hub.Publish<Lives>(new Lives(lives));

            currentScore = 0;
            EventHandler.Instance.hub.Publish<ScoreTotal>(new ScoreTotal(currentScore));

            EventHandler.Instance.hub.Publish<GameStart>(new GameStart());
        });

    }

    protected GameState() {}
}
