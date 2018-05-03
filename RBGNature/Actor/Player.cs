using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RBGNature.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Actor
{
    class Player : BaseActor
    {
        Camera camera;
        Texture2D textureMan;

        public Player(Camera camera)
        {
            this.camera = camera;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            textureMan = contentManager.Load<Texture2D>("Sprites/mc/front");
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 direction = Vector2.Zero;
            float speed = .25f;
            float elapsedTime = gameTime.ElapsedGameTime.Milliseconds;
            float distance = speed * elapsedTime;

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up))
                direction.Y -= distance;

            if (kstate.IsKeyDown(Keys.Down))
                direction.Y += distance;

            if (kstate.IsKeyDown(Keys.Left))
                direction.X -= distance;

            if (kstate.IsKeyDown(Keys.Right))
                direction.X += distance;

            if (Tri.PIT(camera.Position.X, camera.Position.Y, 100, 100, 100, 600, 600, 100) && direction.Y == 0)
            {
                if (direction.X > 0)
                {
                    direction.Y += distance;
                }
                else if (direction.X < 0)
                {
                    direction.Y -= distance;
                }
            }

            camera.Move(direction);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureMan, camera.Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, .5f);
        }
    }
}
