using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRexRunner.Graphics;

namespace TRexRunner.Entities;

public class GroundManager : IGameEntity
{
    private const int SPRITE_WIDTH = 600;
    private const int SPRITE_HEIGHT = 14;

    private const int SPRITE_POS_X = 2;
    private const int SPRITE_POS_Y = 54;

    private Sprite _regularSprite;
    private Sprite _bumpySprite;

    public int DrawOrder { get; set; }

    private const float GROUND_TILE_POS_Y = 119;

    private Texture2D _spriteSheet;

    private readonly List<GroundTile> _groundTiles;
    private readonly EntityManager _entityManager;

    private TRex _trex;

    private Random _random;

    public GroundManager(Texture2D spriteSheet, EntityManager entityManager, TRex trex)
    {
        _spriteSheet = spriteSheet;
        _groundTiles = new List<GroundTile>();
        _entityManager = entityManager;

        _regularSprite = new Sprite(spriteSheet, SPRITE_POS_X, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
        _bumpySprite = new Sprite(spriteSheet, SPRITE_POS_X + SPRITE_WIDTH, SPRITE_POS_Y, SPRITE_WIDTH, SPRITE_HEIGHT);
        _trex = trex;
        _random = new Random();
    }

    public void Update(GameTime time)
    {
        if (_groundTiles.Any())
        {
            float maxPosX = _groundTiles.Max(g => g.PositionX);

            if (maxPosX < 0)
            {
                SpawnTile(maxPosX);
            }
        }

        List<GroundTile> tilesToRemove = new List<GroundTile>();

        foreach (GroundTile gt in _groundTiles)
        {
            gt.PositionX -= _trex.Speed * (float)time.ElapsedGameTime.TotalSeconds;

            if (gt.PositionX < -SPRITE_WIDTH)
            {
                _entityManager.RemoveEntity(gt);
                tilesToRemove.Add(gt);
            }
        }

        foreach (GroundTile gt in tilesToRemove)
        {
            _groundTiles.Remove(gt);
        }

        tilesToRemove.Clear();
    }

    public void Draw(SpriteBatch batch, GameTime time)
    {
    }

    private GroundTile CreateRegularTile(float positionX)
    {
        return new GroundTile(positionX, GROUND_TILE_POS_Y, _regularSprite);
    }

    private GroundTile CreateBumpyTile(float positionX)
    {
        return new GroundTile(positionX, GROUND_TILE_POS_Y, _bumpySprite);
    }

    public void Initialize()
    {
        _groundTiles.Clear();

        foreach (GroundTile gt in _entityManager.GetEntitiesOfType<GroundTile>())
        {
            _entityManager.RemoveEntity(gt);
        }

        GroundTile groundTile = CreateRegularTile(0);
        _groundTiles.Add(groundTile);

        _entityManager.AddEntity(groundTile);
    }

    private void SpawnTile(float maxPosX)
    {
        double randomNumber = _random.NextDouble();

        GroundTile groundTile;
        float posX = maxPosX + SPRITE_WIDTH;

        if (randomNumber > 0.5)
            groundTile = CreateBumpyTile(posX);
        else
            groundTile = CreateRegularTile(posX);
        
        _entityManager.AddEntity(groundTile);
        _groundTiles.Add(groundTile);
    }
}