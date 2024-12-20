using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace cvprojekt.Models.Dto
{
    [XmlRoot("Cv")]
    public class CvDto
    {
        [DataMember]
        public string Description { get; set; }
        [XmlArray("Educations")]
        [XmlArrayItem("Education")]
        public List<EducationDto> Educations { get; set; }
    }
}
