namespace Packbuilder.Options
{
    public class DbOptions
    {
        public required string ConnectionString { get; set; }
        public required string TestConnectionString { get; set; }
    }
}