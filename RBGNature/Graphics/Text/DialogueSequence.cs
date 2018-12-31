using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Graphics.Text
{
    public enum PlateType
    {
        Default
    }

    public class DialogueSequence
    {
        private List<DialogueBite> Bites { get; set; }
        private int Current { get; set; }

        public List<Fragment> Fragments
        {
            get
            {
                return Bites[Current].Fragments;
            }
        } 

        public PlateType Plate
        {
            get
            {
                return Bites[Current].Plate;
            }
        }

        public DialogueSequence()
        {
            Bites = new List<DialogueBite>();
        }

        public void Add(string name, string text, Texture2D portrait = null, PlateType plate = PlateType.Default)
        {
            Bites.Add(new DialogueBite(name, Fragment.Parse(text), portrait, plate));
        }

        public void Reset()
        {
            Current = 0;
        }

        public bool Next()
        {
            if (++Current < Bites.Count)
            {
                return true;
            }
            else
            {
                Reset();
                return false;
            }
        }
    }
}
