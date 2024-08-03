// using Common.Configuration;
// using Common.Helpers;
// using Common.System.Text.Json;
// using LionWeb.Core;
// using LionWeb.Core.M1;
// using LionWeb.Core.Serialization;
// using Microsoft.Extensions.Logging;
// using System.Diagnostics;
// using System.Text.Json.Stream;
//
// public class LionWebStreamingDeserializer : JsonStreamPropertyDeserializer<ILionWebConfiguration>
// {
//     private readonly Stream _stream;
//     private readonly IDeserializer _deserializer;
//
//     public LionWebStreamingDeserializer(ILionWebConfiguration configuration, Stream stream, IDeserializer deserializer, ILogger? logger = null) : base(configuration, logger)
//     {
//         _stream = stream;
//         _deserializer = deserializer;
//     }
//
//     public List<INode> Finish()
//     {
//         ProcessAsync().ConfigureAwait(false).GetAwaiter().GetResult();
//         return _deserializer.Finish().ToList();
//     }
//
//     protected override (bool isValid, string path) GetValidFileWithPath(string path, string caller = "") =>
//         (true, "bla");
//
//     protected override async Task ProcessAsync(Utf8JsonAsyncStreamReader jsonReader, CancellationToken cancellationToken)
//     {
//         switch (jsonReader.GetString())
//         {
//             case "serializationFormatVersion":
//                 await jsonReader.ReadAsync(cancellationToken).ConfigureAwait(false);
//                 string? version = jsonReader.GetString();
//                 Console.WriteLine($"version: {version}");
//                 break;
//             
//             case "nodes":
//                 if (BatchSize > 1)
//                     await DeserializeAsync<SerializedNode>
//                             (jsonReader, this.BatchProcessAsync, cancellationToken)
//                         .ConfigureAwait(false);
//                 else
//                     await DeserializeAsync<SerializedNode>
//                             (jsonReader, ItemProcessAsync, cancellationToken)
//                         .ConfigureAwait(false);
//                 break;
//         }
//     }
//     
//     private Task BatchProcessAsync(IEnumerable<SerializedNode?> items, CancellationToken token)
//     {
//         _deserializer.Process(items);
//         return Task.CompletedTask;
//     }
//     
//     private Task ItemProcessAsync(SerializedNode? item, CancellationToken token)
//     {
//         Console.WriteLine($"item: {item.Id}");
//         _deserializer.Process([item]);
//         return Task.CompletedTask;
//     }
//
//     protected override async Task ReadJsonAsync(string path, Func<Utf8JsonAsyncStreamReader, CancellationToken, Task> @delegate, CancellationToken cancellationToken = default)
//     {
//         this.Logger?.Emit(LogLevel.Information, $"Processing: {path}");
//
//         await using (_stream.ConfigureAwait(false))
//             await this.ReadJsonAsync(_stream, @delegate, cancellationToken)
//                 .ConfigureAwait(false);
//
//     }
// }
//
// public class LionWebFileStreamingDeserializer : JsonFileStreamPropertyDeserializer<ILionWebFilesConfiguration>
// {
//     private readonly IDeserializer _deserializer;
//
//     // public LionWebFileStreamingDeserializer(ILionWebFilesConfiguration configuration, IDeserializer deserializer, ILogger? logger = null) : base(configuration, logger)
//     // {
//     //     _deserializer = deserializer;
//     // }
//
//     public LionWebFileStreamingDeserializer(IFilePathHelper filePathHelper, ILionWebFilesConfiguration configuration, IDeserializer deserializer) : base(filePathHelper, configuration)
//     {
//         _deserializer = deserializer;
//     }
//
//     public List<INode> Finish()
//     {
//         ProcessAsync().GetAwaiter().GetResult();
//         return _deserializer.Finish().ToList();
//     }
//
//     protected override async Task ProcessAsync(Utf8JsonAsyncStreamReader jsonReader, CancellationToken cancellationToken)
//     {
//         switch (jsonReader.GetString())
//         {
//             case "serializationFormatVersion":
//                 await jsonReader.ReadAsync(cancellationToken).ConfigureAwait(false);
//                 string? version = jsonReader.GetString();
//                 Console.WriteLine($"version: {version}");
//                 break;
//             
//             case "nodes":
//                 if (BatchSize > 1)
//                     await DeserializeAsync<SerializedNode>
//                             (jsonReader, this.BatchProcessAsync, cancellationToken)
//                         .ConfigureAwait(false);
//                 else
//                     await DeserializeAsync<SerializedNode>
//                             (jsonReader, ItemProcessAsync, cancellationToken)
//                         .ConfigureAwait(false);
//                 break;
//         }
//     }
//     
//     private Task BatchProcessAsync(IEnumerable<SerializedNode?> items, CancellationToken token)
//     {
//         _deserializer.Process(items);
//         return Task.CompletedTask;
//     }
//
//     private long count = 0;
//     private Task ItemProcessAsync(SerializedNode? item, CancellationToken token)
//     {
//         _deserializer.Process([item]);
//         if (++count % 10_000 == 0)
//         {
//             Console.WriteLine(
//                 $"Creating Line #{count} privateMem: {AsFraction(Process.GetCurrentProcess().PrivateMemorySize64)} gcMem: {AsFraction(GC.GetTotalMemory(false))}");
//             
//         }
//         return Task.CompletedTask;
//
//         string AsFraction(long value1)
//         {
//             return string.Format("{0:0.000}", value1 / 1_000_000D) + "M";
//         }
//     }
// }
// public interface ILionWebConfiguration : IDataConfiguration
// {
// }
//
// public interface ILionWebFilesConfiguration : IFilesConfiguration
// {
//     
// }