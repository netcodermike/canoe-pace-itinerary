using System.Text.Json;

try
{
    if (args.Length != 1) throw new ArgumentException("Pass one JSON input file");
    using var data = JsonDocument.Parse(File.ReadAllText(args[0]));
    Console.WriteLine(JsonSerializer.Serialize(Analyze(data.RootElement)));
    return 0;
}
catch (Exception error) when (error is IOException or JsonException or FormatException
                               or ArgumentException or InvalidOperationException
                               or KeyNotFoundException or OverflowException)
{
    Console.Error.WriteLine(error.Message);
    return 2;
}

static object Analyze(JsonElement data)
{
    decimal kilometers=0,totalMinutes=0;
    var itinerary=new List<object>();
    foreach(var leg in data.GetProperty("legs").EnumerateArray())
    {
        var km=leg.GetProperty("km").GetDecimal();
        var kmh=leg.GetProperty("kmh").GetDecimal();
        var breakMinutes=leg.GetProperty("breakMinutes").GetDecimal();
        if(km<0||kmh<=0||breakMinutes<0)throw new ArgumentException("Invalid trip leg");
        var paddleMinutes=60*km/kmh;
        totalMinutes+=paddleMinutes+breakMinutes;
        kilometers+=km;
        itinerary.Add(new {km,paddleMinutes,breakMinutes,elapsedMinutes=totalMinutes});
    }
    return new {kilometers,totalMinutes,itinerary};
}
