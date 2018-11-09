﻿using System;
using System.IO;

namespace Smash64Network
{
    public class AttackTrait : ITrait
    {
        public Attack Response { get; set; }

        public States StateRng { get; set; }

        public int RngP1 { get; set; }
        public int RngP2 { get; set; }
        public int Rng1 { get; set; }
        public int Rng2 { get; set; }
        public int WeightRng { get; set; }

        public double Points { get; set; }

        /// <summary>
        /// Generate all the random values used to determine if the trait is activated or not
        /// </summary>
        /// <param name="rng">Random number generator</param>
        public AttackTrait(Random rng)
        {
            //Determines what values should be compared and whenever to use <= or => during comparision
            Rng1 = rng.Next(1, 4);
            Rng2 = rng.Next(1, 3);

            //Determines if to use player one/two X position or Y position
            RngP1 = rng.Next(0, 2);
            RngP2 = rng.Next(2, 4);

            //Random value that is added to player two's X or Y value
            WeightRng = rng.Next(-500, 100);

            //Randomly generated state used to compare with player two's current state
            StateRng = ValueGet.GetRandomState(rng);

            //This randomly generated action will be performed in game if trait is activated
            Response = (Attack) rng.Next(1, 10);

            Points = 0;
        }

        /// <summary>
        /// Determines whenever trait is active
        /// </summary>
        /// <param name="vals">Game information</param>
        /// <param name="state">The current state the player two is</param>
        public bool IfActivated(double[] vals, States state)
        {
            switch (Rng1)
            {
                case 1 when Rng2 == 1:
                    //If player (x pos or y pos) is less than opponent (x pos or y pos) plus weight.
                    return vals[RngP1] <= vals[RngP2] + WeightRng;

                case 1:
                    //If player (x pos or y pos) is greater than opponent (x pos or y pos) plus weight.
                    return vals[RngP1] >= vals[RngP2] + WeightRng;

                case 2:
                    //If the state of player two is equal to the randomly chosen state
                    return state == StateRng;

                case 3 when Rng2 == 1:
                    //If player (x pos or y pos) is greater than opponent (x pos or y pos) plus weight AND the state of player two is equal to the randomly chosen state
                    return vals[RngP1] >= vals[RngP2] + WeightRng && state == StateRng;

                case 3:
                    //If player (x pos or y pos) is less than opponent (x pos or y pos) plus weight AND the state of player two is equal to the randomly chosen state
                    return vals[RngP1] <= vals[RngP2] + WeightRng && state == StateRng;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Write the action assigned to the trait to Bizhawk
        /// </summary>
        /// <param name="writer">writing to Bizhawk</param>
        public void Operate(StreamWriter writer)
        {
            int result = (int) Response + 5;

            writer.WriteLine("c" + result.ToString() + "\n");
            writer.Flush();
        }
    }

    /// <summary>
    /// Actions that involve attacking
    /// </summary>
    public enum Attack
    {
        None,
        RightAttack,
        LeftAttack,
        UpAttack,
        DownAttack,
        DownSmash,
        UpSmash,
        LeftSmash,
        RightSmash,
        UpSpeical,
        DownSpeical,
        NetraulSpecial,
        Shield
    }
}