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
    public enum Operation
    {
        addition = 0b000,
        substraction = 0b001,
        multiplication = 0b010,
        division = 0b011,
        action1 = 0b100,
        action2 = 0b101,
        action3 = 0b110,
        action4 = 0b111
    }

    public enum operacja
    {
        addition = 0b00,
        substraction = 0b01,
        multiplication = 0b10,
        division = 0b011
    }

    public class Packet
    {
        BitArray bitArr;
        int index = 0;
        int size;

        public Packet(int size)
        {
            this.size = size;
            bitArr = new BitArray(size);
        }

        public void Add(bool value)
        {
            bitArr.Set(index, value);
            index++;
        }

        public void Add(byte value)
        {
            for (int i = 0; i < 8; i++)
            {
                this.Add((value & (1 << i)) != 0);
            }
        }

        public void Add(Operation op)
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

        public void Add(int value)
        {
            foreach (byte by in BitConverter.GetBytes(value))
            {
                Add(by);
            }
        }
        public byte[] GetBytes()
        {
            byte[] ret = new byte[(bitArr.Length - 1) / 8 + 1];
            bitArr.CopyTo(ret, 0);
            return ret;
        }
    }
}
