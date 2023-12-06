using System.IO;
namespace PleaseResync.Unity
{
    public interface IGameState
    {
        /*PlayerInputs controls;

        uint frame;
        uint sum;

        public virtual void Update(byte[] playerInput)
        {
            frame++;

            GameLoop(playerInput);

            foreach (var num in playerInput)
            {
                sum += num;
            }
        }*/

        void GameLoop(byte[] playerInput) {}

        void Serialize(BinaryWriter bw) {}

        void Deserialize(BinaryReader br) {}

        byte GetLocalInput(int PlayerID) { return 0; }
    }
}
