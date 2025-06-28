using MongoDB.Bson;
using MongoDB.Driver;
using SignalRChatServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configuração MongoDB
var connectionString = builder.Configuration.GetConnectionString("MongoDb");
var client = new MongoClient(connectionString);
var database = client.GetDatabase("ChatDB");
builder.Services.AddSingleton(database);

try
{
    var collections = database.ListCollectionNames().ToList();
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao conectar ao MongoDB: {ex.Message}");
}

// Inserindo documento inicial na coleção TestCollection
var collection = database.GetCollection<BsonDocument>("TestCollection");
if (collection.CountDocuments(new BsonDocument()) == 0)
{
    collection.InsertOne(new BsonDocument("init", true));
}

// Testando conexão com MongoDB e listando bancos de dados
var testClient = new MongoClient("mongodb://localhost:27017");
var dbs = testClient.ListDatabaseNames().ToList();
Console.WriteLine(string.Join(", ", dbs));

// Configurar SignalR
builder.Services.AddSignalR();
builder.Services.AddControllers();

// Configuração CORS mais ampla para desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(host => true); // Permite qualquer origem durante desenvolvimento
    });
});

// Swagger (opcional para API)
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// Middlewares
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseCors(); // Habilita CORS apenas em desenvolvimento
}

app.UseRouting();
app.UseAuthorization();

app.MapHub<ChatHub>("/chathub");
app.MapControllers();
app.MapGet("/", () => "API SignalR Chat Server rodando.");

app.Run();


