using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Sockets;

namespace Smash64Network
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Previous damage of player two
            double previousDamageTwo = 0;

            //Trait lists
            List<AttackTrait> attackTraits = new List<AttackTrait>();
            List<EvadeTrait> evadeTraits = new List<EvadeTrait>();

            //Make a new random number generator
            Random rng = new Random();

            //Generate new traits
            ValueGet.SetTraitList(ref attackTraits, ref evadeTraits, rng);

            //Start a TCP listener at port 5555
            TcpListener listener = new TcpListener(5555);
            listener.Start();

            //Look for a TCP client until one is found
            TcpClient client = listener.AcceptTcpClient();
            while (!client.Connected)
                client = listener.AcceptTcpClient();

            //Reader for reading data from the stream and writer for writing to stream.
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());

            while (true)
            {
                //All game info (Stock count, damage, etc)
                string vals = ValueGet.WriteAndRead(reader, writer, "a\n");

                //Split vals by an empty space
                string[] valSplit = vals.Split(' ');

                //Position values
                double xOne = double.Parse(valSplit[0]);
                double yOne = double.Parse(valSplit[1]);
                double xTwo = double.Parse(valSplit[2]);
                double yTwo = double.Parse(valSplit[3]);

                //Stock values
                double stockOne = double.Parse(valSplit[4]);
                double stockTwo = double.Parse(valSplit[5]);

                //Damage values
                double damageTwo = double.Parse(valSplit[6]);

                //Status values
                States twoStatus = (States) Convert.ToInt32(valSplit[7], 16);

                if (stockOne != 1 && stockTwo != 1)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        //If an attackTrait has been activated...
                        if (attackTraits[i].IfActivated(new double[] {xOne, yOne, xTwo, yTwo}, twoStatus))
                        {
                            //Perform the random action assigned to that attackTrait
                            attackTraits[i].Operate(writer);

                            //If the damage has changed, then adjust the points of the attackTrait
                            attackTraits[i].Points = damageTwo != previousDamageTwo
                                ? attackTraits[i].Points + ValueGet.CalculateAttackPoints(damageTwo, previousDamageTwo)
                                : attackTraits[i].Points;
                            
                        }

                        //If an evadeTrait has been activated...
                        if (evadeTraits[i].IfActivated(new double[] {xOne, yOne, xTwo, yTwo}, twoStatus))
                        {
                            //Perform the random action assigned to that evadeTrait
                            evadeTraits[i].Operate(writer);

                            //Adjust points of evadeTrait
                            evadeTraits[i].Points =
                                evadeTraits[i].Points + ValueGet.CalculateEvadePoints(xOne, yOne, xTwo, yTwo);
                            
                        }
                    }

                    previousDamageTwo = damageTwo;
                }
                else
                {
                    //Sort the traits by how many points they have
                    List<AttackTrait> sortedAttackTrait = attackTraits.OrderBy(a => a.Points).ToList();
                    List<EvadeTrait> sortedEvadeTrait = evadeTraits.OrderBy(a => a.Points).ToList();

                    for (int i = 0; i < 20; i++)
                        Console.WriteLine("SORTED: " + sortedAttackTrait[i].Points + " EVADE: " +
                                          sortedEvadeTrait[i].Points);

                    for (int i = 0; i < 5; i++)
                    {
                        //Remove the 5 traits with the least amount of points.
                        sortedAttackTrait.RemoveAt(i + 15);
                        sortedEvadeTrait.RemoveAt(i + 15);

                        //Add 5 new traits
                        sortedAttackTrait.Add(new AttackTrait(rng));
                        sortedEvadeTrait.Add(new EvadeTrait(rng));
                    }

                    //Replace the current traits with the newly generated ones
                    attackTraits = sortedAttackTrait;
                    evadeTraits = sortedEvadeTrait;

                    //Restart the game!
                    writer.WriteLine("b\n");
                    writer.Flush();

                    previousDamageTwo = damageTwo;
                }
            }
        }
    }
}