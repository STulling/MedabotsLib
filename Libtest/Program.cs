using System;
using System.Collections.Generic;
using System.Linq;
using GBALib;
using MedabotsLib;
using MedabotsLib.Data;

namespace Libtest
{
    class Program
    {
        static void Main(string[] args)
        {
            MedaGame game = MedaGame.Load("game.gba");
            GameData.MedalNames[0].Str = "Cool Medal";
            GameData.MedalNames[GameData.MedalNames.IndexOf("BAT")].Str = "OBAMA";
            game.Save("out.gba");
            
        }
    }
}
