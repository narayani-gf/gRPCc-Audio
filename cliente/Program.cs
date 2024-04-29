using Grpc.Core;
using Grpc.Net.Client;
using System.Media;
using static AudioService;

static async Task<MemoryStream> descargaStreamAsync(AudioServiceClient stub, string nombre_archivo)
{
    using var call = stub.downloadAudio(new DownloadFileRequest
    {
        Nombre = nombre_archivo
    });

   Console.WriteLine($"Recibiendo el archivo: {nombre_archivo}");
   var writeStream = new MemoryStream();
   await foreach (var message in call.ResponseStream.ReadAllAsync())
   {
        if (message.Data != null)
        {
            var bytes = message.Data.Memory;
            Console.Write(".");
            await writeStream.WriteAsync(bytes);
        }
   }

   // Se recibieron todos los datos
   Console.WriteLine("\nRecepcion de datos correcta.\n\n");
   return writeStream;
}

static void playStream(MemoryStream stream, string nombre_archivo) 
{
    if (stream != null)
    {
        Console.WriteLine($"Reproduciendo el archivo: {nombre_archivo}...\n\n");
        SoundPlayer player = new(stream);
        player.Stream?.Seek(0, SeekOrigin.Begin);
        player.Play();
    }
}

// Establece el servidor gRPC
using var channel = GrpcChannel.ForAddress("http://localhost:8080");
// Crea el canal de comunicación
AudioServiceClient stub = new(channel);

string nombre_archivo = "anyma.wav";
// Descarga el stream
MemoryStream stream = await descargaStreamAsync(stub, nombre_archivo);
// Reproduce el stream
playStream(stream, nombre_archivo);

Console.WriteLine("Presione cualquier tecla para terminar el programa...");
Console.ReadKey();
stream.Close();
Console.WriteLine("Apagando...");
channel.ShutdownAsync().Wait();