public class BinomialRequest
{
    public double SpotPrice { get; set; }       // S - current price of underlying
    public double StrikePrice { get; set; }     // K - option strike price
    public double TimeToExpiry { get; set; }    // T - time to expiration in years
    public double RiskFreeRate { get; set; }    // r - annual risk free rate as decimal
    public double Volatility { get; set; }      // σ - annual volatility as decimal
    public int Steps { get; set; } = 100;       // N - number of time steps in the tree
}