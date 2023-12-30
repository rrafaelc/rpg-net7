using System.Text.Json.Serialization;

namespace rpg.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RpgClass
    {
        Knight = 1,
        Mage,
        Cleric
    }
}