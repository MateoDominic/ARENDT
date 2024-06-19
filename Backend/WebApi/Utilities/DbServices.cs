using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Utilities
{
    public interface IDbService {
        public FullQuizDTO AddFullQuiz(FullQuizDTO fullQuizDTO);
        public FullQuizDTO? UpdateFullQuiz(FullQuizDTO fullQuizDTO);
        public QuestionDTO? GetQuizQuestion(int quizId, int questionNumber);
        public IEnumerable<QuizDTO> GetAllQuizzes();
        public IEnumerable<QuizDTO> GetQuizzesByAuthorId(int AuthorId);
        public FullQuizDTO? GetQuizById(int QuizId);
        public QuizDTO? DeleteQuiz(int QuizId);
        public IEnumerable<QuizHistoryDTO>? GetAllQuizHistory();
        public QuizHistoryDTO? GetQuizHistory(int id);
        public void AddQuizHistory(QuizHistoryDTO quizHistoryDTO);
        public IEnumerable<QuizHistoryDTO>? GetQuizHistoryByAuthorId(int authorId);
        public IEnumerable<QuizHistoryDTO> GetQuizHistoryByQuizId(int quizId);
        public QuizHistoryDTO? DeleteQuizHistory(int id);
        public UserDTO? GetUser(int userId);
        public int RegisterUser(UserRegisterDTO user); 
        public bool LoginUser(UserLoginDTO user);
        public UserDTO? DeleteUser(int userId);
        public bool UpdateUserPassword(UserChangePasswordDTO user);
        public bool UpdateUserInfo(UserDTO newUser);
        public QuizRecordDTO? GetQuizRecord(int id);
        public IEnumerable<QuizRecordDTO>? GetQuizRecordsBySessionCode(string sessionCode);
        public int AddNewPlayer(QuizRecordDTO quizRecordDTO);
        public void UpdateQuizRecord(QuizRecordDTO quizRecord);
    }

    public class DbServices : IDbService
    {
        PraContext _praContext;
        IMapper _mapper;
        public DbServices(PraContext praContext, IMapper mapper) { 
            _praContext = praContext;
            _mapper = mapper;
        }

        public FullQuizDTO AddFullQuiz(FullQuizDTO fullQuizDTO)
        {
            var fullQuiz = _mapper.Map<Quiz>(fullQuizDTO);
            _praContext.Quizzes.Add(fullQuiz);
            _praContext.SaveChanges();

            fullQuizDTO.QuizId = fullQuiz.Id;
            return fullQuizDTO;
        }

        public FullQuizDTO? UpdateFullQuiz(FullQuizDTO fullQuizDTO)
        {
            var existingQuiz = _praContext.Quizzes.AsNoTracking().FirstOrDefault(x => x.Id == fullQuizDTO.QuizId);
            if (existingQuiz == null)
            {
                return null;
            }
            _praContext.Entry(existingQuiz).State = EntityState.Detached;
            _praContext.Quizzes.Update(_mapper.Map<Quiz>(fullQuizDTO));
            _praContext.SaveChanges();
            return fullQuizDTO;
        }

        public QuestionDTO? GetQuizQuestion(int quizId, int questionNumber) {
            Quiz? quiz = _praContext.Quizzes.Include("Questions").Include("Questions.Answers").FirstOrDefault(x => x.Id == quizId);
            if (quiz == null)
            {
                return null;
            }
            List<Question> questions = quiz.Questions.OrderBy(x => x.QuestionPosition).ToList();
            return _mapper.Map<QuestionDTO>(questions.ElementAt(questionNumber-1));
        }

        public IEnumerable<QuizDTO> GetAllQuizzes() {
            IEnumerable<Quiz> quizzes = _praContext.Quizzes;
            List<QuizDTO> quizDTOs = new List<QuizDTO>();
            foreach (var quiz in quizzes)
            {
                quizDTOs.Add(_mapper.Map<QuizDTO>(quiz));
            }
            return quizDTOs;
        }

        public IEnumerable<QuizDTO> GetQuizzesByAuthorId(int AuthorId)
        {
            IEnumerable<Quiz> quizzes = _praContext.Quizzes.Where(x => x.AuthorId == AuthorId);
            IEnumerable<QuizDTO> quizDTOs = _mapper.Map<IEnumerable<QuizDTO>>(quizzes);
            return quizDTOs;
        }

        public FullQuizDTO? GetQuizById(int QuizId) {
            Quiz? quiz = _praContext.Quizzes.Include("Questions").Include("Questions.Answers").FirstOrDefault(x => x.Id == QuizId);

            if (quiz == null)
            {
                return null;
            }

            return _mapper.Map<FullQuizDTO>(quiz);
            
        }

        public QuizDTO? DeleteQuiz(int QuizId) {
            var existingQuiz = _praContext.Quizzes.Include("Questions").Include("Questions.Answers").FirstOrDefault(x => x.Id == QuizId);

            if (existingQuiz == null)
            {
                return null;
            }
            IEnumerable<QuizHistory> quizHistories = _praContext.QuizHistories.Where(x => x.QuizId == QuizId);
            foreach (var quizHistory in quizHistories)
            {
                _praContext.QuizHistories.Remove(quizHistory);
            }
            var deletedQuizDto = _mapper.Map<QuizDTO>(existingQuiz);
            _praContext.Quizzes.Remove(existingQuiz);
            _praContext.SaveChanges();
            return deletedQuizDto;
        }

        
        public IEnumerable<QuizHistoryDTO> GetAllQuizHistory()
        {
            return _mapper.Map<IEnumerable<QuizHistoryDTO>>(_praContext.QuizHistories);
        }

        public QuizHistoryDTO? GetQuizHistory(int id) {
            QuizHistory? history = _praContext.QuizHistories.FirstOrDefault(x => x.Id == id);
            if (history == null)
            {
                return null;
            }
            return _mapper.Map<QuizHistoryDTO>(history);
        }

        public IEnumerable<QuizHistoryDTO>? GetQuizHistoryByAuthorId(int authorId) {
            IEnumerable<QuizHistory> histories = _praContext.QuizHistories.Include("Quiz").Where(x => x.Quiz.AuthorId == authorId);
            if (histories.Count() == 0)
            {
                return null;
            }
            return _mapper.Map<IEnumerable<QuizHistoryDTO>>(histories);
        }

        public void AddQuizHistory(QuizHistoryDTO quizHistoryDTO)
        {
            QuizHistory quizHistory = _mapper.Map<QuizHistory>(quizHistoryDTO);
            if (quizHistory.WinnerId == 0)
            {
                quizHistory.WinnerId = null;
            }
            _praContext.QuizHistories.Add(quizHistory);
            _praContext.SaveChanges();
        }

        public IEnumerable<QuizHistoryDTO> GetQuizHistoryByQuizId(int quizId)
        {
            IEnumerable<QuizHistory> histories = _praContext.QuizHistories.Include("Quiz").Where(x => x.Quiz.Id == quizId);
            return _mapper.Map<IEnumerable<QuizHistoryDTO>>(histories);
        }

        public QuizHistoryDTO? DeleteQuizHistory(int id)
        {
            var quizHistory = _praContext.QuizHistories.FirstOrDefault(x => x.Id == id);
            if (quizHistory == null)
            {
                return null;
            }
            _praContext.Entry(quizHistory).State = EntityState.Detached;
            _praContext.QuizHistories.Remove(quizHistory);
            _praContext.SaveChanges();
            return _mapper.Map<QuizHistoryDTO>(quizHistory);
        }

        public UserDTO? GetUser(int userId)
        {
            var user = _praContext.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return null;
            }
            return _mapper.Map<UserDTO>(user);
        }

        public int RegisterUser(UserRegisterDTO user)
        {
            var newUser = _mapper.Map<User>(user);

            newUser.PasswordSalt = AuthUtilities.GetSalt();
            newUser.PasswordHash = AuthUtilities.GetStringSha256Hash(user.Password, newUser.PasswordSalt);
            newUser.JoinDate = DateTime.Now;

            _praContext.Users.Add(newUser);
            _praContext.SaveChanges();
            return newUser.Id;
        }

        public bool LoginUser(UserLoginDTO user)
        {
            var existingUser = _praContext.Users.FirstOrDefault(x => x.Username == user.Username);

            if (existingUser == null)
            {
                return false;
            }
            string v = AuthUtilities.GetStringSha256Hash(user.Password, existingUser.PasswordSalt);
            if (existingUser.PasswordHash != v)
            {
                return false;
            }
            return true;
        }

        public UserDTO? DeleteUser(int userId)
        {
            User? user = _praContext.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return null;
            }

            UserDTO userDto = _mapper.Map<UserDTO>(user);
            _praContext.Users.Remove(user);
            _praContext.SaveChanges();
            return userDto;
        }

        public bool UpdateUserPassword(UserChangePasswordDTO user) {
            var existingUser = _praContext.Users.FirstOrDefault(x => x.Username == user.Username);
            if (existingUser == null || AuthUtilities.GetStringSha256Hash(user.OldPassword, existingUser.PasswordSalt) != existingUser.PasswordHash)
            {
                return false;
            }

            existingUser.PasswordSalt = AuthUtilities.GetSalt();
            existingUser.PasswordHash = AuthUtilities.GetStringSha256Hash(user.NewPassword, existingUser.PasswordSalt);

            _praContext.Users.Update(existingUser);
            _praContext.SaveChanges();
            return true;
        }

        public bool UpdateUserInfo(UserDTO newUser) {
            var existingUser = _praContext.Users.FirstOrDefault(x => x.Id == newUser.Id);
            if (existingUser == null)
            {
                return false;
            }
            var updatedUser = _mapper.Map<User>(newUser);
            updatedUser.JoinDate = existingUser.JoinDate;
            updatedUser.PasswordHash = existingUser.PasswordHash;
            updatedUser.PasswordSalt = existingUser.PasswordSalt;
            _praContext.Entry(existingUser).State = EntityState.Detached;

            _praContext.Users.Update(updatedUser);
            _praContext.SaveChanges();
            return true;
        }

        public int AddNewPlayer(QuizRecordDTO quizRecordDTO) {
            quizRecordDTO.Id = 0;
            var newQuizRecord = _mapper.Map<QuizRecord>(quizRecordDTO);
            _praContext.QuizRecords.Add(newQuizRecord);
            _praContext.SaveChanges();
            return newQuizRecord.Id;
        }

        public QuizRecordDTO? GetQuizRecord(int id) {
            var quizRecord = _praContext.QuizRecords.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (quizRecord == null)
            {
                return null;
            }
            _praContext.Entry(quizRecord).State = EntityState.Detached;
            return _mapper.Map<QuizRecordDTO>(quizRecord);
        }

        public IEnumerable<QuizRecordDTO>? GetQuizRecordsBySessionCode(string sessionCode)
        {
            var quizRecords = _praContext.QuizRecords.AsNoTracking().Where(x => x.SessionId == sessionCode);
            return _mapper.Map<IEnumerable<QuizRecordDTO>>(quizRecords);
        }

        public void UpdateQuizRecord(QuizRecordDTO quizRecordDTO)
        {
            var quizRecord = _praContext.QuizRecords.FirstOrDefault(x => x.Id == quizRecordDTO.Id);
            quizRecord.Score = quizRecordDTO.Score;
            _praContext.Entry(quizRecord).State = EntityState.Detached;

            _praContext.QuizRecords.Update(quizRecord);
            _praContext.SaveChanges();
        }
    }
}
