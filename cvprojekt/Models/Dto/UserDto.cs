using System.Runtime.Serialization;

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
        
        [DataMember]
        public List<CvDto> Cvs { get; set; }
        [DataMember]
        public List<ProjectDto> Projects { get; set; }
        

    }
}
