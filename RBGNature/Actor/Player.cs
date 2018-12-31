using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RBGNature.Graphics.Animate;
using RBGNature.Physics;
using System;
using System.Collections.Generic;

namespace RBGNature.Actor
{
    class Player : IAct, ICollide
    {
        enum FourDirectionAnimation
        {
            Front,
            Back,
            Left,
            Right
        }

        static readonly Rectangle RectBulletDefault = new Rectangle(0, 0, 6, 6);
        static readonly Rectangle RectBulletCannon = new Rectangle(6, 0, 9, 9);

        static AnimationDictionary<FourDirectionAnimation> animDict_Run;
        static AnimationDictionary<FourDirectionAnimation> animDict_Idle;
        static AnimationDictionary<FourDirectionAnimation> animDict_RunStop;


        static CollisionIdentity bulletIdentity = new CollisionIdentity()
        {
            Damage = 1
        };

        static CollisionIdentity cannonballIdentity = new CollisionIdentity()
        {
            Damage = 5
        };

        Camera camera;
        Texture2D textureMan;
        Texture2D textureBullet;
        Texture2D textureCannonball;
        Texture2D textureHPBar;
        Texture2D textureLight;
        SoundEffect soundEffectGunshot;
        SoundEffect soundEffectRhythmClick;

        public int CurrentHealth { get; private set; }
        const int MaxHealth = 15;
        bool tookDamage = false;
        float timeBetweenDamage = 0;
        Animator animator;

        List<Circle> bullets;
        List<Circle> bulletsToRemove;
        List<Circle> cannonballs;
        List<Circle> cannonballsToRemove;
        public Circle collision;
        private CollisionResult collisionResult;

        Texture2D textureCircle10;
        Texture2D textureCircle200;

        GunMode mode;
        private bool canChangeMode;

        private float timeBetweenShots;
        private bool justShot;

        private float shotRhythm;
        private bool countingRhythm;
        private bool rhythmReady;

        public enum GunMode
        {
            Default,
            Shotgun,
            Sniper,
            Cannon,
            Turret,
            Laser
        }

        static Player()
        {
            animDict_Run = new AnimationDictionary<FourDirectionAnimation>();
            animDict_Run[FourDirectionAnimation.Front] = new Animation("Sprites/mc/mc_run_front", 100, 8, 14, 38);
            animDict_Run[FourDirectionAnimation.Back] = new Animation("Sprites/mc/mc_run_back", 100, 8, 14, 38);
            animDict_Run[FourDirectionAnimation.Left] = new Animation("Sprites/mc/mc_run_left", 100, 8, 20, 36);
            animDict_Run[FourDirectionAnimation.Right] = new Animation("Sprites/mc/mc_run_right", 100, 8, 20, 36);

            animDict_Idle = new AnimationDictionary<FourDirectionAnimation>();
            animDict_Idle[FourDirectionAnimation.Front] = new Animation("Sprites/mc/mc_idle_front", 100, 10, 14, 38);
            animDict_Idle[FourDirectionAnimation.Back] = new Animation("Sprites/mc/mc_idle_back", 100, 10, 14, 38);
            animDict_Idle[FourDirectionAnimation.Left] = new Animation("Sprites/mc/mc_idle_left", 100, 10, 12, 32);
            animDict_Idle[FourDirectionAnimation.Right] = new Animation("Sprites/mc/mc_idle_right", 100, 10, 12, 32);

            animDict_RunStop = new AnimationDictionary<FourDirectionAnimation>();
            animDict_RunStop[FourDirectionAnimation.Front] = new Animation("Sprites/mc/mc_stop_run_front", 100, 2, 14, 38);
            animDict_RunStop[FourDirectionAnimation.Back] = new Animation("Sprites/mc/mc_stop_run_back", 100, 2, 14, 38);
            animDict_RunStop[FourDirectionAnimation.Left] = new Animation("Sprites/mc/mc_stop_run_left", 100, 2, 12, 31);
            animDict_RunStop[FourDirectionAnimation.Right] = new Animation("Sprites/mc/mc_stop_run_right", 100, 2, 12, 31);
        }

