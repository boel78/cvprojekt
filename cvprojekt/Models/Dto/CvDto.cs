using System.Runtime.Serialization;

namespace cvprojekt.Models.Dto
{
    [DataContract]
    public class CvDto
    {
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public List<EducationDto> Educations { get; set; }
    }
}
