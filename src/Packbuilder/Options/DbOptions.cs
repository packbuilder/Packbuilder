namespace Packbuilder.Options
{
    public class DbOptions
    {
        public required string ConnectionString { get; set; }
        public string TestConnectionString { get; set; } = null!;
    }
}