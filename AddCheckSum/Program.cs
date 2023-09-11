using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddCheckSum
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                String FileName = args[0];
                String Signature = args[1];
                byte[] ByteFile;
                byte[] HashSignature;
                byte[] HashFile;
                byte[] CombineFile;

                int Length;

                Thread.Sleep(2000);

                if (Signature.Length != 32)
                {
                    Console.WriteLine("AddCheckSum : Error the key property does not exist .");
                    return;
                }
                else
                {
                    HashSignature = ConvertHexStringToByteArray(Signature);
                }

                if (FileName.Length == 0)
                {
                    Console.WriteLine("AddCheckSum : Error input file .");
                    return;
                }

                FileStream BinFile = new FileStream(FileName, FileMode.Open);                
                if (BinFile.Length % 16 != 0)
                {
                    Console.WriteLine("AddCheckSum : Error Length /16 .");
                    return;
                }
                Length = (int)BinFile.Length;

                ByteFile = new byte[Length];
                BinFile.Read(ByteFile, 0, Length);
                MD5 md5 = MD5.Create();
                HashSignature = md5.ComputeHash(HashSignature);
                CombineFile = Combine(ConvertHexStringToByteArray(Signature), ByteFile);
                HashFile = md5.ComputeHash(CombineFile);
                BinFile.Close();

                File.Delete(FileName);

                CombineFile = Combine(HashSignature, HashFile, ByteFile);
                BinFile = new FileStream(FileName, FileMode.Create);                
                BinFile.Write(CombineFile, 0, Length+32);
                BinFile.Flush();
                BinFile.Close();

                Thread.Sleep(2000);

                Console.WriteLine("AddCheckSum : Success .");
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }

        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        public static byte[] Combine(byte[] first, byte[] second, byte[] third)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,
                             third.Length);
            return ret;
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }
}
