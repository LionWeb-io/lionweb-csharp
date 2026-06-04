namespace LionWeb.Test.Run;

using Core.Serialization;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;

class Program
{
    private Dictionary<string, object>? _serializedNodes;

    static void Main(string[] args)
    {
        var program = new Program();
        program.SerializedNodes();

        Console.WriteLine(program._serializedNodes?.Count);
    }

    public void SerializedNodes()
    {
        _serializedNodes = new SerializedNodeTest().CreateSerializedNodes();
        foreach (var (key, value) in _serializedNodes)
        {
            Console.WriteLine($"{key}: {SizeOf(value)}");
        }

        for (int i = 0; i < 400; i++)
        {
            _serializedNodes[i.ToString()] = new SerializedNode()
            {
                Id = i.ToString(),
                Classifier = TestLanguageLanguage.Instance.DataTypeTestConcept.ToMetaPointer(),
                Properties = [],
                Containments = [],
                References = [],
                Annotations = [],
                Parent = null
            };
        }
        
        Thread.Sleep(5000);
    }

    private static int SizeOf(object obj)
    {
        unsafe
        {
            RuntimeTypeHandle th = obj.GetType().TypeHandle;
            int size = *(*(int**)&th + 1);
            return size;
        }
    }
}