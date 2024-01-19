namespace Wam.Proxy.Configuration;

public class ServicesConfiguration
{
    public const string SectionName = "Services";

    public string GamesService { get; set; }
    public string UsersService { get; set; }
    public string ScoresService { get; set; }
}