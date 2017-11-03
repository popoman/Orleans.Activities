using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Xml;

namespace Orleans.Activities.Helpers
{
    public static class NetDataContractSerializerHelper
    {
        public static byte[] Serialize(object input)
        {
            using (MemoryStream memStream = new MemoryStream())
            using (GZipStream zipStream = new GZipStream(memStream, CompressionMode.Compress))
            {
                using (XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateBinaryWriter(zipStream))
                {
                    NetDataContractSerializer serializer = new NetDataContractSerializer();
                    serializer.WriteObject(xmlDictionaryWriter, input);
                    xmlDictionaryWriter.Close();
                }
                zipStream.Close();
                return memStream.ToArray();
            }
        }

        public static object Deserialize(byte[] array)
        {
            using (MemoryStream memStream = new MemoryStream(array))
            using (GZipStream zipStream = new GZipStream(memStream, CompressionMode.Decompress))
            using (XmlDictionaryReader xmlDictionaryReader = XmlDictionaryReader.CreateBinaryReader(zipStream, XmlDictionaryReaderQuotas.Max))
            {
                NetDataContractSerializer serializer = new NetDataContractSerializer();
                return serializer.ReadObject(xmlDictionaryReader);
            }
        }

    }
}
