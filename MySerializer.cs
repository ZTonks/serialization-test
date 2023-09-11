using Azure.Core.Serialization;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace serialization_test
{
    public class MySerializer : JsonObjectSerializer
    {
        /// <summary>
        /// This method overrides Azure's JSON deserialization.
        /// We believe there is a bug with in process Azure functions where
        /// e.g. events sent to orchestrations are serialized twice.
        /// This override attempts to deserialize twice, with a failover to
        /// deserialize once in situations where we are deserializing a single
        /// encoded object.
        /// </summary>
        /// <param name="stream">
        /// The stream to read for deserialization.
        /// </param>
        /// <param name="returnType">
        /// The type to deserialize to.
        /// </param>
        /// <param name="cancellationToken">
        /// Token to cancel the operation.
        /// </param>
        /// <returns></returns>
        public override object Deserialize(
            Stream stream,
            Type returnType,
            CancellationToken cancellationToken)
        {
            using var sr = new StreamReader(stream, leaveOpen: true);

            try
            {
                var str = sr.ReadToEnd();
                var bytes = Encoding.UTF8.GetBytes(str);
                var newStr = Encoding.UTF8.GetString(bytes);
                var newestStr = JsonSerializer.Deserialize<string>(newStr);
                stream.Seek(0, SeekOrigin.Begin);
                return JsonSerializer.Deserialize(newestStr, returnType);
            }
            catch
            {
                stream.Seek(0, SeekOrigin.Begin);
                return JsonSerializer.Deserialize(stream, returnType);
            }
        }
    }
}
