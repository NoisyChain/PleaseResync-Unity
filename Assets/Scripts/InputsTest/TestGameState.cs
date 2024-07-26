using System.Collections;
using System.Collections.Generic;
using PleaseResync;
using FixMath.NET;
using System.IO;

public class TestGameState : IGameState
{
    public PlayerInputs controls;

    public int frame;

    public string InputsDebug;

    public TestPlayer[] players;

    private Fix64 START_POSITION = (Fix64)2;

    public FixVector2[] positions = new[] 
    {
        new FixVector2((Fix64)(-2), Fix64.Zero),
        new FixVector2((Fix64)2, Fix64.Zero),
        new FixVector2(Fix64.Zero, (Fix64)(-2)),
        new FixVector2(Fix64.Zero, (Fix64)2)
    };
    
    public TestGameState(uint playerCount, PlayerInputs inputs)
    {
        controls = inputs;
        players = new TestPlayer[playerCount];
        this.frame = -1;
        for (int i = 0; i < players.Length; ++i)
        {
            this.players[i] = new TestPlayer(positions[i]);
        }
    }

    public TestGameState(BinaryReader br)
    {
        Deserialize(br);
    }

    public void Setup() {}

    public void GameLoop(byte[] playerInput)
    {
        frame++;
        for (int i = 0; i < players.Length; ++i)
        {
            int h, v;
            ParseInputs(playerInput[i], out h, out v);
            players[i].Move(h, v);
        }
    }

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(frame);
        for (int i = 0; i < players.Length; ++i)
            players[i].Serialize(bw);
    }

    public void Deserialize(BinaryReader br)
    {
        frame = br.ReadInt32();
        for (int i = 0; i < players.Length; ++i)
            players[i].Deserialize(br);
    }

    public byte[] GetLocalInput(int PlayerID)
    {
        return ReadInputs(PlayerID);
    }

    public byte[] ReadInputs(int id) 
    {
        byte[] input = new byte[1];

        if (id == 0) {
            if (controls.Player1.Vertical.ReadValue<float>() > 0) {
                input[0] |= TestInputs.INPUT_UP;
            }
            if (controls.Player1.Vertical.ReadValue<float>() < 0) {
                input[0] |= TestInputs.INPUT_DOWN;
            }
            if (controls.Player1.Horizontal.ReadValue<float>() < 0) {
                input[0] |= TestInputs.INPUT_LEFT;
            }
            if (controls.Player1.Horizontal.ReadValue<float>() > 0) {
                input[0] |= TestInputs.INPUT_RIGHT;
            }
            /*if (UnityEngine.Input.GetButtonDown("Fire1" + (id + 1))) {
                input[0] |= INPUT_PUNCH;
            }
            if (UnityEngine.Input.GetButtonDown("Fire2" + (id + 1))) {
                input[0] |= INPUT_KICK;
            }
            if (UnityEngine.Input.GetButtonDown("Fire3" + (id + 1))) {
                input[0] |= INPUT_SPECIAL;
            }*/
        }
        else if (id == 1) {
            //input = 0;
            if (controls.Player2.Vertical.ReadValue<float>() > 0) {
                input[0] |= TestInputs.INPUT_UP;
            }
            if (controls.Player2.Vertical.ReadValue<float>() < 0) {
                input[0] |= TestInputs.INPUT_DOWN;
            }
            if (controls.Player2.Horizontal.ReadValue<float>() < 0) {
                input[0] |= TestInputs.INPUT_LEFT;
            }
            if (controls.Player2.Horizontal.ReadValue<float>() > 0) {
                input[0] |= TestInputs.INPUT_RIGHT;
            }
            /*if (UnityEngine.Input.GetButtonDown("Fire1" + (id + 1))) {
                input[0] |= INPUT_PUNCH;
            }
            if (UnityEngine.Input.GetButtonDown("Fire2" + (id + 1))) {
                input[0] |= INPUT_KICK;
            }
            if (UnityEngine.Input.GetButtonDown("Fire3" + (id + 1))) {
                input[0] |= INPUT_SPECIAL;
            }*/
        }

        return input;
    }

    public void ParseInputs(ushort input, out int h, out int v)
    {
        if ((input & TestInputs.INPUT_RIGHT) != 0)
            h = 1;
        else if ((input & TestInputs.INPUT_LEFT) != 0)
            h = -1;
        else
            h = 0;

        if ((input & TestInputs.INPUT_DOWN) != 0)
            v = -1;
        else if ((input & TestInputs.INPUT_UP) != 0)
            v = 1;
        else
            v = 0;
    }
}

public class TestPlayer
{
    public FixVector2 position;
    public FixVector2 velocity;

    public Fix64 speed = (Fix64)5;

    public TestPlayer()
    {
        position = FixVector2.Zero;
        velocity = FixVector2.Zero;
    }

    public TestPlayer(TestPlayer p)
    {
        position = p.position;
        velocity = p.velocity;
    }

    public TestPlayer(FixVector2 initialPosition)
    {
        position = initialPosition;
        velocity = FixVector2.Zero;
    }

    public void Move(int horizontal, int vertical)
    {
        velocity.x = (Fix64)horizontal * (Fix64)speed;
        velocity.y = (Fix64)vertical * (Fix64)speed;

        position.x += velocity.x / (Fix64)60;
        position.y += velocity.y / (Fix64)60;

        position.x = Fix64.Clamp(position.x, (Fix64)(-9.5f), (Fix64)9.5f);
        position.y = Fix64.Clamp(position.y, (Fix64)(-2.5f), (Fix64)6f);
    }

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(velocity.x.RawValue);
        bw.Write(velocity.y.RawValue);
        bw.Write(position.x.RawValue);
        bw.Write(position.y.RawValue);
    }

    public void Deserialize(BinaryReader br)
    {
        velocity.x = Fix64.FromRaw(br.ReadInt64());
        velocity.y = Fix64.FromRaw(br.ReadInt64());
        position.x = Fix64.FromRaw(br.ReadInt64());
        position.y = Fix64.FromRaw(br.ReadInt64());
    }

    public override int GetHashCode()
    {
        int hashCode = -1214587014;
        hashCode = hashCode * -1521134295 + velocity.x.GetHashCode();
        hashCode = hashCode * -1521134295 + velocity.y.GetHashCode();
        hashCode = hashCode * -1521134295 + position.x.GetHashCode();
        hashCode = hashCode * -1521134295 + position.y.GetHashCode();
        return hashCode;
        //return System.HashCode.Combine(frame, sum);
    }
}

public static class TestInputs
{
    public const int INPUT_UP = 1 << 0;
    public const int INPUT_DOWN = 1 << 1;
    public const int INPUT_LEFT = 1 << 2;
    public const int INPUT_RIGHT = 1 << 3;
}
