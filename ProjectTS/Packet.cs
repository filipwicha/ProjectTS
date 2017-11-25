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
    public class Packet
    {
        BitArray bitArr;
        int index = 0;
        int size;

        BitArray operation = new BitArray(3); //dla dwuargumentowych dodatkowe operacje (x -pierwszy argument, y -drugi argument):
        int number1;                          //4: x^y; 5: pierwiasek stopnia y z x; 6: log o podstawie y z x; 7 czy x równa się y.
        int number2;                          //dla wieloargumentowych tylko operacje +-*/
        BitArray status = new BitArray(2);
        BitArray id = new BitArray(8);
        BitArray state = new BitArray(2); //odpowiada za informowanie serwera czy jest to pakiet           \
                                          //z operacją dwuargumentową lub wieloargumentową o wartościach:  |
                                          //"00"-operacja dwuargumentowa,                                  |-czekam na odpowiedz Marka
                                          //"01"-operacja wieloargumentowa, ale nie ostatni pakiet,        | czy tak może być to zrobione
                                          //"10"-operacja wieloargumentowa, ostatni pakiet.                / 
        public void Serialize()
        {
            //serializowanie pola operation
            foreach (bool b in operation)
            {
                bitArr.Set(index, b);
                index++;
            }

            //serializowanie pola number1
            this.Add(number1);

            //serializowanie pola number2
            {
                this.Add(number2);
            }

            //serializowanie pola status
            foreach (bool b in status)
            {
                bitArr.Set(index, b);
                index++;
            }

            //serializowanie pola id
            foreach (bool b in id)
            {
                bitArr.Set(index, b);
                index++;
            }

            //serializowanie pola state
            foreach (bool b in state)
            {
                bitArr.Set(index, b);
                index++;
            }
        }

        public void Deserialize()
        {
            index = 0;
            //deserializowanie pola operation
            for (int i = 0; i < operation.Length; i++)
            {
                operation[i] = bitArr[index];
                index++;
            }

            //deserializowanie pola number1
            GetInt();

            //deserializowanie pola number2
            GetInt();

            //deserializowanie pola status
            for (int i = 0; i < status.Length; i++)
            {
                status[i] = bitArr[index];
                index++;
            }

            //deserializowanie pola id
            for (int i = 0; i < id.Length; i++)
            {
                id[i] = bitArr[index];
                index++;
            }

            //deserializowanie pola state
            for (int i = 0; i < state.Length; i++)
            {
                state[i] = bitArr[index];
                index++;
            }
        }

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
        // koniec funkcji do deresializacji


        // funkcje do serializacji
        public void SetOperation(int num) //ustawianie operacji (3bit)
        {
            switch (num)
            {
                case 0: //dodawanie
                    operation.Set(0, false);
                    operation.Set(1, false);
                    operation.Set(2, false);
                    break;

                case 1: //odejmowanie
                    operation.Set(0, false);
                    operation.Set(1, false);
                    operation.Set(2, true);
                    break;

                case 2: //mnożenie
                    operation.Set(0, false);
                    operation.Set(1, true);
                    operation.Set(2, false);
                    break;

                case 3: //dzielenie
                    operation.Set(0, false);
                    operation.Set(1, true);
                    operation.Set(2, true);
                    break;

                case 4: //potega
                    operation.Set(0, true);
                    operation.Set(1, false);
                    operation.Set(2, false);
                    break;

                case 5: //pierwiastek
                    operation.Set(0, true);
                    operation.Set(1, false);
                    operation.Set(2, true);
                    break;

                case 6: //logarytm
                    operation.Set(0, true);
                    operation.Set(1, true);
                    operation.Set(2, false);
                    break;

                case 7: //czy równa
                    operation.Set(0, true);
                    operation.Set(1, true);
                    operation.Set(2, true);
                    break;
            }
        }

        public void SetState(int num)
        {
            switch (num)
            {
                case 0: //operacja 2 argumentowa
                    state.Set(0, false);
                    state.Set(1, false);
                    break;

                case 1: //operacja wieloargumentowa, ale nie ostatni pakiet
                    state.Set(0, false);
                    state.Set(1, true);
                    break;

                case 2: //operacja wieloargumentowa, ostatni pakiet
                    state.Set(0, true);
                    state.Set(1, false);
                    break;

                case 3: //nie zdefiniowane
                    state.Set(0, true);
                    state.Set(1, true);
                    break;
            }
        } //ustawianie state (czy operacja dwu czy wieloargumentowa) (2bit)

        public void SetStatus(int num)
        {
            switch (num)
            {
                case 0: //dzielenie przez 0
                    status.Set(0, false);
                    status.Set(1, false);
                    break;

                case 1: //overrange
                    status.Set(0, false);
                    status.Set(1, true);
                    break;

                case 2: //status 3 niezdefiniowane
                    status.Set(0, true);
                    status.Set(1, false);
                    break;

                case 3: //status 3 niezdefiniowane
                    status.Set(0, true);
                    status.Set(1, true);
                    break;
            }
        } //ustawianie statusu (2bit)

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

        public void Add(int value)
        {
            foreach (byte by in BitConverter.GetBytes(value))
            {
                Add(by);
            }
        }
        // koniec funkcji do serializacji



        public byte[] GetBytes()
        {
            byte[] ret = new byte[(bitArr.Length - 1) / 8 + 1];
            bitArr.CopyTo(ret, 0);
            return ret;
        }
    }
}
