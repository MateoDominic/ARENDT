using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Question
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = null!;

    public int? PictureId { get; set; }

    public int QuizId { get; set; }

    public int QuestionPosition { get; set; }

    public virtual ICollection<Answer> Answers { get; } = new List<Answer>();

    public virtual Picture? Picture { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;
}
