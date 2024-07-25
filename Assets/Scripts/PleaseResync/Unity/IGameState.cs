using System.IO;
namespace PleaseResync
{
    public interface IGameState
    {
        void Setup();
        void GameLoop(PlayerInput[] playerInput);
        void Serialize(BinaryWriter bw);
        void Deserialize(BinaryReader br);
        PlayerInput GetLocalInput(int PlayerID);
        //int StateFrame();
        //uint StateChecksum();
    }
}
