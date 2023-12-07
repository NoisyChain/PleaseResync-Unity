using System.IO;
namespace PleaseResync.Unity
{
    public interface IGameState
    {
        void GameLoop(byte[] playerInput);
        void Serialize(BinaryWriter bw);
        void Deserialize(BinaryReader br);
        byte GetLocalInput(int PlayerID);
        string GetStateFrame();
    }
}
