namespace AriguinhaLambdaTest.Exchanges;
using System.Net.Http;

public class GateIo : GenericExchange
{
    public override string Name => "gate-io";
    private readonly List<string[]> symbolMap = [
        ["BTCUSDT-PERP", "BTC-USDT-SWAP"],
        ["ETHUSDT-PERP", "ETH-USDT-SWAP"],
    ];
    protected override List<string[]> SymbolMap => symbolMap;

    private static readonly HttpClient httpClient = new HttpClient();

    private async Task<string> DownloadData(string symbol, string dataType, DateTime date)
    {
        var exchangeSymbol = ToExchangeSymbol(symbol);
        var fileName = $"{exchangeSymbol}-{dataType}-{date:yyyy-MM-dd}.zip";
        var url = $"https://www.okx.com/cdn/okex/traderecords/{dataType}/daily/{date:yyyyMMdd}/{fileName}";

        var filePath = Path.Combine(Path.GetTempPath(), fileName);
        var fileBytes = await httpClient.GetByteArrayAsync(url);
        await File.WriteAllBytesAsync(filePath, fileBytes);

        return filePath;
    }

    public override Task<string> DownloadFundingRates(string symbol, DateTime date) => DownloadData(symbol, "swaprates", date);
    public override Task<string> DownloadTrades(string symbol, DateTime date) => DownloadData(symbol, "trades", date);
}
