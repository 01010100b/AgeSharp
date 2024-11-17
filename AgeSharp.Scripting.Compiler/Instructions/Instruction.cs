using System.Text.Encodings.Web;
using System.Text.Json;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal abstract class Instruction
    {
        public override string ToString()
        {
            var json = JsonSerializer.Serialize(this, GetType(), new JsonSerializerOptions() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            return $"{GetType().Name}: {json}";
        }
    }
}
