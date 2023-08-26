using System.IO;
namespace PleaseResync.Unity
{
    public class BaseGameState
    {
        public PlayerInputs controls;

        public uint frame;
        public uint sum;

        public virtual void Update(byte[] playerInput)
        {
            frame++;

            GameLoop(playerInput);

            foreach (var num in playerInput)
            {
                sum += num;
            }
        }

        public virtual void GameLoop(byte[] playerInput) {}

        public virtual void Serialize(BinaryWriter bw) {}

        public virtual void Deserialize(BinaryReader br) {}

        public virtual byte GetLocalInput(int PlayerID) { return 0; }

        public virtual void Load(BinaryReader br) { Deserialize(br); }
    }
}
