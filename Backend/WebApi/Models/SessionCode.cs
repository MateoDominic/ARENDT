using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class SessionCode
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int? QuizId { get; set; }

    public virtual Quiz? Quiz { get; set; }
}
