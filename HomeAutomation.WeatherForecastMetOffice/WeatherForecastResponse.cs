// ReSharper disable All
namespace HomeAutomation.WeatherForecastMetOffice;

public class RootObject
{
    public SiteRep SiteRep { get; set; } = null!;
}

public class SiteRep
{
    public Wx Wx { get; set; } = null!;
    public DV DV { get; set; } = null!;
}

public class Wx
{
    public Param[] Param { get; set; } = null!;
}

public class Param
{
    public string name { get; set; } = null!;
    public string units { get; set; } = null!;
}

public class DV
{
    public string dataDate { get; set; } = null!;
    public string type { get; set; } = null!;
    public Location Location { get; set; } = null!;
}

public class Location
{
    public string i { get; set; } = null!;
    public string lat { get; set; } = null!;
    public string lon { get; set; } = null!;
    public string name { get; set; } = null!;
    public string country { get; set; } = null!;
    public string continent { get; set; } = null!;
    public string elevation { get; set; } = null!;
    public Period[] Period { get; set; } = null!;
}

public class Period
{
    public string type { get; set; } = null!;
    public string value { get; set; } = null!;
    public Rep[] Rep { get; set; } = null!;
}

public class Rep
{
    public string D { get; set; } = null!;
    public string F { get; set; } = null!;
    public string G { get; set; } = null!;
    public string H { get; set; } = null!;
    public string Pp { get; set; } = null!;
    public string S { get; set; } = null!;
    public string T { get; set; } = null!;
    public string V { get; set; } = null!;
    public string W { get; set; } = null!;
    public string U { get; set; } = null!;
}