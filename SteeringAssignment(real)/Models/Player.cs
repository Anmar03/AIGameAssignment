﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.GameLighting;
using SteeringAssignment_real.Mangers;
using System;
using System.Collections.Generic;

namespace SteeringAssignment_real.Models
{
    public enum PlayerState
    {
        Walk,
        FistAttack,
        SwordAttack,
        Dead
    }
    public class Player : Sprite
    {
        private PlayerState currentState = PlayerState.Walk;
        private Vector2 _minPos, _maxPos;
        private Vector2 direction;
        private readonly Animation frame;
        private readonly AnimationManager _anims = new();
        private readonly AnimationManager _fistAttackAnim = new();
        private readonly AnimationManager _swordAttackAnim = new();
        private readonly AnimationManager _deathAnim = new();
        private readonly GameManager _gameManager;
        private readonly Texture2D fistAttackTexture;
        private readonly Texture2D swordAttackTexture;
        private readonly Texture2D deadTexture;
        private readonly Random random = new();
        public Vector2 PlayerDirection {  get; private set; }

        private const float punchPushForce = 900;
        private const float swordPushForce = 600;
        private const float fistAttackDamage = 2.0f;
        private const float swordAttackDamage = 5.0f;

        private object lastKey;
        private float attackDelayTimer = 0;
        private const float attackDelayDuration = 0.5f;

        private List<GlowStick> _glowSticks;
        private int glowStickAmmo = 3;
        private bool glowstickCooldown = false;
        private float glowstickCooldownDuration = 1.0f;
        private float glowstickCooldownTimer = 0.0f;

        public Player(Texture2D texture, Vector2 position, GameManager _gameMannager) : base(texture, position)
        {
            Health = 50f;
            speed = 300f;

            this._gameManager = _gameMannager;
            _glowSticks = new();
            GenerateGlowSticks(glowStickAmmo);

            frame = new Animation(texture, 10, 8, 0.1f, 5);
            _anims.AddAnimation(new Vector2(0, 1), frame); // S
            _anims.AddAnimation(new Vector2(-1, 0), new Animation(texture, 10, 8, 0.1f, 7)); // A
            _anims.AddAnimation(new Vector2(1, 0), new Animation(texture, 10, 8, 0.1f, 3)); // D
            _anims.AddAnimation(new Vector2(0, -1), new Animation(texture, 10, 8, 0.1f, 1)); // W
            _anims.AddAnimation(new Vector2(-1, 1), new Animation(texture, 10, 8, 0.1f, 6)); // SA
            _anims.AddAnimation(new Vector2(-1, -1), new Animation(texture, 10, 8, 0.1f, 8)); // WA
            _anims.AddAnimation(new Vector2(1, 1), new Animation(texture, 10, 8, 0.1f, 4)); // SD
            _anims.AddAnimation(new Vector2(1, -1), new Animation(texture, 10, 8, 0.1f, 2)); // WD

            fistAttackTexture = Globals.Content.Load<Texture2D>("fist_attack");
            _fistAttackAnim.AddAnimation(new Vector2(0, 1), new Animation(fistAttackTexture, 9, 8, 0.1f, 5)); // S
            _fistAttackAnim.AddAnimation(new Vector2(-1, 0), new Animation(fistAttackTexture, 9, 8, 0.1f, 7)); // A
            _fistAttackAnim.AddAnimation(new Vector2(1, 0), new Animation(fistAttackTexture, 9, 8, 0.1f, 3)); // D
            _fistAttackAnim.AddAnimation(new Vector2(0, -1), new Animation(fistAttackTexture, 9, 8, 0.1f, 1)); // W
            _fistAttackAnim.AddAnimation(new Vector2(-1, 1), new Animation(fistAttackTexture, 9, 8, 0.1f, 6)); // SA
            _fistAttackAnim.AddAnimation(new Vector2(-1, -1), new Animation(fistAttackTexture, 9, 8, 0.1f, 8)); // WA
            _fistAttackAnim.AddAnimation(new Vector2(1, 1), new Animation(fistAttackTexture, 9, 8, 0.1f, 4)); // SD
            _fistAttackAnim.AddAnimation(new Vector2(1, -1), new Animation(fistAttackTexture, 9, 8, 0.1f, 2)); // WD

            swordAttackTexture = Globals.Content.Load<Texture2D>("shword_attack");
            _swordAttackAnim.AddAnimation(new Vector2(0, 1), new Animation(swordAttackTexture, 9, 8, 0.1f, 5)); // S
            _swordAttackAnim.AddAnimation(new Vector2(-1, 0), new Animation(swordAttackTexture, 9, 8, 0.1f, 7)); // A
            _swordAttackAnim.AddAnimation(new Vector2(1, 0), new Animation(swordAttackTexture, 9, 8, 0.1f, 3)); // D
            _swordAttackAnim.AddAnimation(new Vector2(0, -1), new Animation(swordAttackTexture, 9, 8, 0.1f, 1)); // W
            _swordAttackAnim.AddAnimation(new Vector2(-1, 1), new Animation(swordAttackTexture, 9, 8, 0.1f, 6)); // SA
            _swordAttackAnim.AddAnimation(new Vector2(-1, -1), new Animation(swordAttackTexture, 9, 8, 0.1f, 8)); // WA
            _swordAttackAnim.AddAnimation(new Vector2(1, 1), new Animation(swordAttackTexture, 9, 8, 0.1f, 4)); // SD
            _swordAttackAnim.AddAnimation(new Vector2(1, -1), new Animation(swordAttackTexture, 9, 8, 0.1f, 2)); // WD

            deadTexture = Globals.Content.Load<Texture2D>("die2");
            _deathAnim.AddAnimation(new Vector2(0, 1), new Animation(deadTexture, 13, 8, 0.1f, 5)); // S
            _deathAnim.AddAnimation(new Vector2(-1, 0), new Animation(deadTexture, 13, 8, 0.1f, 7)); // A
            _deathAnim.AddAnimation(new Vector2(1, 0), new Animation(deadTexture, 13, 8, 0.1f, 3)); // D
            _deathAnim.AddAnimation(new Vector2(0, -1), new Animation(deadTexture, 13, 8, 0.1f, 1)); // W
            _deathAnim.AddAnimation(new Vector2(-1, 1), new Animation(deadTexture, 13, 8, 0.1f, 6)); // SA
            _deathAnim.AddAnimation(new Vector2(-1, -1), new Animation(deadTexture, 13, 8, 0.1f, 8)); // WA
            _deathAnim.AddAnimation(new Vector2(1, 1), new Animation(deadTexture, 13, 8, 0.1f, 4)); // SD
            _deathAnim.AddAnimation(new Vector2(1, -1), new Animation(deadTexture, 13, 8, 0.1f, 2)); // WD
        }

