public interface IAnalysisService
{
    Task<string> AnalyseAsync(string jobDescription);
}