using System.IO;
namespace Smash64Network
{
    public interface ITrait
    {
        States StateRng { get; set;  }

        int RngP1 { get; set; }
        int RngP2 { get; set; }
        int Rng1 { get; set; }
        int Rng2{ get; set; }
        int WeightRng { get; set; }
        double Points { get; set; }

        bool IfActivated(double[] vals, States state);

        void Operate(StreamWriter writer);
    }
}