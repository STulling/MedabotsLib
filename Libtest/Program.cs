using System;
using GBALib;
using MedabotsLib;
using MedabotsLib.Data;

namespace Libtest
{
    class Program
    {
        static void Main(string[] args)
        {
            Game.Load("game.gba");
            GameData.LoadAll();
            Console.WriteLine(GameData.MedalNames[0].Str);
            GameData.MedalNames[0].Str = "Cool New Medal Name";
            GameData.MedalNames.Verify();
            Console.WriteLine(GameData.PreBattleMessage);
            Console.WriteLine(GameData.Messages[0]);
            Console.WriteLine(GameData.Battles[100]);
        }
    }
}
