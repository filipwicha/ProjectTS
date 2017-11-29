using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//wytyczne
//
//połączeniowy,
//wszystkie dane przesyłane w postaci binarnej,
//pole operacji o długości 3 bitów,
//pola liczb o długości 32 bitów,
//pole statusu o długości 2 bitów,
//pole identyfikatora o długości 8 bitów,
//dodatkowe pola zdefiniowane przez programistę

namespace ProjectTS
{
    #region Enums
    public enum Operation
    {
        Addition = 0b000,
        Substraction = 0b001,
        Multiplication = 0b010,
        Division = 0b011,
        LinearFunction = 0b100,
        Log = 0b101,
        Average = 0b110,
        Equals = 0b111
    }

    public enum Mode
    {
        TwoArguments = 0b00,
        MultiArguments = 0b01,
        MultiArgumentsLP = 0b10,
        NotDefined = 0b011
    }

    public enum State
    {
        Nothing = 0b00,
        DivisionByZero = 0b01, 
        OutOfRange = 0b10,
        NotDefined = 0b11
    }

    #endregion

    public class Packet
    {
        BitArray bitArr;

        public Operation operation;
        public int number1 = 0;
        public int number2 = 0;
        public State state = State.Nothing;
        public int sessionId;
        public Mode mode;

        public Packet()
        {
            bitArr = new BitArray(80);
        }

        public Packet(byte[] buffer)
        {
            bitArr = new BitArray(buffer);
            Deserialize();
        }

        #region Deserialization

        public void Deserialize()
        {
            operation = (Operation)GetInt(3);
            number1 = GetInt(32);
            number2 = GetInt(32);
            state = (State)GetInt(2);
            sessionId = GetInt(8);
            mode = (Mode)GetInt(2);
        }

        private int GetInt(int length)
        {
            var result = new int[1];
            BitArray tmp = new BitArray(32, false);
            for (int i = length-1; i >= 0 ; i--)
            {
                tmp[i] = getBit();
            }
            tmp.CopyTo(result, 0);
            return result[0];
        }

        private bool getBit()
        {
            if (index % 8 == 0)
            {
                byteIndex++;
                index = byteIndex * 8;
            }
            index--;
            return bitArr[index];
        }

        #endregion

        #region Serialization

        public void Serialize()
        {
            Add(operation);
            Add(number1);
            Add(number2);
            Add(state);
            Add(Convert.ToByte(5));
            Add(mode);
        }

        int index = 0;
        int byteIndex = 0;
        private void Add(bool value)
        {
            if (index % 8 == 0)
            {
                byteIndex++;
                index = byteIndex * 8;
            }
            index--;
            bitArr.Set(index, value);
        }

        private void Add(byte value)
        {
            for (int i = 7; i >= 0; i--)
            {
                this.Add((value & (1 << i)) != 0);
            }
        }

        private void Add(Operation op)
        {
            foreach (byte by in BitConverter.GetBytes((int)op))
            {
                for (int i = 2; i >= 0; i--)
                {
                     this.Add((by & (1 << i)) != 0);
                }
                break;
            }
        }

        private void Add(Mode mo)
        {
            foreach (byte by in BitConverter.GetBytes((int)mo))
            {
                for (int i = 1; i >= 0; i--)
                {
                    this.Add((by & (1 << i)) != 0);
                }
                break;
            }
        }

        private void Add(State st)
        {
            foreach (byte by in BitConverter.GetBytes((int)st))
            {
                for (int i = 1; i >= 0; i--)
                {
                    this.Add((by & (1 << i)) != 0);
                }
                break;
            }
        }

        private void Add(int value)
        {
            byte[] binary = BitConverter.GetBytes(value);
            foreach (byte by in binary.Reverse())
            {
                Add(by);
            }
        }

        #endregion

        public byte[] GetBytes()
        {
            this.Serialize();
            byte[] binary = new byte[(bitArr.Length - 1) / 8 + 1];
            bitArr.CopyTo(binary, 0);
            return binary;
        }
    }
}
