var builder = WebApplication.CreateBuilder(args);

// Agregar el servicio de gRPC
builder.Services.AddGrpc();

var app = builder.Build();

// Implementa el proto en el servidor
app.MapGrpcService<AudioServicer>();

app.Run();
