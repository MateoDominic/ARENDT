CREATE TABLE Pictures(
	ID int primary key identity(1, 1),
	Image varbinary(max) not null
);

CREATE TABLE Users(
	ID int primary key identity(1, 1),
	Username nvarchar(75) unique not null,
	PasswordHash nvarchar(256) not null,
	PasswordSalt nvarchar(256) not null,
	ProfilePictureID int foreign key references Pictures(ID),
	FirstName nvarchar(50) not null,
	Email nvarchar(50) not null,
	LastName nvarchar(50) not null,
	JoinDate date not null
);

CREATE TABLE Quizzes(
	ID int primary key identity(1, 1),
	Title nvarchar(75) unique not null,
	Description nvarchar(max) not null,
	AuthorID int foreign key references Users(ID) not null
);

CREATE TABLE Questions(
	ID int primary key identity(1, 1),
	QuestionText nvarchar(max) not null,
	PictureID int foreign key references Pictures(ID),
	QuizID int foreign key references Quizzes(ID) not null,
	QuestionPosition int not null,
	QuestionTime int not null default 30,
	QuestionMaxPoints int not null
);

CREATE TABLE Answers(
	ID int primary key identity(1, 1),
	QuestionID int foreign key references Questions(ID) not null,
	AnswerText nvarchar(150) not null,
	Correct bit not null
);

CREATE TABLE QuizHistory(
	ID int primary key identity(1, 1),
	QuizID int foreign key references Quizzes(ID) not null,
	WinnerID int foreign key references Users(ID),
	PlayedAt datetime not null,
	WinnerName nvarchar(75) not null,
	WinnerScore int not null
);