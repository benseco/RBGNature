using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RBGNature.Actor.Battle;
using RBGNature.Graphics.Animate;
using RBGNature.Physics;
using System;
using System.Collections.Generic;

namespace RBGNature.Actor
{
    public class Player : IAct, ICollide
    {
        enum FourDirectionAnimation
        {
            Front,
            Back,
            Left,
            Right
        }
        static AnimationDictionary<FourDirectionAnimation> animDict_Run;
        static AnimationDictionary<FourDirectionAnimation> animDict_Idle;
        static AnimationDictionary<FourDirectionAnimation> animDict_RunStop;

        Camera camera;
        Texture2D textureMan;
        Texture2D textureHPBar;
        Texture2D textureLight;
        Texture2D textureCircle10;
        Texture2D textureCircle200;

        public int CurrentHealth { get; private set; }
        const int MaxHealth = 15;
        bool tookDamage = false;
        float timeBetweenDamage = 0;
        Animator animator;

        public Circle collision;
        private CollisionResult collisionResult;

        public PlayerWeapon Weapon { get; set; }
        public PlayerShield Shield { get; set; }

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
            collision = new Circle()
            {
                Radius = 10,
                Position = new Vector2(camera.Position.X, camera.Position.Y),
                Mass = 1
            };
            collisionResult = CollisionResult.None;
            CurrentHealth = MaxHealth;
            animator = new Animator(animDict_Idle[FourDirectionAnimation.Front]);
            Weapon = new PlayerWeapon(camera);
            Shield = new PlayerShield(collision);
        }

        public void LoadContent(ContentManager contentManager)
        {
            animDict_Run.Load(contentManager);
            animDict_Idle.Load(contentManager);
            animDict_RunStop.Load(contentManager);

            textureMan = contentManager.Load<Texture2D>("Sprites/mc/front");
            textureCircle10 = contentManager.Load<Texture2D>("Sprites/debug/circle10");
            textureCircle200 = contentManager.Load<Texture2D>("Sprites/debug/circle200");
            textureHPBar = contentManager.Load<Texture2D>("UI/HPBar");

            textureLight = contentManager.Load<Texture2D>("Sprites/light/53");

            Weapon.LoadContent(contentManager);
            Shield.LoadContent(contentManager);
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
            CollisionResult old = collisionResult;
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

            Weapon.Update(gameTime);
            Shield.Update(gameTime);

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
            spriteBatch.Draw(animator.Texture, camera.Position - new Vector2(8,30), animator.NextFrame(), tint, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, this.LayerDepth(collision.Position.Y));

            spriteBatch.Draw(textureCircle10, camera.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            //spriteBatch.Draw(textureCircle200, new Vector2(100,100), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            spriteBatch.Draw(textureHPBar, collision.Position - new Vector2(0, 50), getHealthSpriteRect(), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);

            Weapon.Draw(gameTime, spriteBatch);
            Shield.Draw(gameTime, spriteBatch);
        }

        private void TakeDamage(CollisionIdentity identity)
        {
            if (identity == null)
                return;
            if (identity.Damage > 0)
            {
                if (Shield.Shielded)
                {
                    Shield.AlertHit();
                }
                else
                {
                    CurrentHealth -= identity.Damage;
                    tookDamage = true;
                    timeBetweenDamage = 0;
                }
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
                Weapon.Collide(s, groupType, other);

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
                response = Weapon.Collide(s, groupType, physicsObject, identity);

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
