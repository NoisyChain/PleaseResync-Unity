using System.IO;

namespace PleaseResync
{
    //Change this if you wanna use a different input storing method
    public struct PlayerInput
    {
        public ushort rawInput;

        public PlayerInput(ushort input) { rawInput = input; }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(rawInput);
        }

        public void Deserialize(BinaryReader br)
        {
            rawInput = br.ReadUInt16();
        }

        public override string ToString()
        {
            return rawInput.ToString();
        }
    }
}