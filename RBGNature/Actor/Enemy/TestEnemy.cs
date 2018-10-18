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

        public TestEnemy(Vector2 position)
        {
            collision = new Circle()
            {
                Position = position,
                Velocity = Vector2.Zero,
                Radius = 10,
                Mass = 1000000
            };
        }

        public void Collide(PhysicsGroupType groupType, ICollide other)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                other.Collide(groupType, collision);
            }
        }

        public CollisionResult Collide(PhysicsGroupType groupType, PhysicsObject physicsObject)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                return physicsObject.Collide(collision).Switch();
            }
            return CollisionResult.None;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureFront, collision.Position - new Vector2(10, 30), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, LayerDepth(collision.Position.Y));
            spriteBatch.Draw(textureCircle10, collision.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            textureFront = contentManager.Load<Texture2D>("Sprites/enemy/front");
            textureCircle10 = contentManager.Load<Texture2D>("Sprites/debug/circle10");
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
