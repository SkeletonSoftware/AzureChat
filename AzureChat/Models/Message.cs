using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace AzureChat.Models
{
    public class Message
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "messageContent")]
        public string MessageContent { get; set; }

        [JsonProperty(PropertyName = "sender")]
        public string Sender { get; set; }

        [JsonProperty(PropertyName = "recipient")]
        public string Recipient { get; set; }

        [CreatedAt]
        public DateTime Date { get; set; }

        [Version]
        public string Version { get; set; }
    }
}
