using Newtonsoft.Json;

namespace AzureChat.Models
{
    public class Person
    {
        [JsonProperty(PropertyName = "id")]
        public string Username { get; set; }
    }
}
