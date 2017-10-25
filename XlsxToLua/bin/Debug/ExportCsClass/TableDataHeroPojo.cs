using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsClass
{
    public class TableDataHeroPojo
    {
        public int HeroId { get; set; }
        public string Name { get; set; }
        public int Rare { get; set; }
        public int Type { get; set; }
        public int DefaultStar { get; set; }
        public bool IsOpen { get; set; }
        public LitJson.JsonData Attributes { get; set; }
    }
}
