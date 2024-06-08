using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class QuizHistory
{
    public int Id { get; set; }

    public int QuizId { get; set; }

    public int? WinnerId { get; set; }

    public DateTime PlayedAt { get; set; }

    public string WinnerName { get; set; } = null!;

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual User? Winner { get; set; }
}