        public Player(Camera camera)
        {
            this.camera = camera;
            bullets = new List<Circle>();
            bulletsToRemove = new List<Circle>();
            cannonballs = new List<Circle>();
            cannonballsToRemove = new List<Circle>();
            collision = new Circle()
            {
                Radius = 10,
                Position = new Vector2(camera.Position.X, camera.Position.Y),
                Mass = 1
            };
            collisionResult = CollisionResult.None;
            CurrentHealth = MaxHealth;
            animator = new Animator(animDict_Idle[FourDirectionAnimation.Front]);
        }

        public void LoadContent(ContentManager contentManager)
        {
            animDict_Run.Load(contentManager);
            animDict_Idle.Load(contentManager);
            animDict_RunStop.Load(contentManager);

            textureMan = contentManager.Load<Texture2D>("Sprites/mc/front");
            textureBullet = contentManager.Load<Texture2D>("Sprites/bullet/bullet");
            textureCannonball = contentManager.Load<Texture2D>("Sprites/bullet/cannonball");
            textureCircle10 = contentManager.Load<Texture2D>("Sprites/debug/circle10");
            textureCircle200 = contentManager.Load<Texture2D>("Sprites/debug/circle200");
            textureHPBar = contentManager.Load<Texture2D>("UI/HPBar");

            textureLight = contentManager.Load<Texture2D>("Sprites/light/53");

            soundEffectGunshot = contentManager.Load<SoundEffect>("Sound/effect/gunshot");
            soundEffectRhythmClick = contentManager.Load<SoundEffect>("Sound/effect/metronome");
        }

        /// <summary>
        /// Tracks what input direction was last pressed to play correct idle animation
        /// </summary>
        FourDirectionAnimation lastInputDirection = FourDirectionAnimation.Front;

        /// <summary>
        /// Tracks if the game has played the run stop animation yet
        /// </summary>
        Boolean playedRunStopAnimation = false;
        float runStopAnimationFrameTime = 200;

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //System.Console.WriteLine("Elapsed Time = " + elapsedTime);
            var kstate = Keyboard.GetState();
            if (collisionResult)
            {
                // If we had a collision, start over with the collision result
                collision.Position = collisionResult.PositionA;
                collision.Velocity = collisionResult.VelocityA;
                collisionResult = CollisionResult.None;
            }
            else
            {
                collision.Position += collision.Velocity * elapsedTime;
            }
            
            float speed = .2f;
            Vector2 inputVelocity = Vector2.Zero;

            /* Monet: Added to play the idle animation */
            if (playedRunStopAnimation)
                animator.Set(animDict_Idle[lastInputDirection]);
            else
            {
                animator.Set(animDict_RunStop[lastInputDirection]);
                runStopAnimationFrameTime -= elapsedTime;
                if (runStopAnimationFrameTime <= 0)
                {
                    playedRunStopAnimation = true;
                    runStopAnimationFrameTime = 200;
                }
            }

            if (kstate.IsKeyDown(Keys.W))
            {
                inputVelocity.Y -= 1;
                animator.Set(animDict_Run[FourDirectionAnimation.Back]);
                lastInputDirection = FourDirectionAnimation.Back;
            }
            if (kstate.IsKeyDown(Keys.A))
            {
                inputVelocity.X -= 1;
                animator.Set(animDict_Run[FourDirectionAnimation.Left]);
                lastInputDirection = FourDirectionAnimation.Left;
            }
            if (kstate.IsKeyDown(Keys.S))
            {
                inputVelocity.Y += 1;
                animator.Set(animDict_Run[FourDirectionAnimation.Front]);
                lastInputDirection = FourDirectionAnimation.Front;
            }
            if (kstate.IsKeyDown(Keys.D))
            {
                inputVelocity.X += 1;
                animator.Set(animDict_Run[FourDirectionAnimation.Right]);
                lastInputDirection = FourDirectionAnimation.Right;
            }

