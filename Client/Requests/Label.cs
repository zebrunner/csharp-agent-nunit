namespace ZafiraIntegration.Client.Requests
{
    public class Label
    {
        public string Key { get; }
        public string Value { get; }

        public Label(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}