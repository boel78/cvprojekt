using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace cvprojekt.Models;

public partial class User : IdentityUser
{
    public string? Name { get; set; }

    public bool IsPrivate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public byte[]? ProfilePicture { get; set; }

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();

    public virtual ICollection<Message> MessageRecieverNavigations { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenderNavigations { get; set; } = new List<Message>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectsNavigation { get; set; } = new List<Project>();

}
