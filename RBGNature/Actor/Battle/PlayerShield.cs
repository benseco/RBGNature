using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RBGNature.Physics;

namespace RBGNature.Actor.Battle
{
    public class PlayerShield : IAct
    {
        private const float SHIELD_TIME = 1000f;
        private const float COOLDOWN = 5000f;
        private const float PERFECT_WINDOW = 250f;

        private Texture2D textureShield;
        private Circle playerCollision;
        private float uptime;
        private float timeSinceLastShield;

        public bool PerfectHit { get; set; }
        public bool Shielded { get; private set; }

        public PlayerShield(Circle playerCollision)
        {
            this.playerCollision = playerCollision;
            timeSinceLastShield = COOLDOWN;
        }

        public bool Dead()
        {
            return false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Shielded)
            {
                spriteBatch.Draw(textureShield, playerCollision.Position + new Vector2(-10, -20), null, PerfectHit ? Color.Yellow : Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
        }

        public void Light(SpriteBatch spriteBatch)
        {
        }

        public void LoadContent(ContentManager contentManager)
        {
            textureShield = contentManager.Load<Texture2D>("Sprites/battle/shield");
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (Shielded)
            {
                uptime += elapsedTime;

                if (uptime > SHIELD_TIME)
                {
                    Reset();
                }
            }
            else
            {
                timeSinceLastShield += elapsedTime;
                var kstate = Keyboard.GetState();

                if (kstate.IsKeyDown(Keys.Space))
                {
                    ShieldUp();
                }
            }
        }

        public void ShieldUp()
        {
            if (timeSinceLastShield > COOLDOWN)
            {
                Shielded = true;
            }
        }

        public void AlertHit()
        {
            if (!Shielded) return;

            if (uptime < PERFECT_WINDOW)
            {
                PerfectHit = true;
            }
            else if (!PerfectHit)
            {
                Reset();
            }
        }

        private void Reset()
        {
            Shielded = false;
            uptime = 0;
            timeSinceLastShield = PerfectHit ? COOLDOWN / 2f : 0;
            PerfectHit = false;
        }
    }
}
