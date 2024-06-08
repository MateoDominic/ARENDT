using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public int? ProfilePictureId { get; set; }

    public string FirstName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime JoinDate { get; set; }

    public virtual Picture? ProfilePicture { get; set; }

    public virtual ICollection<QuizHistory> QuizHistories { get; } = new List<QuizHistory>();

    public virtual ICollection<Quiz> Quizzes { get; } = new List<Quiz>();
}
