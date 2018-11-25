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
        const char SPACE = ' ';
        const char BRACE_LEFT = '{';
        const char BRACE_RIGHT = '}';
        const char COLON = ':';

        SpriteFont Font { get; set; }
        Rectangle TextArea { get; set; }
        Color Color { get; set; }
        int Speed { get; set; }

        List<Fragment> Fragments { get; set; }

        private string FontKey { get; set; }
        private int Index { get; set; }
        private Vector2 Space { get; set; }
        public Vector2 Origin { get; set; }

        public FragmentWriter(string fontKey, string source, Rectangle textArea, Color color, int speed = 100)
        {
            FontKey = fontKey;
            TextArea = textArea;
            Origin = textArea.Location.ToVector2();
            Color = color;
            Speed = speed;
            Index = 0;
            ParseFragments(source);
        }

        private void ParseFragments(string source)
        {
            Fragments = new List<Fragment>();

            StringBuilder currentWord = new StringBuilder();
            int i = 0;
            while(i < source.Length)
            {
                char c = source[i];
                switch(c)
                {
                    case BRACE_LEFT:
                        // If we were in the middle of building a word, add it as a fragment.
                        if (currentWord.Length > 0)
                        {
                            Fragments.Add(new Fragment(currentWord.ToString(), null));
                            currentWord.Clear();
                        }
                        
                        int endBraceIndex = source.IndexOf(BRACE_RIGHT, i);
                        string[] colon = source.Substring(i + 1, endBraceIndex - i - 1).Split(COLON);
                        TextEffect effect = new TextEffect(colon[0]);
                        foreach (string word in colon[1].Split(SPACE))
                        {
                            Fragments.Add(new Fragment(word, effect));
                        }
                        i = endBraceIndex;
                        break;
                    case SPACE:
                        if (currentWord.Length > 0)
                        {
                            Fragments.Add(new Fragment(currentWord.ToString(), null));
                            currentWord.Clear();
                        }
                        break;
                    default:
                        currentWord.Append(c);
                        break;
                }
                i++;
            }
            if (currentWord.Length > 0)
            {
                Fragments.Add(new Fragment(currentWord.ToString(), null));
            }
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

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 position = Origin;

            // Actually do the Draw
            for (int i = 0; i <= Index; i++)
            {
                Fragment fragment = Fragments[i];

                Vector2 fragmentLength = Font.MeasureString(fragment.Text);
                if (position.X + fragmentLength.X - Origin.X > TextArea.Width)
                {
                    position = new Vector2(Origin.X, position.Y + Font.LineSpacing);
                }

                position = fragment.Draw(spriteBatch, Font, position, Color);
                position += new Vector2(Space.X, 0);
            }

        }

        public void Load(ContentManager contentManager)
        {
            Font = contentManager.Load<SpriteFont>(FontKey);
            Space = Font.MeasureString(SPACE.ToString());
        }
    }
}
