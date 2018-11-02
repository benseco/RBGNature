using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Text
{
    enum TextEffectType
    {
        Jitter,
        Brown,
        Bold,
        Fast,
        Wait,
        Wave,
        Slow
    }

    class TextEffect
    {
        List<TextEffectType> EffectTypes { get; set; }
        public int Speed { get; set; }
        public int Wait { get; set; }

        public TextEffect(string typeList)
        {
            EffectTypes = new List<TextEffectType>();

            foreach (string typeString in typeList.Split(','))
            {
                string[] typeParams = typeString.Split('.');
                TextEffectType textEffectType;
                if (Enum.TryParse(typeParams[0], out textEffectType))
                {
                    EffectTypes.Add(textEffectType);
                    if (typeParams.Length > 1)
                    {
                        int time = int.Parse(typeParams[1]);
                        if (textEffectType == TextEffectType.Wait) Wait = time;
                        else Speed = int.Parse(typeParams[1]);
                    }
                }
            }
        }

        public bool Is(TextEffectType type)
        {
            return EffectTypes.Contains(type);
        }
    }
}
