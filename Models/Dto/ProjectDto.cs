using System.Runtime.Serialization;

namespace cvprojekt.Models.Dto
{
    [DataContract]
    public class ProjectDto
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
}
