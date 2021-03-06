﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Pedestrian.Engine;
using Pedestrian.Engine.Collision;
using Pedestrian.Engine.Graphics;
using Pedestrian.Engine.Graphics.Shapes;
using Pedestrian.Engine.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pedestrian
{
    public class Enemy : IEntity
    {
        static Random randomTurn = new Random();

        AnimatedTexture frontSprite;
        AnimatedTexture leftSprite;
        AnimatedTexture rightSprite;
        AnimatedTexture currentSprite;
        Vector2 initialDirection;
        Vector2 movementDirection;
        Vector2 initialPosition;
        Vector2 previousPosition;
        int timeSinceTurn = 0;
        SoundEffect scream;


        // Min and max ms to wait before enemy turns
        public int[] IntervalRangeForTurn { get; set; } = new int[] { 350, 1800 };
        public Color Color { get; set; } = Color.White;
        public bool IsStatic { get; } = false;
        public float Speed { get; set; } = 1.5f;
        public Collider Collider { get; }
        public Vector2 Position { get; private set; }
        public Vector2 MovementDirection
        {
            get { return movementDirection; }
            private set
            {
                movementDirection = value;
                timeSinceTurn = 0;
                UpdateSpriteDirection();
            }
        }

        public Enemy(Vector2 enemyPosition)
        {
            Position = enemyPosition;
            initialPosition = enemyPosition;
            previousPosition = enemyPosition;
            
            initialDirection = DirectionMap.DIRECTION_VECTORS[Direction.Down];
            MovementDirection = initialDirection;

            scream = PedestrianGame.Instance.Content.Load<SoundEffect>("Audio/scream");

            frontSprite = new AnimatedTexture();
            var frontAsset = PedestrianGame.Instance.Content.Load<Texture2D>("gremlin16bit02");
            frontSprite.Load(frontAsset, 2, 60);

            leftSprite = new AnimatedTexture();
            var leftAsset = PedestrianGame.Instance.Content.Load<Texture2D>("gremlin16bit-left01");
            leftSprite.Load(leftAsset, 2, 60);

            rightSprite = new AnimatedTexture();
            var rightAsset = PedestrianGame.Instance.Content.Load<Texture2D>("gremlin16bit-right01");
            rightSprite.Load(rightAsset, 2, 60);

            currentSprite = frontSprite;

            // Collides only with default and not other collider types
            Collider = new BoxCollider(ColliderCategory.Default, ColliderCategory.Default | ColliderCategory.GameBounds)
            {
                Position = enemyPosition,
                Width = frontSprite.FrameWidth - 3,
                Height = frontSprite.FrameHeight - 1,
                OnCollisionEntered = OnCollisionEntered
            };
        }

        public void OnCollisionEntered(IEnumerable<IEntity> entities)
        {
            if (RoadBounds.Instance.Bounds.Contains(Position))
            {
                var hitByPlayer = false;

                foreach (Player player in entities.Where(e => e is Player))
                {
                    player.IncrementScore();
                    hitByPlayer = true;
                }

                if (hitByPlayer)
                {
                    Kill();
                    return;
                }
            }

            if (entities.Any(e => !(e is Player)))
            {
                // Create chance to turn in opposite direction when hitting wall
                if (entities.Any(e => e is PlayArea) && randomTurn.Next(0, 2) == 0)
                {
                    MovementDirection = -MovementDirection;
                }
                else
                {
                    MakeRandomTurn();
                }
                Position = previousPosition;
                Collider.Position = previousPosition;
                Collider.Clear();
            }
        }

        private void Kill()
        {
            scream.Play();
            PedestrianGame.Instance.Events.Emit(GameEvents.EnemyKilled, this);
            Reset();
        }

        public void MakeRandomTurn()
        {
            Vector3 resultantDirection = Vector3.Cross(new Vector3(MovementDirection, 0), Vector3.UnitZ);
            var randomDirection = randomTurn.Next(0, 2);
            if (randomDirection == 0)
            {
                resultantDirection = -resultantDirection;
            }
            MovementDirection = new Vector2(resultantDirection.X, resultantDirection.Y);
        }

        private void UpdateSpriteDirection()
        {
            if (MovementDirection == DirectionMap.DIRECTION_VECTORS[Direction.Left])
            {
                currentSprite = leftSprite;
            }
            else if (MovementDirection == DirectionMap.DIRECTION_VECTORS[Direction.Right])
            {
                currentSprite = rightSprite;
            }
            else
            {
                currentSprite = frontSprite;
            }
        }

        public void Update(GameTime time)
        {
            timeSinceTurn += time.ElapsedGameTime.Milliseconds;
            var shouldTurn = false;
            if (timeSinceTurn >= IntervalRangeForTurn[1])
            {
                shouldTurn = true;
            }
            else if (timeSinceTurn >= IntervalRangeForTurn[0])
            {
                // 5% chance to turn each update during allowed interval
                shouldTurn = randomTurn.Next(0, 20) == 0;
            }
            if (shouldTurn)
            {
                MakeRandomTurn();
            }

            previousPosition = Position;
            Position += MovementDirection * Speed;
            Collider.Position = Position;

            currentSprite.Update(time);
        }

        public void Reset()
        {
            Position = initialPosition;
            Collider.Position = initialPosition;
            MovementDirection = initialDirection;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            currentSprite.Draw(spriteBatch, Position);
        }

        public void DrawDebug(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Collider.Draw(spriteBatch);
            RectangleShape.Draw(spriteBatch, new Rectangle(Position.ToPoint(), new Point(1, 1)), Color.Red);
        }

        public void Unload() { }
    }
}
