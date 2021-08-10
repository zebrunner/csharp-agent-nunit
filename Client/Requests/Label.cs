namespace ZebrunnerAgent.Client.Requests
{
    internal class Label
    {
        public string Key { get; }
        public string Value { get; }

        internal Label(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}