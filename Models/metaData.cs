
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyApp.Models;

public class MetaData
{
    public List<string> Religions { get; set; } = new List<string>
    {
        "Hindu",
        "Muslim",
        "Christian",
        "Sikh",
        "Buddhist",
        "Jain",
        "Parsi",
        "Jewish",
        "Other"
    };

    public List<string> MaritalStatuses { get; set; } = new List<string>
    {
        "Single",
        "Divorced",
        "Widowed",
        "Separated"
    };

    public List<string> Languages { get; set; } = new List<string>
    {
        "English",
        "Spanish",
        "Hindi",
        "Telugu",
        "Tamil",
        "French",
        "Kannada",
    };
    public List<string> Countries { get; set; } = new List<string>
    {
        "USA",
        "India",
        "UK",
        "Canada",
        "Australia",
        "Germany",
        "France"
    };
    public List<string> States { get; set; } = new List<string>
    {
        "Andhra Pradesh",
        "Tamil Nadu",
        "Karnataka",
        "Maharashtra",
        "Kerala",
        "California",
        "Texas",
        "New York"
    };
    public List<string> Cities { get; set; } = new List<string>
    {
        "Los Angeles",
        "San Francisco",
        "New York",
        "Chicago",
        "Houston",
        "Bangalore",
        "Chennai",
        "Mumbai"
    };

    public List<string> Genders { get; set; } = new List<string>
    {
        "Male",
        "Female"
    };

    public List<string> star { get; set; } = new List<string>
    {
        "Ashwini",
        "Bharani",
        "Krittika",
        "Rohini",
        "Mrigashira",
        "Ardra",
        "Punarvasu",
        "Pushya",
        "Ashlesha",
        "Magha",
        "Purva Phalguni",
        "Uttara Phalguni",
        "Hasta",
        "Chitra",
        "Swati",
        "Vishakha",
        "Anuradha",
        "Jyeshtha",
        "Mula",
        "Purva Ashadha",
        "Uttara Ashadha",
        "Shravana",
        "Dhanishta",
        "Shatabhisha",
        "Purva Bhadrapada",
        "Uttara Bhadrapada",
        "Revati"
    };

    public List<string> Rasi { get; set; } = new List<string>
    {
        "Kumbha",
        "Meena",
        "Mesha",
        "Vrishabha",
        "Mithuna",
        "Karka",
        "Simha",
        "Kanya",
        "Tula",
        "Vrischika",
        "Dhanu",
        "Makara"
    };

}



public class Religions
{
    [Key]
    public int Id { get; set; }
    public string ReligionName { get; set; }
}

public class MaritalStatus
{
    [Key]
    public int Id { get; set; }
    public string MaritalStatusName { get; set; }
}

public class Language
{
    [Key]
    public int Id { get; set; }
    public string LanguageName { get; set; }
}

public class Country
{
    [Key]
    public int Id { get; set; }
    public string CountryName { get; set; }
}

public class State
{
    [Key]
    public int Id { get; set; }
    public string StateName { get; set; }
    public int CountryId { get; set; }
    [ForeignKey("CountryId")]
    public Country Country { get; set; }
}
public class City
{
    [Key]
    public int Id { get; set; }
    public string CityName { get; set; }
    public int StateId { get; set; }
    [ForeignKey("StateId")]
    public State State { get; set; }
}

public class star
{
    [Key]
    public int Id { get; set; }
    public string StarName { get; set; }
}


public class Rasi
{
    [Key]
    public int Id { get; set; }
    public string RasiName { get; set; }
}

public class Genders
{
    [Key]
    public int Id { get; set; }
    public string GenderName { get; set; }
}