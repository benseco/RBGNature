using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Text
{
    enum TextEffectType
    {
        Fast,
        Slow,
        Wait,
        Jitter,
        Color,
        //Bold,
        //Wave,
    }

    class TextEffect
    {
        List<TextEffectType> EffectTypes { get; set; }
        public int Speed { get; set; }
        public int Wait { get; set; }
        public Color Color { get; set; }

        public TextEffect(string typeList)
        {
            EffectTypes = new List<TextEffectType>();
            Color = default(Color);

            foreach (string typeString in typeList.Split(','))
            {
                string[] typeParams = typeString.Split('.');
                TextEffectType textEffectType;
                if (Enum.TryParse(typeParams[0], out textEffectType))
                {
                    EffectTypes.Add(textEffectType);
                    if (typeParams.Length > 1) ParseTypeParams(textEffectType, typeParams[1]);
                }
            }
        }

        private void ParseTypeParams(TextEffectType type, string paramString)
        {
            switch (type)
            {
                case TextEffectType.Fast:
                case TextEffectType.Slow:
                    Speed = int.Parse(paramString);
                    break;
                case TextEffectType.Wait:
                    Wait = int.Parse(paramString);
                    break;
                case TextEffectType.Color:
                    Color = ParseColor(paramString);
                    break;
            }
        }

        private Color ParseColor(string colorName)
        {
            switch (colorName)
            {
                case "Red":     return Color.Red;
                case "Orange":  return Color.Orange;
                case "Yellow":  return Color.Yellow;
                case "Green":   return Color.Green;
                case "Blue":    return Color.Blue;
                case "Indigo":  return Color.Indigo;
                case "Violet":  return Color.Violet;
                case "Brown":   return Color.Brown;
                case "Black":   return Color.Black;
                default:
                    string[] rgba = colorName.Split('|');
                    if (rgba.Length < 3) return default(Color);
                    int r = int.Parse(rgba[0]);
                    int g = int.Parse(rgba[1]);
                    int b = int.Parse(rgba[2]);
                    int a = rgba.Length > 3 ? int.Parse(rgba[3]) : 255;
                    return new Color(r, g, b, a);
            }
        }

        public bool Is(TextEffectType type)
        {
            return EffectTypes.Contains(type);
        }
    }
}
