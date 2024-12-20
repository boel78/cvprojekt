using System.Runtime.Serialization;

namespace cvprojekt.Models.Dto
{
    [DataContract]
    public class SkillDto
    {
        [DataMember]
        public string Name { get; set; }
    }
}
