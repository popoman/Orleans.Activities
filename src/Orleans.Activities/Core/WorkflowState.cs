using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Activities;
using System.Runtime.DurableInstancing;
using System.Xml.Linq;
using Orleans.CodeGeneration;
using Orleans.Serialization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Runtime.Serialization;

namespace Orleans.Activities
{
    /// <summary>
    /// Base class for WorkflowGrain states.
    /// <para>Usage is not mandatory, <see cref="WorkflowGrain{TGrain, TGrainState, TWorkflowInterface, TWorkflowCallbackInterface}"/> requires only to implement <see cref="IWorkflowState"/> interface.</para>
    /// </summary>
    public sealed class WorkflowState : IWorkflowState
    {
        public IDictionary<XName, InstanceValue> InstanceValues { get; set; }
        public WorkflowIdentity WorkflowDefinitionIdentity { get; set; }


        [SerializerMethod]
        static private void Serialize(object input, ISerializationContext context, Type expected)
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
                var array = memStream.ToArray();
                context.StreamWriter.Write(array.Length);
                context.StreamWriter.Write(array);
            }
        }

        [DeserializerMethod]
        static private object Deserialize(Type expected, IDeserializationContext context)
        {
            var len = context.StreamReader.ReadInt();

            using (MemoryStream memStream = new MemoryStream(context.StreamReader.ReadBytes(len)))
            using (GZipStream zipStream = new GZipStream(memStream, CompressionMode.Decompress))
            using (XmlDictionaryReader xmlDictionaryReader = XmlDictionaryReader.CreateBinaryReader(zipStream, XmlDictionaryReaderQuotas.Max))
            {
                NetDataContractSerializer serializer = new NetDataContractSerializer();
                return serializer.ReadObject(xmlDictionaryReader);
            }
        }

        [CopierMethod]
        static private object Copy(object input, ICopyContext context)
        {
            return input;
        }
    }
}
