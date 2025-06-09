namespace Aspire.Configuration;

public interface IModule<out TConfig>
    where TConfig : class
{
    string Name { get; }
    string Type { get; }
    bool Enabled { get; }
    TConfig Config { get; }
}