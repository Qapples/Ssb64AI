using System;
using System.Collections.Generic;
using System.IO;

namespace Smash64Network
{
    public class ValueGet
    {
        /// <summary>
        /// Writes to the stream and returns what was read in the stream. 
        /// </summary>
        /// <param name="reader">Used to read data coming from Bizhawk</param>
        /// <param name="writer">Used to write data to Bizhawk</param>
        /// <param name="message"></param>
        public static string WriteAndRead(StreamReader reader, StreamWriter writer, string message)
        {
            writer.WriteLine(message);
            writer.Flush();

            return reader.ReadLine();
        }

        /// <summary>
        /// Gets all the values under the enum "States" and selects a random one
        /// </summary>
        /// <param name="rng">Random Number Generator</param>
        public static States GetRandomState(Random rng) =>
            (States) Enum.GetValues(typeof(States)).GetValue(rng.Next(17));

        /// <summary>
        /// Generate new traits.
        /// </summary>
        /// <param name="attackList">List of Attack Traits</param>
        /// <param name="evadeList">List of Evade Traits</param>
        /// <param name="rng">Random Number Generator</param>
        public static void SetTraitList(ref List<AttackTrait> attackList, ref List<EvadeTrait> evadeList, Random rng)
        {
            for (int i = 0; i < 20; i++)
            {
                attackList.Add(new AttackTrait(rng));
                evadeList.Add(new EvadeTrait(rng));
            }
        }

        /// <summary>
        /// Get difference between current damage and previous damage
        /// </summary>
        /// <param name="damageTwo">Current damage of player two</param>
        /// <param name="previousDamageTwo">Previous damage of player two</param>
        public static double CalculateAttackPoints(double damageTwo, double previousDamageTwo) =>
            damageTwo - previousDamageTwo;

        /// <summary>
        /// Get difference between player one and player two position 
        /// </summary>
        /// <param name="xOne">X position of player one</param>
        /// <param name="yOne">Y position of player one</param>
        /// <param name="xTwo">X position of player two</param>
        /// <param name="yTwo">Y position of player two</param>
        /// <returns></returns>
        public static double CalculateEvadePoints(double xOne, double yOne, double xTwo, double yTwo) =>
            1000 - ((xOne - xTwo) + (yOne - yTwo));
    }
}