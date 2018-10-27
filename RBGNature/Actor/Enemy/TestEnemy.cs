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
        Player player;
        List<Circle> bullets;

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
        }

        public void Collide(PhysicsGroupType groupType, ICollide other)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                other.Collide(groupType, collision, null);
            }
        }

        public CollisionResult Collide(PhysicsGroupType groupType, PhysicsObject physicsObject, CollisionIdentity identity)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                return physicsObject.Collide(collision).Switch();
            }
            return CollisionResult.None;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureFront, collision.Position - new Vector2(10, 30), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, LayerDepth(collision.Position.Y));
            spriteBatch.Draw(textureCircle10, collision.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            foreach (Circle bullet in bullets)
            {
                spriteBatch.Draw(textureBullet, bullet.Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
        }

        public override void LoadContent(ContentManager contentManager)
        {
            textureFront = contentManager.Load<Texture2D>("Sprites/enemy/square flower - corrupt");
            textureBullet = contentManager.Load<Texture2D>("Sprites/enemy/effect/square flower - corrupt bullet");
            textureCircle10 = contentManager.Load<Texture2D>("Sprites/debug/circle10");
        }

        int timeBetweenShots = 0;
        Vector2 headOffset = new Vector2(0, -25);

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



        }
    }
}
