using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RBGNature.Physics;

namespace RBGNature.Actor.Enemy
{
    class TestEnemy : BaseActor, ICollide
    {
        Circle collision;
        Texture2D textureFront;
        Texture2D textureCircle10;
        Texture2D textureBullet;
        Texture2D textureHPBar;
        Player player;
        List<Circle> bullets;
        public int CurrentHealth { get; private set; }
        const int MaxHealth = 10;
        bool tookDamage = false;

        public TestEnemy(Vector2 position, Player player)
        {
            collision = new Circle()
            {
                Position = position,
                Velocity = Vector2.Zero,
                Radius = 10,
                Mass = 1000000
            };
            this.player = player;
            bullets = new List<Circle>();
            CurrentHealth = MaxHealth;
        }

        public void Collide(PhysicsGroupType groupType, ICollide other)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                CollisionResult result = other.Collide(groupType, collision, null);
                if (result)
                {
                    takeDamage(result.Identity);
                }
            }
        }

        public CollisionResult Collide(PhysicsGroupType groupType, PhysicsObject physicsObject, CollisionIdentity identity)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                CollisionResult result = physicsObject.Collide(collision).Switch();
                if (result)
                {
                    takeDamage(identity);
                    return result;
                }
            }
            return CollisionResult.None;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
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
            spriteBatch.Draw(textureFront, collision.Position - new Vector2(10, 30), null, tint, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, LayerDepth(collision.Position.Y));
            spriteBatch.Draw(textureCircle10, collision.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            foreach (Circle bullet in bullets)
            {
                spriteBatch.Draw(textureBullet, bullet.Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
            spriteBatch.Draw(textureHPBar, collision.Position - new Vector2(0, 50), getHealthSpriteRect(), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, LayerDepth(collision.Position.Y));
        }

        public override void LoadContent(ContentManager contentManager)
        {
            textureFront = contentManager.Load<Texture2D>("Sprites/enemy/square flower - corrupt");
            textureBullet = contentManager.Load<Texture2D>("Sprites/enemy/effect/square flower - corrupt bullet");
            textureCircle10 = contentManager.Load<Texture2D>("Sprites/debug/circle10");
            textureHPBar = contentManager.Load<Texture2D>("UI/HPBar");
        }

        public override bool Dead()
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

        int timeBetweenShots = 0;
        Vector2 headOffset = new Vector2(0, -25);
        int timeBetweenDamage = 0;

        public override void Update(GameTime gameTime)
        {
            foreach (Circle bullet in bullets)
            {
                bullet.Position += bullet.Velocity;
            }

            float speed = .15f;
            int ellapsedTime = gameTime.ElapsedGameTime.Milliseconds;
            timeBetweenShots += ellapsedTime;
            if (timeBetweenShots > 1000)
            {
                timeBetweenShots = 0;
                Vector2 bulletOrigin = this.collision.Position + headOffset;
                Vector2 bulletDirection = player.collision.Position + player.collision.Velocity * 40 - bulletOrigin;
                Vector2 bulletVelocity = Vector2.Normalize(bulletDirection) * speed * ellapsedTime;
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
                timeBetweenDamage += ellapsedTime;
            }
            if (timeBetweenDamage > 100)
            {
                timeBetweenDamage = 0;
                tookDamage = false;
            }

        }
    }
}
