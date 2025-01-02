using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cvprojekt.Models;

public partial class Message
{
    public int Mid { get; set; }
    public string? Content { get; set; }

    public bool? IsRead { get; set; }

    public DateOnly? TimeSent { get; set; }

    public string? Sender { get; set; }
    public string? Reciever { get; set; }

    public virtual User? RecieverNavigation { get; set; }

    public virtual User? SenderNavigation { get; set; }
}
