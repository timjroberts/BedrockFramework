using System.Collections.Generic;

namespace Bedrock.Framework
{
    public class KafkaOptions
    {
        public string GroupId { get; set; }
        
        public List<string> Topics { get; set; }
    }
}