            if (inputVelocity == Vector2.Zero)
            {
                float previousSpeed = collision.Velocity.Length();
                if (previousSpeed > .01f)
                {
                    collision.Velocity *= .3f;
                }
                else
                {
                    collision.Velocity = Vector2.Zero;
                }
            }
            else
            {
                playedRunStopAnimation = false;
                if (inputVelocity.X != 0 && inputVelocity.Y != 0) { inputVelocity.Normalize(); } //TODO: Performance?

                Vector2 newDirection;
                float previousSpeed = collision.Velocity.Length();
                if (previousSpeed > .01f)
                {
                    Vector2 compromise = collision.Velocity / previousSpeed + inputVelocity;
                    newDirection = compromise == Vector2.Zero ? compromise : Vector2.Normalize(compromise);
                }
                else
                {
                    newDirection = inputVelocity;
                    previousSpeed = 0; // Round this down to improve accuracy
                }
                collision.Velocity = newDirection * (speed * .7f + previousSpeed * .3f);
            }

            camera.MoveTo(collision.Position);

            if (kstate.IsKeyDown(Keys.F))
            {
                if (canChangeMode)
                {
                    canChangeMode = false;
                    if (mode == GunMode.Default) mode = GunMode.Cannon;
                    else mode = GunMode.Default;
                }
            }
            else { canChangeMode = true; }

            //Remove any bullets that collided in the last update
            bulletsToRemove.ForEach(b => bullets.Remove(b));
            cannonballsToRemove.ForEach(b => cannonballs.Remove(b));

            var mstate = Mouse.GetState();
            timeBetweenShots += elapsedTime;
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                if (timeBetweenShots > 100 && !justShot)
                {
                    if (mode == GunMode.Default)
                    {
                        bullets.Add(new Circle()
                        {
                            Position = camera.Position,
                            Velocity = Vector2.Normalize(mstate.Position.ToVector2() - camera.FocalPoint) * .5f,
                            Mass = 1,
                            Radius = 3
                        });
                    }
                    else if (mode == GunMode.Cannon)
                    {
                        cannonballs.Add(new Circle()
                        {
                            Position = camera.Position,
                            Velocity = Vector2.Normalize(mstate.Position.ToVector2() - camera.FocalPoint) * .25f,
                            Mass = 1,
                            Radius = 6
                        });
                    }

                    soundEffectGunshot.CreateInstance().Play();

                    if (countingRhythm)
                    {
                        if (shotRhythm == 0)
                        {
                            shotRhythm = timeBetweenShots;
                        }
                        else if (Math.Abs(timeBetweenShots - shotRhythm) < 60)
                        {
                            rhythmReady = true;
                        }
                        else
                        {
                            rhythmReady = false;
                            countingRhythm = false;
                        }
                    }
                    else
                    {
                        countingRhythm = true;
                        shotRhythm = 0;
                    }

                    timeBetweenShots = 0;
                    justShot = true;
                }
            }
            else
            {
                justShot = false;
            }

            if (rhythmReady && timeBetweenShots >= .5 * shotRhythm && rhythmReady)
            {
                soundEffectRhythmClick.CreateInstance().Play();
                rhythmReady = false;
            }

            foreach (Circle bullet in bullets)
            {
                bullet.Position += bullet.Velocity * elapsedTime;
            }
            foreach (Circle cannonball in cannonballs)
            {
                cannonball.Position += cannonball.Velocity * elapsedTime;
            }