        public void SetBounds(Point mapSize, Point tileSize)
        {
            width = frame.frameWidth;
            height = frame.frameHeight;
            Origin = new Vector2(width / 2, height / 2);

            _minPos = new(-tileSize.X / 2 + width / 3, -tileSize.Y / 2 + height / 2);
            _maxPos = new(mapSize.X - tileSize.X / 2 - width / 3, mapSize.Y - tileSize.X / 2 - height / 2);
        }

        public void Update(CollisionManager collisionManager)
        {
            Vector2 pushDirection = Vector2.Zero;
            object animationKey = AnimationManager.GetAnimationKey(InputManager.Direction);
            float distance = 0;
            direction = Vector2.Zero;
            float enemyHealth = 0;

            if (Health <= 0)
            {
                currentState = PlayerState.Dead;
            }
            else if(currentState == PlayerState.FistAttack || currentState == PlayerState.SwordAttack)
            {
                var result = collisionManager.ClosestEntityInfo(Position);
                distance = result.distance;
                direction = result.direction;
                enemyHealth = result.health;
            }

            if (glowstickCooldown)
            {
                glowstickCooldownTimer -= Globals.Time;
                if (glowstickCooldownTimer <= 0)
                {
                    glowstickCooldown = false;
                }
            }

            if (glowStickAmmo >= 0 && !glowstickCooldown && !isDead())
            {
                if (InputManager.G_keyPressed && _glowSticks[glowStickAmmo - 1].throwable)
                {
                    _glowSticks[glowStickAmmo - 1].Throw(InputManager.LastDirection);
                    glowStickAmmo--;
                    glowstickCooldown = true;
                    glowstickCooldownTimer = glowstickCooldownDuration;
                }

                if (InputManager.E_keyPressed && _glowSticks[glowStickAmmo - 1].throwable)
                {
                    _glowSticks[glowStickAmmo - 1].Active = true;
                }
            }

            foreach (var glowStick in _glowSticks)
            {
                if (glowStick.throwable)
                {
                    glowStick.Position = Position + Origin * 1.5f;
                }
                glowStick.Update();
            }

            switch (currentState)
            {
                case PlayerState.Walk:
                    if (attackDelayTimer > 0)
                    {
                        attackDelayTimer -= Globals.Time;
                    }

                    if (InputManager.Moving)
                    {
                        PlayerDirection = Vector2.Normalize(InputManager.Direction);
                        Position += PlayerDirection * speed * Globals.Time;
                        lastKey = AnimationManager.GetAnimationKey(InputManager.LastDirection);
                    }
                    else if (InputManager.SpacebarPressed && attackDelayTimer <= 0)
                    {
                        currentState = PlayerState.FistAttack;
                    }
                    else if (InputManager.F_keyPressed && attackDelayTimer <= 0)
                    {
                        currentState = PlayerState.SwordAttack;
                    }

                    _anims.Update(animationKey);
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);
                    break;

                case PlayerState.FistAttack:
                    _fistAttackAnim.Update(lastKey);
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);

