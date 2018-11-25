using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RBGNature.Graphics;
using RBGNature.Graphics.Animate;
using RBGNature.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        Camera camera;
        Texture2D textureMan;
        Texture2D textureBullet;

        Animator animator;

        List<Circle> bullets;
        public Circle collision;

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
            collision = new Circle()
            {
                Radius = 10,
                Position = new Vector2(camera.Position.X, camera.Position.Y),
                Mass = 1
            };

            animator = new Animator(animDict_Run[RunAnimation.Front]);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            animDict_Run.Load(contentManager);

            textureMan = contentManager.Load<Texture2D>("Sprites/mc/front");
            textureBullet = contentManager.Load<Texture2D>("Sprites/bullet/bullet");
            textureCircle10 = contentManager.Load<Texture2D>("Sprites/debug/circle10");
            textureCircle200 = contentManager.Load<Texture2D>("Sprites/debug/circle200");
        }

        public override void Update(GameTime gameTime)
        {
            collision.Position += collision.Velocity;
            camera.MoveTo(collision.Position);

            float speed = .15f;
            int elapsedTime = gameTime.ElapsedGameTime.Milliseconds;
            float distance = speed * elapsedTime;

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
            timeBetweenShots += elapsedTime;
            if (mstate.LeftButton == ButtonState.Pressed && timeBetweenShots > 100)
            {
                timeBetweenShots = 0;
                bullets.Add(new Circle()
                {
                    Position = camera.Position,
                    Velocity = Vector2.Normalize(mstate.Position.ToVector2() - camera.FocalPoint) * 4,
                    Mass = 1,
                });
            }

            foreach (Circle bullet in bullets)
            {
                bullet.Position += bullet.Velocity;
            }

            animator.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(animator.Animation.Texture, camera.Position - new Vector2(10,30), animator.NextFrame(), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, LayerDepth(collision.Position.Y));

            spriteBatch.Draw(textureCircle10, camera.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            spriteBatch.Draw(textureCircle200, new Vector2(100,100), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            if (mode == GunMode.Default)
            {
                foreach (Circle bullet in bullets)
                {
                    spriteBatch.Draw(textureBullet, bullet.Position, RectBulletDefault, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
                }
            }
            else if (mode == GunMode.Cannon)
            {
                foreach (Circle bullet in bullets)
                {
                    spriteBatch.Draw(textureBullet, bullet.Position, RectBulletCannon, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
                }

            }

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
                CollisionResult playerCollision = other.Collide(groupType, this.collision, null);
                if (playerCollision)
                {
                    collision.Position = playerCollision.PositionB;
                    collision.Velocity = playerCollision.VelocityB - collision.Velocity;
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

                CollisionResult playerCollision = physicsObject.Collide(collision);
                if (playerCollision)
                {
                    collision.Position = playerCollision.PositionB;
                    collision.Velocity = playerCollision.VelocityB - collision.Velocity;
                    response = playerCollision;
                }
            }
            return response.Switch();
        }
    }
}