            animator.Update(gameTime);
            if (tookDamage)
            {
                timeBetweenDamage += elapsedTime;
            }
            if (timeBetweenDamage > 100)
            {
                timeBetweenDamage = 0;
                tookDamage = false;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = Color.White;
            if (tookDamage)
            {
                tint = Color.Red;
            }
            spriteBatch.Draw(animator.Texture, camera.Position - new Vector2(10,30), animator.NextFrame(), tint, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, this.LayerDepth(collision.Position.Y));

            //spriteBatch.Draw(textureCircle10, camera.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            //spriteBatch.Draw(textureCircle200, new Vector2(100,100), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            spriteBatch.Draw(textureHPBar, collision.Position - new Vector2(0, 50), getHealthSpriteRect(), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);

            foreach (Circle bullet in bullets)
            {
                spriteBatch.Draw(textureBullet, bullet.Position, RectBulletDefault, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
            foreach (Circle cannonball in cannonballs)
            {
                spriteBatch.Draw(textureCannonball, cannonball.Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
        }

        private void TakeDamage(CollisionIdentity identity)
        {
            if (identity == null)
                return;
            if (identity.Damage > 0)
            {
                CurrentHealth -= identity.Damage;
                tookDamage = true;
                timeBetweenDamage = 0;
            }
        }

        private Rectangle getHealthSpriteRect()
        {
            int index = 8 - (int)Math.Ceiling(((float)CurrentHealth / MaxHealth * 8));
            return new Rectangle(0, index * 8, 8, 8);
        }

        public PhysicsObject GetCollisionObject(PhysicsGroupType groupType)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                return collision;
            }
            return null;
        }

        public void Collide(float s, PhysicsGroupType groupType, ICollide other)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    Circle bullet = bullets[i];
                    CollisionResult bulletCollision = other.Collide(s, groupType, bullet, bulletIdentity);
                    if (bulletCollision)
                    {
                        bulletsToRemove.Add(bullet);
                        bullet.Position = bulletCollision.PositionA;
                        //bullet.Velocity = bulletCollision.VelocityA;
                    }

                }
                for (int i = cannonballs.Count - 1; i >= 0; i--)
                {
                    Circle cannonball = cannonballs[i];
                    CollisionResult cannonballCollision = other.Collide(s, groupType, cannonball, cannonballIdentity);
                    if (cannonballCollision)
                    {
                        cannonballsToRemove.Add(cannonball);
                        cannonball.Position = cannonballCollision.PositionA;
                        //cannonball.Velocity = cannonballCollision.VelocityA;
                    }

                }
                CollisionResult playerCollision = other.Collide(s, groupType, this.collision, null);
                if (playerCollision)
                {
                    if (collisionResult)
                    {
                        //Not sure if necessary, but if we have multiple collisions in a single frame, stop all movement.
                        collisionResult.PositionA = collision.Position;
                        collisionResult.VelocityA = Vector2.Zero;
                    }
                    else
                    {
                        collisionResult = playerCollision;
                    }
                    TakeDamage(playerCollision.Identity);
                }
            }
        }

        public CollisionResult Collide(float s, PhysicsGroupType groupType, PhysicsObject physicsObject, CollisionIdentity identity)
        {
            CollisionResult response = CollisionResult.None;
            if (groupType == PhysicsGroupType.Physical)
            {
                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    Circle bullet = bullets[i];
                    CollisionResult bulletCollision = physicsObject.Collide(s, bullet);
                    if (bulletCollision)
                    {
                        bulletsToRemove.Add(bullet);
                        bullet.Position = bulletCollision.PositionB;
                        //bullet.Velocity = bulletCollision.VelocityB;
                        bulletCollision.Identity = bulletIdentity;
                        response = bulletCollision;
                    }
                }
                for (int i = cannonballs.Count - 1; i >= 0; i--)
                {
                    Circle cannonball = cannonballs[i];
                    CollisionResult cannonballCollision = physicsObject.Collide(s, cannonball);
                    if (cannonballCollision)
                    {
                        cannonballsToRemove.Add(cannonball);
                        cannonball.Position = cannonballCollision.PositionB;
                        //cannonball.Velocity = cannonballCollision.VelocityB;
                        cannonballCollision.Identity = cannonballIdentity;
                        response = cannonballCollision;
                    }
                }

                CollisionResult playerCollision = physicsObject.Collide(s, collision);
                if (playerCollision)
                {
                    if (collisionResult)
                    {
                        //Not sure if necessary, but if we have multiple collisions in a single frame, stop all movement.
                        collisionResult.PositionA = collision.Position;
                        collisionResult.VelocityA = Vector2.Zero;
                    }
                    else
                    {
                        collisionResult = playerCollision.Switch();
                    }
                    response = playerCollision;
                    TakeDamage(identity);
                }
            }
            return response;
        }

        public bool Dead()
        {
            return false;
        }

        public void Light(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureLight, collision.Position - new Vector2(105, 120), null, new Color(0, 255, 0), 0, Vector2.Zero, Vector2.One * 4, SpriteEffects.None, 1);
        }
    }
}
