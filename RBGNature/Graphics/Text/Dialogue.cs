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
        private static DialogueSequence Sequence;
        private static FragmentWriter Writer;
        private static bool Started;

        private static Dictionary<PlateType,Texture2D> PlateDictionary;

        static Dialogue()
        {
            Writer = new FragmentWriter("Fonts/TooMuchInk", new Rectangle(0, 0, 300, 100), Color.Black);
        }

        public static void Start(DialogueSequence dialogueSequence)
        {
            Sequence = dialogueSequence;
            Sequence.Reset();
            Writer.SetText(dialogueSequence.Fragments);
            Started = true;
        }

        public static void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (!Started) { return; }
            spriteBatch.Draw(PlateDictionary[Sequence.Plate], position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, .999f);
            Writer.Draw(spriteBatch, position + new Vector2(10,10));
        }

        public static void Update(GameTime gameTime)
        {
            if (!Started) return;
            Writer.Update(gameTime);
        }

        public static void Load(ContentManager contentManager)
        {
            Writer.Load(contentManager);
            LoadPlateTextures(contentManager);
        }

        private static void LoadPlateTextures(ContentManager contentManager)
        {
            PlateDictionary = new Dictionary<PlateType, Texture2D>();
            PlateDictionary[PlateType.Default] = contentManager.Load<Texture2D>("UI/DialogueBox");
        }
    }
}
