namespace LionWeb.Test.Run;

using Core;
using Core.Serialization;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;

class Program
{
    private const int _sleepMilliseconds = 5000;
    private Dictionary<string, object>? _serializedNodes;
    private List<IReadableNode>? _nodes;

    static void Main(string[] args)
    {
        var program = new Program();
        program.SerializedNodes();
        program.SerializeStream();

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
        
        Thread.Sleep(_sleepMilliseconds);
    }

    public void SerializeStream()
    {
        Console.WriteLine("---- Serializing Stream");
        var stream = new MemoryStream();
        var serializedNodeTest = new SerializedNodeTest();
        serializedNodeTest.SerializeNodes(stream);
        
        Thread.Sleep(_sleepMilliseconds);

        stream.Seek(0, SeekOrigin.Begin);

        Console.WriteLine("deserializing");

        _nodes = serializedNodeTest.DeserializeNodes(stream);
        
        Thread.Sleep(_sleepMilliseconds);

        Console.WriteLine($"{_nodes.Count} nodes");
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