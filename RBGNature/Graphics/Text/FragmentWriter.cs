using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Text
{

    class FragmentWriter
    {
        const char SPACE = ' ';
        const char BRACE_LEFT = '{';
        const char BRACE_RIGHT = '}';
        const char COLON = ':';

        int Index { get; set; }
        int Length { get; set; }
        int Speed { get; set; }
        List<Fragment> Fragments { get; set; }

        public FragmentWriter(string source, int speed = 100)
        {
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

            foreach (Fragment fragment in Fragments)
            {
                Length += fragment.Text.Length;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, SpriteFont font, Vector2 position, Color color)
        {
            Vector2 space = font.MeasureString(SPACE.ToString());
            
            // Calculate new Index
            Fragment currentFragment = Fragments[Index];
            bool fragmentDone = !currentFragment.UpdateIndex(gameTime.ElapsedGameTime.Milliseconds, Speed);
            if (fragmentDone && Index < Fragments.Count - 1) Index++;

            // Actually do the Draw
            for (int i = 0; i <= Index; i++)
            {
                Fragment fragment = Fragments[i];
                position = fragment.Draw(spriteBatch, font, position, Color.White);
                position.X += space.X;
            }

        }
    }
}
