using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace cvprojekt.Models.Dto
{
    [DataContract]
    public class UserDto
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Name { get; set; }
        
        [XmlArray("Cvs")]
        [XmlArrayItem("Cv")]
        public List<CvDto> Cvs { get; set; }
        [XmlArray("Projects")]
        [XmlArrayItem("Project")]
        public List<ProjectDto> Projects { get; set; }
        

    }
}
