namespace UniJSON
{
    public class JsonSerializer
    {
        public static JsonSerializer Create()
        {
            return new JsonSerializer();
        }

        public string Serialize(int value)
        {
            return value.ToString();
        }
    }
}
