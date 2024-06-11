using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Quiz
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int AuthorId { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; } = new List<Question>();

    public virtual ICollection<QuizHistory> QuizHistories { get; } = new List<QuizHistory>();
}
