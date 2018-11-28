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
    class Player : BaseActor, ICollide
    {
        enum RunAnimation
        {
            Front,
            Back
        }
        
        static readonly Rectangle RectBulletDefault = new Rectangle(0, 0, 6, 6);
        static readonly Rectangle RectBulletCannon = new Rectangle(6, 0, 9, 9);

        static AnimationDictionary<RunAnimation> animDict_Run;

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
        SoundEffect soundEffectGunshot;

        public int CurrentHealth { get; private set; }
        const int MaxHealth = 15;
        bool tookDamage = false;
        int timeBetweenDamage = 0;
        Animator animator;

        List<Circle> bullets;
        List<Circle> cannonballs;
        public Circle collision;
        private CollisionResult collisionResult;
        private bool nextCollisionSet;

        Texture2D textureCircle10;
        Texture2D textureCircle200;

        GunMode mode;
        private bool canChangeMode;

        private int timeBetweenShots;

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
            animDict_Run = new AnimationDictionary<RunAnimation>();
            animDict_Run[RunAnimation.Front] = new Animation("Sprites/mc/mc_run_front", 100, 8, 14, 38);
            animDict_Run[RunAnimation.Back] = new Animation("Sprites/mc/mc_run_back", 100, 8, 14, 38);
        }

        public Player(Camera camera)
        {
            this.camera = camera;
            this.bullets = new List<Circle>();
            this.cannonballs = new List<Circle>();
            collision = new Circle()
            {
                Radius = 10,
                Position = new Vector2(camera.Position.X, camera.Position.Y),
                Mass = 1
            };
            collisionResult = CollisionResult.None;
            CurrentHealth = MaxHealth;
            animator = new Animator(animDict_Run[RunAnimation.Front]);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            animDict_Run.Load(contentManager);

            textureMan = contentManager.Load<Texture2D>("Sprites/mc/front");
            textureBullet = contentManager.Load<Texture2D>("Sprites/bullet/bullet");
            textureCannonball = contentManager.Load<Texture2D>("Sprites/bullet/cannonball");
            textureCircle10 = contentManager.Load<Texture2D>("Sprites/debug/circle10");
            textureCircle200 = contentManager.Load<Texture2D>("Sprites/debug/circle200");
            textureHPBar = contentManager.Load<Texture2D>("UI/HPBar");

            soundEffectGunshot = contentManager.Load<SoundEffect>("Sound/effect/gunshot");
        }

        public override void Update(GameTime gameTime)
        {
            if (collisionResult)
            {
                // If we had a collision, start over with the collision result
                collision.Position = collisionResult.PositionA;
                collision.Velocity = collisionResult.VelocityA;
                collisionResult = CollisionResult.None;
            }
            else
            {
                collision.Position += collision.Velocity;
            }

            camera.MoveTo(collision.Position);

            float speed = .15f;
            int ellapsedTime = gameTime.ElapsedGameTime.Milliseconds;
            float distance = speed * ellapsedTime;

            Vector2 inputVelocity = Vector2.Zero;

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W))
            {
                inputVelocity.Y -= distance;
                animator.Animate(animDict_Run[RunAnimation.Back]);
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                inputVelocity.Y += distance;
                animator.Animate(animDict_Run[RunAnimation.Front]);
            }

            if (kstate.IsKeyDown(Keys.A))
            {
                inputVelocity.X -= distance;
            }

            if (kstate.IsKeyDown(Keys.D))
            {
                inputVelocity.X += distance;
            }
            collision.Velocity = collision.Velocity * 0.7f + inputVelocity * 0.3f;



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


            var mstate = Mouse.GetState();
            timeBetweenShots += ellapsedTime;
            if (mstate.LeftButton == ButtonState.Pressed && timeBetweenShots > 100)
            {
                timeBetweenShots = 0;
                if (mode == GunMode.Default)
                {
                    bullets.Add(new Circle()
                    {
                        Position = camera.Position,
                        Velocity = Vector2.Normalize(mstate.Position.ToVector2() - camera.FocalPoint) * 4,
                        Mass = 1,
                        Radius = 3
                    });
                }
                else if (mode == GunMode.Cannon)
                {
                    cannonballs.Add(new Circle()
                    {
                        Position = camera.Position,
                        Velocity = Vector2.Normalize(mstate.Position.ToVector2() - camera.FocalPoint) * 2,
                        Mass = 1,
                        Radius = 6
                    });
                }

                soundEffectGunshot.CreateInstance().Play();
            }

            foreach (Circle bullet in bullets)
            {
                bullet.Position += bullet.Velocity;
            }
            foreach (Circle cannonball in cannonballs)
            {
                cannonball.Position += cannonball.Velocity;
            }

            animator.Update(gameTime);
            if (tookDamage)
            {
                timeBetweenDamage += ellapsedTime;
            }
            if (timeBetweenDamage > 100)
            {
                timeBetweenDamage = 0;
                tookDamage = false;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = Color.White;
            if (tookDamage)
            {
                tint = Color.Red;
            }
            spriteBatch.Draw(animator.Animation.Texture, camera.Position - new Vector2(10,30), animator.NextFrame(), tint, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, LayerDepth(collision.Position.Y));

            spriteBatch.Draw(textureCircle10, camera.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            spriteBatch.Draw(textureCircle200, new Vector2(100,100), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
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

        public void Collide(PhysicsGroupType groupType, ICollide other)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    Circle bullet = bullets[i];
                    CollisionResult bulletCollision = other.Collide(groupType, bullet, bulletIdentity);
                    if (bulletCollision)
                    {
                        bullets.RemoveAt(i);
                        //bullet.Position = bulletCollision.PositionB;
                        //bullet.Velocity = bulletCollision.VelocityB;
                    }

                }
                for (int i = cannonballs.Count - 1; i >= 0; i--)
                {
                    Circle cannonball = cannonballs[i];
                    CollisionResult cannonballCollision = other.Collide(groupType, cannonball, cannonballIdentity);
                    if (cannonballCollision)
                    {
                        cannonballs.RemoveAt(i);
                        //bullet.Position = bulletCollision.PositionB;
                        //bullet.Velocity = bulletCollision.VelocityB;
                    }

                }
                CollisionResult playerCollision = other.Collide(groupType, this.collision, null);
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

        public CollisionResult Collide(PhysicsGroupType groupType, PhysicsObject physicsObject, CollisionIdentity identity)
        {
            CollisionResult response = CollisionResult.None;
            if (groupType == PhysicsGroupType.Physical)
            {
                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    Circle bullet = bullets[i];
                    CollisionResult bulletCollision = physicsObject.Collide(bullet);
                    if (bulletCollision)
                    {
                        bullets.RemoveAt(i);
                        //bullet.Position = bulletCollision.PositionB;
                        //bullet.Velocity = bulletCollision.VelocityB;
                        bulletCollision.Identity = bulletIdentity;
                        response = bulletCollision;
                    }
                }
                for (int i = cannonballs.Count - 1; i >= 0; i--)
                {
                    Circle cannonball = cannonballs[i];
                    CollisionResult cannonballCollision = physicsObject.Collide(cannonball);
                    if (cannonballCollision)
                    {
                        cannonballs.RemoveAt(i);
                        //bullet.Position = bulletCollision.PositionB;
                        //bullet.Velocity = bulletCollision.VelocityB;
                        cannonballCollision.Identity = cannonballIdentity;
                        response = cannonballCollision;
                    }
                }

                CollisionResult playerCollision = physicsObject.Collide(collision);
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
    }
}
