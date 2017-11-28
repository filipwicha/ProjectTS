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
        Sqrt = 0b100,
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

//>>>>>>> ceb62c61b28e6ce024e217973884b8290d21b82d
        Operation operation;
        int number1 = 0;
        int number2 = 0;
        State state = State.Nothing;
        int sessionId;
        Mode mode;
        

        public Packet()
        {
            bitArr = new BitArray(80);
        }

        public Packet(byte[] buffer)
        {
            bitArr = new BitArray(buffer);
            Deserialize();
        }

        /*
        public Packet(int size, Operation operation, int number1, int number2, State state, int sessionId, Mode mode)
        {
            this.size = size;
            this.operation = operation;
            this.number1 = number1;
            this.number2 = number2;
            this.state = state;
            this.sessionId = sessionId;
            this.mode = mode;
        }
        */

        //wytyczne
        //
        //połączeniowy,
        //wszystkie dane przesyłane w postaci binarnej,
        //pole operacji o długości 3 bitów,
        //pola liczb o długości 32 bitów,
        //pole statusu o długości 2 bitów,
        //pole identyfikatora o długości 8 bitów,
        //dodatkowe pola zdefiniowane przez programistę

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

//<<<<<<< HEAD
        #region Deserialization

        public void Deserialize()
        {

        }




        //public void Deserialize()
        //{
        //    index = 0;
        //    //deserializowanie pola operation
        //    for (int i = 0; i < operation.Length; i++)
        //    {
        //        operation[i] = bitArr[index];
        //        index++;
        //    }

        //    //deserializowanie pola number1
        //    GetInt();

        //    //deserializowanie pola number2
        //    GetInt();

        //    //deserializowanie pola status
        //    for (int i = 0; i < status.Length; i++)
        //    {
        //        status[i] = bitArr[index];
        //        index++;
        //    }

        //    //deserializowanie pola id
        //    for (int i = 0; i < id.Length; i++)
        //    {
        //        id[i] = bitArr[index];
        //        index++;
        //    }

        //    //deserializowanie pola state
        //    for (int i = 0; i < state.Length; i++)
        //    {
        //        state[i] = bitArr[index];
        //        index++;
        //    }
        //}

        // funkcje do deserializacji
        public void GetInt(); //boo oznacza czy jest to number1 (false) czy number2 (true)
//=======
        private int GetInt(int length)
//>>>>>>> ceb62c61b28e6ce024e217973884b8290d21b82d
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
            Add(Operation.Substraction);
            Add(Int32.MaxValue);
            Add(100);
            Add(State.OutOfRange);
            Add(Convert.ToByte(5));
            Add(Mode.TwoArguments); //na takiej zasadzie + trzeba dodać logikę 
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
