using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTS
{
    #region Enums
    public enum Operation
    {
        Addition = 0b000,
        Substraction = 0b001,
        Multiplication = 0b010,
        Division = 0b011,
        Action1 = 0b100,
        Action2 = 0b101,
        Action3 = 0b110,
        Action4 = 0b111
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
        PamietajCholeroNieDzielPrzezZero = 0b01, //XDD
        OutOfRange = 0b10,
        NotDefined = 0b11
    }

    #endregion

    public class Packet
    {
        BitArray bitArr;
        int index = 0;

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

        //wytyczne
        //
        //połączeniowy,
        //wszystkie dane przesyłane w postaci binarnej,
        //pole operacji o długości 3 bitów,
        //pola liczb o długości 32 bitów,
        //pole statusu o długości 2 bitów,
        //pole identyfikatora o długości 8 bitów,
        //dodatkowe pola zdefiniowane przez programistę

        public void Serialize()
        {
            Add(operation);
            Add(number1);
            Add(number2);
            Add(state);
            Add(Convert.ToByte(sessionId));
            Add(mode); //na takiej zasadzie + trzeba dodać logikę 
        }

        #region Deserialization

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
        public void GetInt() //boo oznacza czy jest to number1 (false) czy number2 (true)
        {
            BitArray temp = new BitArray(32); //wycina kawałek oryginalnej tablicy z liczbą, potrzebne do konwersji
            for (int i = 0; i < 32; i++)
            {
                temp[0] = bitArr[index];
                index++;
            }
            int[] arr = new int[1];
            temp.CopyTo(arr, 0);

            if (index==35) //jeżeli indeks jest równy 3+32=35 po kopiowaniu tablicy, to zapisujemy do number1
            {
                number1 = arr[0];
            }
            else if(index == 67) //jeżeli indeks jest równy 3+32+32=67 po kopiowaniu tablicy, to zapisujemy do number1
            {
                number2 = arr[0];
            }
            
        }

        #endregion

        #region Add methods

        private void Add(bool value)
        {
            bitArr.Set(index, value);
            index++;
        }

        private void Add(byte value)
        {
            for (int i = 0; i < 8; i++)
            {
                this.Add((value & (1 << i)) != 0);
            }
        }

        private void Add(Operation op)
        {
            foreach (byte by in BitConverter.GetBytes((int)op))
            {
                for (int i = 0; i < 3; i++)
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
                for (int i = 0; i < 2; i++)
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
                for (int i = 0; i < 2; i++)
                {
                    this.Add((by & (1 << i)) != 0);
                }
                break;
            }
        }

        private void Add(int value)
        {
            foreach (byte by in BitConverter.GetBytes(value))
            {
                Add(by);
            }
        }

        #endregion

        public byte[] GetBytes()
        {
            this.Serialize();
            byte[] ret = new byte[(bitArr.Length - 1) / 8 + 1];
            bitArr.CopyTo(ret, 0);
            return ret;
        }
    }
}
