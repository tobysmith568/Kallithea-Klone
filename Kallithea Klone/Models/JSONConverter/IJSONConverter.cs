namespace KallitheaKlone.Models.JSONConverter
{
    public interface IJSONConverter
    {
        T FromJson<T>(string json);
        string ToJson<T>(T model);
    }
}
