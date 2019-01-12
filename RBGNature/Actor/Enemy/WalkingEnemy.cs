using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RBGNature.Graphics.Animate;
using RBGNature.Physics;

namespace RBGNature.Actor.Enemy
{
    class WalkingEnemy : IAct, ICollide
    {
        enum WalkAnimation
        {
            Front,
            Back,
            Left,
            Right
        }

        static AnimationDictionary<WalkAnimation> animDict_Walk;


        public Circle collision;
        Texture2D textureFront;
        Texture2D textureCircle10;
        Texture2D textureBullet;
        Texture2D textureHPBar;
        Player player;
        List<Circle> bullets;
        public int CurrentHealth { get; private set; }
        const int MaxHealth = 100;
        bool tookDamage = false;
        Random random;
        Animator animator;

        static CollisionIdentity bulletIdentity = new CollisionIdentity()
        {
            Damage = 1
        };

        static WalkingEnemy()
        {
            animDict_Walk = new AnimationDictionary<WalkAnimation>();
            animDict_Walk[WalkAnimation.Left] = new Animation("Sprites/enemy/redTitan/redtitan_walk_left", 100, 10, 30, 30);
            animDict_Walk[WalkAnimation.Right] = new Animation("Sprites/enemy/redTitan/redtitan_walk_right", 100, 10, 30, 30);
        }

        public WalkingEnemy(Vector2 position, Player player)
        {
            collision = new Circle()
            {
                Position = position,
                Velocity = Vector2.Zero,
                Radius = 10,
                Mass = 10
            };
            this.player = player;
            bullets = new List<Circle>();
            CurrentHealth = MaxHealth;
            random = new Random(234);
            animator = new Animator(animDict_Walk[WalkAnimation.Left]);
        }

        private CollisionResult collisionResult = CollisionResult.None;

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
                        bullets.RemoveAt(i);
                        //bullet.Position = bulletCollision.PositionB;
                        //bullet.Velocity = bulletCollision.VelocityB;
                    }
                }
                CollisionResult result = other.Collide(s, groupType, collision, null);
                if (result)
                {
                    if (collisionResult)
                    {
                        collisionResult.PositionA = collision.Position;
                        collisionResult.VelocityA = Vector2.Zero;
                    }
                    else
                    {
                        collisionResult = result;
                    }
                    takeDamage(result.Identity);
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
                        bullets.RemoveAt(i);
                        //bullet.Position = bulletCollision.PositionB;
                        //bullet.Velocity = bulletCollision.VelocityB;
                        bulletCollision.Identity = bulletIdentity;
                        response = bulletCollision;
                    }
                }
                CollisionResult result = physicsObject.Collide(s, collision);
                if (result)
                {
                    if (collisionResult)
                    {
                        collisionResult.PositionA = collision.Position;
                        collisionResult.VelocityA = Vector2.Zero;
                    }
                    else
                    {
                        collisionResult = result.Switch();
                    }
                    takeDamage(identity);
                    response = result;
                }
            }
            return response;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Dead())
            {
                return;
            }
            Color tint = Color.White;
            if (tookDamage)
            {
                tint = Color.Red;
            }
            spriteBatch.Draw(animator.Texture, collision.Position - new Vector2(15, 25), animator.NextFrame(), tint, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, this.LayerDepth(collision.Position.Y));
            spriteBatch.Draw(textureCircle10, collision.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            foreach (Circle bullet in bullets)
            {
                spriteBatch.Draw(textureBullet, bullet.Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
            spriteBatch.Draw(textureHPBar, collision.Position - new Vector2(0, 50), getHealthSpriteRect(), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, this.LayerDepth(collision.Position.Y));
        }

        public void LoadContent(ContentManager contentManager)
        {
            textureFront = contentManager.Load<Texture2D>("Sprites/enemy/square flower - corrupt");
            textureBullet = contentManager.Load<Texture2D>("Sprites/enemy/effect/square flower - corrupt bullet");
            textureCircle10 = contentManager.Load<Texture2D>("Sprites/debug/circle10");
            textureHPBar = contentManager.Load<Texture2D>("UI/HPBar");
            animDict_Walk.Load(contentManager);
        }

        public bool Dead()
        {
            return CurrentHealth <= 0;
        }

        private Rectangle getHealthSpriteRect()
        {
            int index = 8 - (int) Math.Ceiling(((float) CurrentHealth / MaxHealth * 8));
            return new Rectangle(0, index*8, 8, 8);
        }

        private void takeDamage(CollisionIdentity identity)
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

        float timeBetweenShots = 0;
        Vector2 headOffset = new Vector2(0, -15);
        float timeBetweenDamage = 0;
        float speed = .05f;

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (collisionResult)
            {
                collision.Position = collisionResult.PositionA;
                collision.Velocity = collisionResult.VelocityA;
                collisionResult = CollisionResult.None;
            }
            else
            {
                collision.Position += collision.Velocity * elapsedTime;
                collision.Velocity *= .1f;
                //collision.Velocity = .5f * (new Vector2(random.Next(-1, 2), random.Next(-1, 2)) * 0.05f + collision.Velocity * 0.92f + Vector2.Normalize((player.collision.Position - collision.Position)) * 0.03f);

                //collision.Velocity = speed * Vector2.Normalize(player.collision.Position - collision.Position);
                
                if (collision.Velocity == Vector2.Zero)
                {
                    collision.Velocity = speed * Vector2.Normalize(player.collision.Position - collision.Position);
                }
                else
                {
                    collision.Velocity = speed * (Vector2.Normalize(collision.Velocity) * 0.92f + Vector2.Normalize(player.collision.Position - collision.Position) * .08f);
                }
                
            }

            bool left = false;
            if (Vector2.Dot(collision.Velocity, new Vector2(1,0)) < 0)
            {
                animator.Set(animDict_Walk[WalkAnimation.Left]);
                left = true;
            }
            else
            {
                animator.Set(animDict_Walk[WalkAnimation.Right]);
            }

            foreach (Circle bullet in bullets)
            {
                bullet.Position += bullet.Velocity * elapsedTime;
            }

            float bulletSpeed = .01f;
            timeBetweenShots += elapsedTime;
            if (timeBetweenShots > 500)
            {
                timeBetweenShots = 0;
                Vector2 bulletOrigin = this.collision.Position + headOffset + (left ? new Vector2(-4,0) : new Vector2(4,0));
                Vector2 bulletDirection = player.collision.Position + player.collision.Velocity * 40 - bulletOrigin;
                Vector2 bulletVelocity = Vector2.Normalize(bulletDirection) * bulletSpeed * elapsedTime;
                bullets.Add(new Circle()
                {
                    Position = bulletOrigin,
                    Velocity = bulletVelocity,
                    Mass = 1,
                    Radius = 2
                });
            }

            if (tookDamage)
            {
                timeBetweenDamage += elapsedTime;
            }
            if (timeBetweenDamage > 100)
            {
                timeBetweenDamage = 0;
                tookDamage = false;
            }

            animator.Update(gameTime);
        }

        public void Light(SpriteBatch spriteBatch)
        {
        }
    }
}
