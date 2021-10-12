using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MedabotsLib;
using MedabotsLib.Data;
using static MedabotsLib.IdTranslator;

namespace MedabotsRandomizer
{
    public class Randomizer
    {
        List<Battle> battles;
        List<Encounters> encounters;
        List<Part> parts;
        Random rng;

        List<int> randomizedMedals;
        public Dictionary<byte, byte> medalExchanges;
        public int starterMedal;

        public Randomizer(List<Battle> battles, List<Encounters> encounters, List<Part> parts, Random rng)
        {
            this.battles = battles;
            this.encounters = encounters;
            this.parts = parts;
            this.rng = rng;
            this.randomizedMedals = new List<int>();
            this.medalExchanges = new Dictionary<byte, byte>();

        }

        public Dictionary<byte, List<int>> findUniques(Medabot[] bots, int num_bots)
        {
            Dictionary<byte, List<int>> uniques = new Dictionary<byte, List<int>>();

            for (int i = 0; i < num_bots; i++)
            {
                if (uniques.ContainsKey(bots[i].head))
                {
                    uniques[bots[i].head].Add(i);
                }
                else
                {
                    uniques.Add(bots[i].head, new List<int>() { i });
                }
            }

            return uniques;
        }

        public void RandomizeBattles(bool keep_team_structure, bool balanced_medal_level, float mixedchance, bool continuity)
        {
            if (keep_team_structure)
            {
                if (continuity)
                {
                    Dictionary<byte, List<int>> uniques = findCharacterOccurences(battles);

                    foreach (KeyValuePair<byte, List<int>> entry in uniques)
                    {
                        List<byte> diffBots = new List<byte>();

                        foreach (int i in entry.Value)
                        {
                            Battle battle = battles[i];

                            for (int botIndex = 0; botIndex < battle.number_of_bots; botIndex++)
                            {
                                Medabot bot = battle.bots[botIndex];

                                if (!diffBots.Contains(bot.head))
                                    diffBots.Add(bot.head);
                            }
                        }

                        List<Medabot> newBots = new List<Medabot>();

                        foreach (byte i in diffBots)
                        {
                            newBots.Add(GenerateRandomBot(mixedchance));
                        }

                        foreach (int i in entry.Value)
                        {
                            Battle battle = battles[i];

                            for (int botIndex = 0; botIndex < battle.number_of_bots; botIndex++)
                            {
                                Medabot bot = battle.bots[botIndex];
                                Medabot newBot = newBots[diffBots.IndexOf(bot.head)];

                                if (balanced_medal_level)
                                    newBot.medal_level = bot.medal_level;

                                battle.bots[botIndex] = newBot;
                            }
                        }
                    }
                }
                else
                {
                    foreach (Battle battle in battles)
                    {
                        Dictionary<byte, List<int>> uniques = findUniques(battle.bots, battle.number_of_bots);

                        foreach (KeyValuePair<byte, List<int>> entry in uniques)
                        {
                            Medabot newBot = GenerateRandomBot(mixedchance);

                            foreach (int i in entry.Value)
                            {
                                if (balanced_medal_level)
                                    newBot.medal_level = battle.bots[i].medal_level;

                                battle.bots[i] = newBot;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Battle battle in battles)
                {
                    for (int i = 0; i < battle.number_of_bots; i++)
                    {
                        Medabot newBot = GenerateRandomBot(mixedchance);

                        if (balanced_medal_level)
                            newBot.medal_level = battle.bots[i].medal_level;

                        battle.bots[i] = newBot;
                    }
                }
            }
        }

        public Dictionary<byte, List<int>> findCharacterOccurences(List<Battle> battles)
        {
            Dictionary<byte, List<int>> uniques = new Dictionary<byte, List<int>>();
            for (int i = 0; i < battles.Count; i++)
            {
                if (uniques.ContainsKey(battles[i].characterId))
                {
                    uniques[battles[i].characterId].Add(i);
                }
                else
                {
                    uniques.Add(battles[i].characterId, new List<int>() { i });
                }
            }

            return uniques;
        }

        public void RandomizeCharacters(bool continuity)
        {
            if (continuity)
            {
                Dictionary<byte, List<int>> uniques = findCharacterOccurences(battles);
                List<int> possibleChars = Enumerable.Range(1, 0x5f).ToList();

                foreach (KeyValuePair<byte, List<int>> entry in uniques)
                {
                    int index = rng.Next(possibleChars.Count);
                    byte character = (byte)possibleChars[index];

                    foreach (int id in entry.Value)
                    {
                        battles[id].characterId = character;
                    }

                    possibleChars.RemoveAt(index);
                }
            }
            else
            {
                foreach (Battle battle in battles)
                {
                    battle.characterId = (byte)rng.Next(1, 0x60);
                }
            }
        }

        private Medal_Id getBestMedal(Medabot bot)
        {
            return botMedal(bot.head);
        }

        public void fixSoftlock()
        {
            Battle odoroBattle = battles[0xA4];

            byte firstFixedPart = 0x0;
            if (odoroBattle.bots[0].isComplete())
            {
                odoroBattle.bots[0].head = firstFixedPart;
                odoroBattle.bots[0].right_arm = firstFixedPart;
                odoroBattle.bots[0].left_arm = firstFixedPart;
                odoroBattle.bots[0].legs = firstFixedPart;
                odoroBattle.bots[0].medal = IdTranslator.botMedal(firstFixedPart);
            }
            else
            {
                odoroBattle.bots[0].head = firstFixedPart;
            }

            byte secondFixedPart = 0x4;
            if (odoroBattle.bots[1].isComplete())
            {
                odoroBattle.bots[1].head = secondFixedPart;
                odoroBattle.bots[1].right_arm = secondFixedPart;
                odoroBattle.bots[1].left_arm = secondFixedPart;
                odoroBattle.bots[1].legs = secondFixedPart;
                odoroBattle.bots[1].medal = IdTranslator.botMedal(secondFixedPart);
            }
            else
            {
                odoroBattle.bots[1].right_arm = secondFixedPart;
            }

            Battle kappaBattle = battles[0x39];
            int i = rng.Next(0, 3);

            byte kappaFixedPart = 0x6C;
            if (kappaBattle.bots[i].isComplete())
            {
                kappaBattle.bots[i].head = kappaFixedPart;
                kappaBattle.bots[i].right_arm = kappaFixedPart;
                kappaBattle.bots[i].left_arm = kappaFixedPart;
                kappaBattle.bots[i].legs = kappaFixedPart;
                kappaBattle.bots[i].medal = botMedal(kappaFixedPart);
            }
            else
            {
                kappaBattle.bots[i].head = kappaFixedPart;
            }
        }

        public Medabot GenerateRandomBot(float mixedchance)
        {
            Medabot bot = new Medabot();
            bot.unknown = 1;

            if (mixedchance != 0 && mixedchance >= rng.NextDouble())
            {
                bot.head = (byte)rng.Next(0, 0x78);
                bot.left_arm = (byte)rng.Next(0, 0x78);
                bot.right_arm = (byte)rng.Next(0, 0x78);
                bot.legs = (byte)rng.Next(0, 0x78);
                bot.medal_level = (byte)rng.Next(1, 100);
            }
            else
            {
                byte set = (byte)rng.Next(0, 0x78);
                bot.head = set;
                bot.left_arm = set;
                bot.right_arm = set;
                bot.legs = set;
                bot.medal_level = (byte)rng.Next(1, 100);
            }

            bot.medal = getBestMedal(bot);
            return bot;
        }

        public byte GetRandomMedal(byte medal)
        {
            var medalId = (byte)rng.Next(0, 0x1E);

            while (true)
            {
                //new medal
                if (!randomizedMedals.Contains(medalId) && medalId != starterMedal)
                {
                    randomizedMedals.Add(medalId);
                    medalExchanges.Add(medal, medalId);
                    return medalId;
                }

                //pick next closest medal to randomly selected, otherwise we would have to wait for rng to pick a new one itself
                                //29 - max id of medal
                if (medalId == 0x1D)
                {
                    medalId = 0x00;
                }
                else
                {
                    medalId++;
                }
            }
        }
    }
}
