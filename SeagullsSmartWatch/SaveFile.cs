using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Windows;

namespace SeagullsSmartWatch
{
    public enum FileDataType
    {
        INT,
        BOOL,
        CHAR,
        FLOAT,
        STRING,
        BYTE,
    }

    public class SaveFile
    {
        string path = "";
        Hashtable data = new Hashtable();

        public SaveFile(string _path)
        {
            path = _path;
        }


        protected BinaryReader LoadSaveFileProcess()
        {
            BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open));
            return br;
        }

        public bool OpenSaveFile()
        {
            FileInfo fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
                return false;

            BinaryReader br = LoadSaveFileProcess();

            if (br == null)
                return false;

            data.Clear();

            try
            {
                while (br.BaseStream.Length != br.BaseStream.Position)
                {
                    int keyLength = br.ReadInt32();
                    int dataLength = br.ReadInt32();

                    byte type = br.ReadByte();

                    byte[] key = br.ReadBytes(keyLength);
                    byte[] value = br.ReadBytes(dataLength);

                    object keyString = Encoding.UTF8.GetString(key);
                    object dataValue = GetConvertedDataByDataType(type, value);

                    data.Add(keyString, dataValue);
                }
            }
            catch
            {
                br.Dispose();
                return false;
            }

            br.Dispose();
            return true;
        }

        protected BinaryWriter SaveSaveFileProcess()
        {
            BinaryWriter bw = null;

            int dirIdx = path.LastIndexOf("\\");
            if (dirIdx != -1)
            {
                string dirPath = path.Remove(dirIdx);
                DirectoryInfo directory = new DirectoryInfo(dirPath);
                if (directory.Exists == false)
                {
                    directory.Create();
                }
            }

            bw = new BinaryWriter(new FileStream(path, FileMode.Create));
            return bw;
        }

        public bool SaveThisFile()
        {
            BinaryWriter bw = SaveSaveFileProcess();

            foreach (object key in data.Keys)
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key.ToString());
                byte type = GetDataTypeOfByte(data[key].GetType());
                byte[] dataBytes = GetBytesByDataType(type, data[key]);//Encoding.UTF8.GetBytes(m_gameProgressData[key].ToString());

                bw.Write(keyBytes.Length);
                bw.Write(dataBytes.Length);
                bw.Write(type);
                bw.Write(keyBytes);
                bw.Write(dataBytes);

                //bw.Write(",");
                //bw.Write("//");
            }

            bw.Dispose();
            return true;
        }

        public void SetData(object key, object value)
        {
            if (data.ContainsKey(key))
                data[key] = value;
            else
                data.Add(key, value);
        }

        public object GetData(object key, object defaultValue)
        {
            try
            {
                if (data.ContainsKey(key))
                    return data[key];
            }
            catch (Exception e)
            {
                return defaultValue;
            }

            return defaultValue;
        }

        static byte GetDataTypeOfByte(Type type)
        {
            if (type == typeof(bool))
                return (byte)FileDataType.BOOL;
            else if (type == typeof(char))
                return (byte)FileDataType.CHAR;
            else if (type == typeof(float))
                return (byte)FileDataType.FLOAT;
            else if (type == typeof(string))
                return (byte)FileDataType.STRING;
            else if (type == typeof(byte))
                return (byte)FileDataType.BYTE;
            else
                return (byte)FileDataType.INT;
        }
        static object GetConvertedDataByDataType(byte byteType, byte[] dataBytes)
        {
            //string data = Encoding.UTF8.GetString(dataBytes);

            if (byteType == (byte)FileDataType.BOOL)
                return BitConverter.ToBoolean(dataBytes, 0);
            else if (byteType == (byte)FileDataType.CHAR)
                return BitConverter.ToChar(dataBytes, 0);
            else if (byteType == (byte)FileDataType.FLOAT)
                return BitConverter.ToSingle(dataBytes, 0);
            else if (byteType == (byte)FileDataType.STRING)
                return Encoding.UTF8.GetString(dataBytes);
            else if (byteType == (byte)FileDataType.BYTE)
                return dataBytes[0];
            else
                return BitConverter.ToInt32(dataBytes, 0);
        }

        static byte[] GetBytesByDataType(byte byteType, object data)
        {
            if (byteType == (byte)FileDataType.BOOL)
                return BitConverter.GetBytes((bool)data);
            else if (byteType == (byte)FileDataType.CHAR)
                return BitConverter.GetBytes((char)data);
            else if (byteType == (byte)FileDataType.FLOAT)
                return BitConverter.GetBytes((float)data);
            else if (byteType == (byte)FileDataType.STRING)
                return Encoding.UTF8.GetBytes((string)data);
            else if (byteType == (byte)FileDataType.BYTE)
                return BitConverter.GetBytes((byte)data);
            else
                return BitConverter.GetBytes((int)data);
        }
    }
}
