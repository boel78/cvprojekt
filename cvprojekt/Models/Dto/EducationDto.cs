using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace cvprojekt.Models.Dto
{
    [XmlRoot("Education")]
    public class EducationDto
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [XmlArray("Skills")]
        [XmlArrayItem("Skill")]
        public List<SkillDto> Skills { get; set; }
        
    }
}
