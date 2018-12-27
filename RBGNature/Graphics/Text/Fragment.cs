using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Text
{
    public class Fragment
    {
        const char WORD_SEPARATOR = ' ';
        const char EFFECT_START = '{';
        const char EFFECT_END = '}';
        const char PARAM_SEPARATOR = ':';

        public string Text { get; set; }
        private TextEffect Effect { get; set; }
        private int Index { get; set; }
        private int ElapsedTime { get; set; }
        private int FrameCount { get; set; }

        public Fragment(string text, TextEffect effect)
        {
            Text = text;
            Effect = effect;
            Index = 0;
            ElapsedTime = 0;

            if (Effect != null)
            {
                if (Effect.Wait > 0) ElapsedTime -= Effect.Wait;
            }
        }

        public void Reset()
        {
            Index = 0;
            ElapsedTime = 0;
            FrameCount = 0;
        }

        public Vector2 Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color color)
        {
            if (Effect != null && Effect.Color != default(Color)) color = Effect.Color;

            for (int i = 0; i < Index; i++)
            {
                string str = Text[i].ToString();
                Vector2 strPos = position;

                if (Effect != null)
                {
                    if (Effect.Is(TextEffectType.Jitter))
                    {
                        int amp = FrameCount % 3;
                        float sin = FrameCount % 6 / 3 > 0 ? amp : 1.5f - amp;
                        strPos += new Vector2(0, (sin - 1.5f) * .5f * (i % 2 > 0 ? 1 : -1));
                    }
                }
                
                spriteBatch.DrawString(font, str, strPos, color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                position.X += font.MeasureString(str).X;
            }

            FrameCount++;
            FrameCount = FrameCount % 60;

            return position;
        }

        public bool UpdateIndex(int elapsedTime, int speed)
        {
            if (Index == Text.Length) return false;

            if (Effect != null)
            {
                if (Effect.Speed > 0) speed = Effect.Speed;
                else if (Effect.Is(TextEffectType.Slow)) speed *= 2;
                else if (Effect.Is(TextEffectType.Fast)) speed /= 2;
            }

            ElapsedTime += elapsedTime;
            if (ElapsedTime > speed)
            {
                ElapsedTime -= speed;
                Index++;
            }

            return true;
        }

        public static List<Fragment> Parse(string source)
        {

            List<Fragment> fragments = new List<Fragment>();

            StringBuilder currentWord = new StringBuilder();
            int i = 0;
            while (i < source.Length)
            {
                char c = source[i];
                switch (c)
                {
                    case EFFECT_START:
                        // If we were in the middle of building a word, add it as a fragment.
                        if (currentWord.Length > 0)
                        {
                            fragments.Add(new Fragment(currentWord.ToString(), null));
                            currentWord.Clear();
                        }

                        int endBraceIndex = source.IndexOf(EFFECT_END, i);
                        string[] colon = source.Substring(i + 1, endBraceIndex - i - 1).Split(PARAM_SEPARATOR);
                        TextEffect effect = new TextEffect(colon[0]);
                        foreach (string word in colon[1].Split(WORD_SEPARATOR))
                        {
                            fragments.Add(new Fragment(word, effect));
                        }
                        i = endBraceIndex;
                        break;
                    case WORD_SEPARATOR:
                        if (currentWord.Length > 0)
                        {
                            fragments.Add(new Fragment(currentWord.ToString(), null));
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
                fragments.Add(new Fragment(currentWord.ToString(), null));
            }

            return fragments;
        }
    }
}
