using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TRexRunner.Entities;

public class ObstacleManager : IGameEntity
{

    private const float MIN_SPAWN_DISANCE = 20;
    private const int MIN_OBSTACLE_DISTANCE = 10;
    private const int MAX_OBSTACLE_DISTANCE = 50;
    private const int OBSTACLE_DISTANCE_SPEED_TOLERANCE = 5;
    private const int LARGE_CACTUS_POS_Y = 80;
    private const int SMALL_CACTUS_POS_Y = 94;

    private readonly EntityManager _entityManager;
    private readonly TRex _trex;
    private readonly ScoreBoard _scoreBoard;
    private readonly Random _random;

    private double _lastSpawnScore = -1;
    private double _currentTargetDistance;
    private Texture2D _spriteSheet;

    private const int OBSTACLE_DRAW_ORDER = 12;
    private const float OBSTACE_DESPAWN_POS_X = -200;

    private const int FLYING_DINO_SPAWN_SCORE_MIN = 150;

    public bool CanSpawnObstacles => IsEnabled && _scoreBoard.Score >= MIN_SPAWN_DISANCE;

    public bool IsEnabled { get; set; }

    public int DrawOrder => 0;

    public ObstacleManager(EntityManager entityManager, TRex trex, ScoreBoard scoreBoard, Texture2D spriteSheet)
    {
        _entityManager = entityManager;
        _trex = trex;
        _scoreBoard = scoreBoard;
        _random = new Random();
        _spriteSheet = spriteSheet;
    }
    
    public void Update(GameTime time)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (CanSpawnObstacles &&
            (_lastSpawnScore <= 0 || (_scoreBoard.Score - _lastSpawnScore >= _currentTargetDistance)))
        {
            _currentTargetDistance = _random.NextDouble() * (MAX_OBSTACLE_DISTANCE - MIN_OBSTACLE_DISTANCE) + MIN_OBSTACLE_DISTANCE;

            _currentTargetDistance += (_trex.Speed - TRex.START_SPEED ) / (TRex.MAX_SPEED - TRex.START_SPEED) * OBSTACLE_DISTANCE_SPEED_TOLERANCE;
            
            _lastSpawnScore = _scoreBoard.Score;

            SpawnRandomObstacle();
        }

        foreach (Obstacle obstacle in _entityManager.GetEntitiesOfType<Obstacle>())
        {
            if (obstacle.Position.X < OBSTACE_DESPAWN_POS_X)
            {
                _entityManager.RemoveEntity(obstacle);
            }
        }
    }

    private void SpawnRandomObstacle()
    {
        Obstacle obstacle = null;

        int cactusGroupSpawnRate = 75;
        int flyingDinoSpawnRate = _scoreBoard.Score >= FLYING_DINO_SPAWN_SCORE_MIN ? 75 : 0;

        int rng = _random.Next(0, cactusGroupSpawnRate + flyingDinoSpawnRate + 1);

        if (rng <= cactusGroupSpawnRate)
        {
            CactusGroup.GroupSize randomGroupsSize = (CactusGroup.GroupSize) _random.Next((int)CactusGroup.GroupSize.Small, (int)CactusGroup.GroupSize.Large + 1);
            bool isLarge = _random.NextDouble() > 0.5;
            float posY = isLarge ? LARGE_CACTUS_POS_Y : SMALL_CACTUS_POS_Y;
            obstacle = new CactusGroup(_spriteSheet, isLarge, randomGroupsSize, _trex, new Vector2(TRexRunnerGame.WINDOW_WIDTH, posY));
        }
        else
        {
            int verticalPosIndex = _random.Next(0, FlyingDino.FlyingPositions.Length);
            float posY = FlyingDino.FlyingPositions[verticalPosIndex];
            obstacle = new FlyingDino(_spriteSheet, _trex, new Vector2(TRexRunnerGame.WINDOW_WIDTH, posY));
        }

        obstacle.DrawOrder = OBSTACLE_DRAW_ORDER;
        
        _entityManager.AddEntity(obstacle);
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
        
    }

    public void Reset()
    {
        foreach (Obstacle obstacle in _entityManager.GetEntitiesOfType<Obstacle>())
        {
            _entityManager.RemoveEntity(obstacle);
        }

        _currentTargetDistance = 0;
        _lastSpawnScore = -1;
    }
}