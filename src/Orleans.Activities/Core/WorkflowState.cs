using Orleans.Activities.Helpers;
using Orleans.CodeGeneration;
using Orleans.Serialization;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Runtime.DurableInstancing;
using System.Xml.Linq;

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
            byte[] array = NetDataContractSerializerHelper.Serialize(input);

            context.StreamWriter.Write(array.Length);
            context.StreamWriter.Write(array);
        }

        [DeserializerMethod]
        static private object Deserialize(Type expected, IDeserializationContext context)
        {
            var len = context.StreamReader.ReadInt();
            return NetDataContractSerializerHelper.Deserialize(context.StreamReader.ReadBytes(len));
        }

        [CopierMethod]
        static private object Copy(object input, ICopyContext context)
        {
            return input;
        }
    }
}
