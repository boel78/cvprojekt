using System;
using System.Collections.Generic;

namespace cvprojekt.Models;

public partial class Message
{
    public int Mid { get; set; }

    public int Sender { get; set; }

    public int Reciever { get; set; }

    public string? Content { get; set; }

    public bool? IsRead { get; set; }

    public DateOnly? TimeSent { get; set; }

    public virtual User RecieverNavigation { get; set; } = null!;

    public virtual User SenderNavigation { get; set; } = null!;
}
