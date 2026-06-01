public class BinomialCalculatorService : IBinomialCalculatorService
{
    public BinomialResult Calculate(BinomialRequest request)
    {
        double S     = request.SpotPrice;
        double K     = request.StrikePrice;
        double T     = request.TimeToExpiry;
        double r     = request.RiskFreeRate;
        double sigma = request.Volatility;
        int    N     = request.Steps;

        // Time step size
        double dt = T / N;

        // CRR up/down factors
        double u = Math.Exp(sigma * Math.Sqrt(dt));
        double d = 1.0 / u;

        // Risk neutral probability
        double p    = (Math.Exp(r * dt) - d) / (u - d);
        double q    = 1.0 - p;

        // Discount factor per step
        double discount = Math.Exp(-r * dt);

        // Build terminal asset prices at step N
        // Price at node j = S * u^j * d^(N-j)
        double[] prices = new double[N + 1];
        for (int j = 0; j <= N; j++)
            prices[j] = S * Math.Pow(u, j) * Math.Pow(d, N - j);

        // --- AMERICAN OPTION TREES ---

        // Initialise payoffs at expiration
        double[] americanCall = new double[N + 1];
        double[] americanPut  = new double[N + 1];

        for (int j = 0; j <= N; j++)
        {
            americanCall[j] = Math.Max(prices[j] - K, 0);
            americanPut[j]  = Math.Max(K - prices[j], 0);
        }

        // Work backwards through the tree
        // At each node compare discounted continuation value vs immediate exercise
        for (int i = N - 1; i >= 0; i--)
        {
            for (int j = 0; j <= i; j++)
            {
                double nodePrice = S * Math.Pow(u, j) * Math.Pow(d, i - j);

                // Discounted expected value (continuation)
                double callContinuation = discount * (p * americanCall[j + 1] + q * americanCall[j]);
                double putContinuation  = discount * (p * americanPut[j + 1]  + q * americanPut[j]);

                // Intrinsic value at this node (immediate exercise)
                double callIntrinsic = Math.Max(nodePrice - K, 0);
                double putIntrinsic  = Math.Max(K - nodePrice, 0);

                // American: take the greater of continuation or exercise
                americanCall[j] = Math.Max(callContinuation, callIntrinsic);
                americanPut[j]  = Math.Max(putContinuation,  putIntrinsic);
            }
        }

        // --- EUROPEAN OPTION TREES ---
        // Same logic but NO early exercise comparison — continuation value only

        double[] europeanCall = new double[N + 1];
        double[] europeanPut  = new double[N + 1];

        for (int j = 0; j <= N; j++)
        {
            europeanCall[j] = Math.Max(prices[j] - K, 0);
            europeanPut[j]  = Math.Max(K - prices[j], 0);
        }

        for (int i = N - 1; i >= 0; i--)
        {
            for (int j = 0; j <= i; j++)
            {
                europeanCall[j] = discount * (p * europeanCall[j + 1] + q * europeanCall[j]);
                europeanPut[j]  = discount * (p * europeanPut[j + 1]  + q * europeanPut[j]);
            }
        }

        // --- DELTA from first step of American tree ---
        // Delta = (V_up - V_down) / (S_up - S_down)
        double sUp   = S * u;
        double sDown = S * d;

        // Rebuild one-step values for delta calculation
        double[] deltaCallTree = new double[N + 1];
        double[] deltaPutTree  = new double[N + 1];

        for (int j = 0; j <= N; j++)
        {
            deltaCallTree[j] = Math.Max(prices[j] - K, 0);
            deltaPutTree[j]  = Math.Max(K - prices[j], 0);
        }

        for (int i = N - 1; i >= 1; i--)
        {
            for (int j = 0; j <= i; j++)
            {
                double nodePrice     = S * Math.Pow(u, j) * Math.Pow(d, i - j);
                double callCont      = discount * (p * deltaCallTree[j + 1] + q * deltaCallTree[j]);
                double putCont       = discount * (p * deltaPutTree[j + 1]  + q * deltaPutTree[j]);
                deltaCallTree[j]     = Math.Max(callCont, Math.Max(nodePrice - K, 0));
                deltaPutTree[j]      = Math.Max(putCont,  Math.Max(K - nodePrice, 0));
            }
        }

        double callDelta = (deltaCallTree[1] - deltaCallTree[0]) / (sUp - sDown);
        double putDelta  = (deltaPutTree[1]  - deltaPutTree[0])  / (sUp - sDown);

        return new BinomialResult
        {
            AmericanCallPrice          = Round(americanCall[0]),
            AmericanPutPrice           = Round(americanPut[0]),
            EuropeanCallPrice          = Round(europeanCall[0]),
            EuropeanPutPrice           = Round(europeanPut[0]),
            EarlyExercisePremiumCall   = Round(americanCall[0] - europeanCall[0]),
            EarlyExercisePremiumPut    = Round(americanPut[0]  - europeanPut[0]),
            UpFactor                   = Round(u),
            DownFactor                 = Round(d),
            RiskNeutralProbability     = Round(p),
            TimeStepSize               = Round(dt),
            CallDelta                  = Round(callDelta),
            PutDelta                   = Round(putDelta)
        };
    }

    private double Round(double value) => Math.Round(value, 6);
}