                    // if in the middle of animation and still close, push the player line 128 TO-DO: ADD CHECK IF PLAYER IS FACING ENEMY
                    if (_fistAttackAnim.CurrentFrame == 5 && distance < width + width/2)
                    {
                        pushDirection = direction * punchPushForce;
                    }
                    
                    if (_fistAttackAnim.CurrentFrame == _fistAttackAnim.TotalFrames - 1)
                    {
                        currentState = PlayerState.Walk;
                        _fistAttackAnim.Reset();

                        attackDelayTimer = attackDelayDuration;
                    }
                    break;

                case PlayerState.SwordAttack:
                    _swordAttackAnim.Update(lastKey);
                    Position = Vector2.Clamp(Position, _minPos, _maxPos);

                    if (_swordAttackAnim.CurrentFrame == 4 && distance < width * 1.5f)
                    {
                        pushDirection = direction * swordPushForce;
                    }

                    if (_swordAttackAnim.CurrentFrame == _swordAttackAnim.TotalFrames - 1)
                    {
                        currentState = PlayerState.Walk;
                        _swordAttackAnim.Reset();

                        attackDelayTimer = attackDelayDuration;
                    }
                    break;

                case PlayerState.Dead:
                    EntityCollision = false;

                    if (_deathAnim.CurrentFrame == _deathAnim.TotalFrames - 1)
                    {
                        _deathAnim.UpdateDeath(Vector2.Zero);
                    }
                    else
                    {
                        _deathAnim.Update(lastKey);
                    }
                    break;
            }

            if (pushDirection != Vector2.Zero && enemyHealth > 0)
            {
                enemyHealth -= GetAttackDamage();
                collisionManager.SetClosestEntityHealth(Position, enemyHealth);
                collisionManager.SetClosestEntityPosition(Position, pushDirection);
            }
        }

        private float GetAttackDamage()
        {
            switch (currentState)
            {
                case PlayerState.FistAttack:
                    return fistAttackDamage;
                    
                case PlayerState.SwordAttack:
                    return swordAttackDamage;
            }
            return -1;
        }

        public override void Draw()
        {
            switch (currentState)
            {
                case PlayerState.Walk:
                    _anims.Draw(Position - Origin, Color);
                    break;

                case PlayerState.FistAttack:
                    _fistAttackAnim.Draw(Position - Origin, Color);
                    break;

                case PlayerState.SwordAttack:
                    _swordAttackAnim.Draw(Position - Origin, Color);
                    break;

                case PlayerState.Dead:
                    _deathAnim.Draw(Position - Origin, Color);
                    break;
            }

            foreach (var glowStick in _glowSticks)
            {
                glowStick.Draw();
            }
        }

        private void GenerateGlowSticks(int count)
        {
            Color red = new(200, 100, 100);
            Color green = new(100, 200, 100);
            Color blue = new(100, 100, 200);
            GlowStick glowStick;
            int randomPicker;

            for (int i = 0; i < count; i++)
            {
                randomPicker = random.Next(0, 3);

                if (randomPicker == 0)
                {
                    glowStick = new((Position), red);
                }
                else if (randomPicker == 1)
                {
                    glowStick = new((Position), green);
                }
                else
                {
                    glowStick = new((Position), blue);
                }
                glowStick.SetBounds(_gameManager.GetMapSize(), _gameManager.GetTileSize());
                _glowSticks.Add(glowStick);
                _gameManager.AddLight(glowStick);
            }
        }

        public int GetGlowStickAmmo()
        {
            return glowStickAmmo;
        }

        public bool isDead()
        {
            return currentState == PlayerState.Dead;
        }
    }
}
