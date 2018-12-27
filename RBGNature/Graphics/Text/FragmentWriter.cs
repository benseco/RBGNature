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

    public class FragmentWriter
    {
        const char CHAR_SPACE = ' ';

        SpriteFont Font { get; set; }
        Rectangle TextArea { get; set; }
        Color Color { get; set; }
        int Speed { get; set; }

        List<Fragment> Fragments { get; set; }

        private string FontKey { get; set; }
        private int Index { get; set; }
        private Vector2 Space { get; set; }

        public FragmentWriter(string fontKey, Rectangle textArea, Color color, int speed = 100)
        {
            FontKey = fontKey;
            TextArea = textArea;
            Color = color;
            Speed = speed;
            Index = 0;
        }

        public void SetText(List<Fragment> fragments)
        {
            Fragments = fragments;
            Fragments.ForEach(f => f.Reset());
            Index = 0;
        }

        public void Update(GameTime gameTime)
        {
            // Calculate new Index
            Fragment currentFragment = Fragments[Index];
            bool fragmentDone = !currentFragment.UpdateIndex(gameTime.ElapsedGameTime.Milliseconds, Speed);
            if (fragmentDone && Index < Fragments.Count - 1)
            {
                Index++;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 origin)
        {
            Vector2 position = origin;

            // Actually do the Draw
            for (int i = 0; i <= Index; i++)
            {
                Fragment fragment = Fragments[i];

                Vector2 fragmentLength = Font.MeasureString(fragment.Text);
                if (position.X + fragmentLength.X - origin.X > TextArea.Width)
                {
                    position = new Vector2(origin.X, position.Y + Font.LineSpacing);
                }

                position = fragment.Draw(spriteBatch, Font, position, Color);
                position += new Vector2(Space.X, 0);
            }

        }

        public void Load(ContentManager contentManager)
        {
            Font = contentManager.Load<SpriteFont>(FontKey);
            Space = Font.MeasureString(CHAR_SPACE.ToString());
        }
    }
}
