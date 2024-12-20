using System.Runtime.Serialization;

namespace cvprojekt.Models.Dto
{
    [DataContract]
    public class EducationDto
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public List<SkillDto> Skills { get; set; }
        
    }
}
