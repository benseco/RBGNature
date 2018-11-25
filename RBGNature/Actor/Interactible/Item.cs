using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RBGNature.Actor.Interactible
{
    class Item : BaseActor
    {
        private Player Player { get; set; }
        private Vector2 Position { get; set; }

        private bool ShowInteractIcon { get; set; }
        private bool Taken { get; set; }
        private Texture2D TextureInteractIcon { get; set; }
        private Texture2D TextureItem { get; set; }

        public Item(Player player, Vector2 position)
        {
            Player = player;
            Position = position;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureItem, Position - new Vector2(6,6), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            if (ShowInteractIcon)
            {
                spriteBatch.Draw(TextureInteractIcon, Position - new Vector2(5, 16), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
        }

        public override void LoadContent(ContentManager contentManager)
        {
            TextureInteractIcon = contentManager.Load<Texture2D>("UI/interactButton");
            TextureItem = contentManager.Load<Texture2D>("Sprites/item/maxRevive");
        }

        public override void Update(GameTime gameTime)
        {
            if (Vector2.DistanceSquared(Player.collision.Position, Position) < 800)
            {
                ShowInteractIcon = true;
                var kstate = Keyboard.GetState();
                if (kstate.IsKeyDown(Keys.E))
                {
                    //Player.IncrementItem();
                    Taken = true;
                }
            }
            else
            {
                ShowInteractIcon = false;
            }

            
        }

        public override bool Dead()
        {
            return Taken;
        }
    }
}
