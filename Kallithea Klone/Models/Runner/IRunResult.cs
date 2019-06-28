namespace KallitheaKlone.Models.Runner
{
    public interface IRunResult
    {
        //  Properties
        //  ==========

        int ExitCode { get; set; }
        string StandardOut { get; set; }
        string ErrorOut { get; set; }
    }
}