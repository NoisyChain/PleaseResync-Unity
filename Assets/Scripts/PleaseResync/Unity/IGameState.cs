using System.IO;
namespace PleaseResync
{
    public interface IGameState
    {
        /// <summary>
        /// Setup()
        /// Called when the game is started to initialize the simulation.
        /// </summary>
        void Setup();

        /// <summary>
        /// GameLoop()
        /// It's the simulation loop. Called every simulation tick.
        /// </summary>
        /// <param name="playerInput">The overall inputs from the connected players</param>
        void GameLoop(byte[] playerInput);
        
        /// <summary>
        /// SaveState()
        /// Called when the game is simulated. It saves a snapshot of the current tick.
        /// </summary>
        /// <param name="bw">Serialization</param>
        void SaveState(BinaryWriter bw);
        
        /// <summary>
        /// LoadState()
        /// Called only during rollbacks. It gets the saved snapshot of the selected tick to resimulate it
        /// </summary>
        /// <param name="br">Deserialization</param>
        void LoadState(BinaryReader br);
        
        /// <summary>
        /// GetLocalInput()
        /// Use this to process how the game should read inputs
        /// </summary>
        /// <param name="PlayerID">The selected player's ID</param>
        /// <returns>A byte array to use in the simulation by the specified player</returns>
        byte[] GetLocalInput(int PlayerID);
    }
}
