using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Text
{
    public static class Dialogue
    {
        private static FragmentWriter Writer;
        private static bool Started;
        static Dialogue()
        {
            Writer = new FragmentWriter("Fonts/TooMuchInk", new Rectangle(0, 0, 320, 120), Color.White);
        }

        public static void Start(DialogueSequence dialogueSequence)
        {
            dialogueSequence.Reset();
            Writer.SetText(dialogueSequence.Fragments);
            Started = true;
        }

        public static void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (!Started) { return; }
            Writer.Draw(spriteBatch, position);
        }

        public static void Update(GameTime gameTime)
        {
            if (!Started) return;
            Writer.Update(gameTime);
        }

        public static void Load(ContentManager contentManager)
        {
            Writer.Load(contentManager);
        }
    }
}
