﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModernGod.Characters.Pathfinding;
using ModernGod.Debugging;
using ModernGod.Pathfinding;
using ModernGod.Utils;
using ModernGod.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Characters
{
    public class Character
    {
        public static Texture2D CharacterTexture;
        public static int Completed;

        public CharacterManager Manager { get; private set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if(value == null)
                {
                    _name = "No Name";
                }
                else
                {
                    _name = value;
                }
            }
        }
        private string _name;

        public Vector2Int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value == _position)
                    return;

                if (value.AdjacentTo(lastPosition))
                {
                    _position = value;
                    moveProgress = 0f;
                }
                else
                {
                    Debug.LogError("Cannot set character position to " + value + ", it is not adjacent to the last position. Use Teleport instead.");
                }
            }
        }
        private Vector2Int _position;

        private Vector2Int lastPosition;
        public bool AtPosition
        {
            get
            {
                return lastPosition == Position;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                var pos = Vector2Int.Lerp(lastPosition * Terrain.TILE_SIZE, Position * Terrain.TILE_SIZE, moveProgress, IntLerpMode.DEFAULT);
                return new Rectangle(pos.X, pos.Y, 16, 16);
            }
        }

        public Color Colour = Color.BlueViolet;
        public bool IsDestroyed { get; private set; }

        private float moveProgress;
        private const float LONG_SIDE = 1.414213562373095f;

        private List<PNode> path;

        public Character(string name, Vector2Int position, CharacterManager manager)
        {
            this.Name = name;
            Teleport(position);

            Manager = manager ?? throw new ArgumentNullException("manager", "The manager object cannot be null!");
            Manager.Register(this);

            var map = manager.Map;
            map.Pathing.MakeRequest(new PathingRequest(position.X, position.Y, map.Width - 1, map.Height - 1, UponPathCompleted));
        }

        private void UponPathCompleted(PathfindingResult result, List<PNode> path)
        {
            if(result != PathfindingResult.SUCCESSFUL)
            {
                Debug.LogError(result);
            }
            else
            {
                this.path = path;
            }
            Completed++;
        }

        public void Teleport(Vector2Int position)
        {
            moveProgress = 1f;
            lastPosition = position;
            _position = position;
        }

        public void Destroy()
        {
            if (IsDestroyed)
                return;

            IsDestroyed = true;
        }

        public void Update()
        {
            if(CharacterTexture == null)
            {
                CharacterTexture = Main.ContentMangager.Load<Texture2D>("Textures/Character");
            }

            float tilesPerSecond = 5f;

            if (moveProgress != 1f)
            {
                // Work out the distance in terms of tiles, assuming that the target position is adjacent to the last position.
                float dst = 1f;
                if (Position.X != lastPosition.X && Position.Y != lastPosition.Y)
                    dst = LONG_SIDE;

                float time = dst / tilesPerSecond;
                moveProgress += Time.DeltaTime / time;
                moveProgress = MathUtils.Clamp01(moveProgress);
            }
            else
            {
                lastPosition = Position;

                // Move to next point.
                if(path != null && path.Count > 0)
                {
                    Position = path[0];
                    path.RemoveAt(0);
                }                
            }
        }

        public void Draw(SpriteBatch spr)
        {
            Rectangle source = CharacterTexture.Bounds;
            spr.Draw(CharacterTexture, Bounds, source, Colour, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }

        public override string ToString()
        {
            return Name + " @ " + Position;
        }
    }
}