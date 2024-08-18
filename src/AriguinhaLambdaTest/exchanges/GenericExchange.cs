namespace AriguinhaLambdaTest.Exchanges;

public abstract class GenericExchange
{
    public abstract string Name { get; }
    protected abstract List<string[]> SymbolMap { get; }

    protected string? ToExchangeSymbol(string canonicalSymbol)
    {
        var map = SymbolMap.Find(x => x[0] == canonicalSymbol);
        if(map != null) return map[1];
        return null;
    }

    protected string? ToCanonicalSymbol(string exchangeSymbol)
    {
        var map = SymbolMap.Find(x => x[1] == exchangeSymbol);
        if(map != null) return map[0];
        return null;
    }

    public abstract Task<string> DownloadFundingRates(string symbol, DateTime date);
    public abstract Task<string> DownloadTrades(string symbol, DateTime date);
}
