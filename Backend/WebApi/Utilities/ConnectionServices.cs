﻿using Fleck;
using System.Text.Json;
using WebApi.DTOs;

namespace WebApi.Utilities
{
    public class PlayerScore
    {
        public int Id { get; set; }

        public QuestionDTO? Question { get; set; } = null;

        public bool Answered { get; set; } = false;
    }
    public class PlayerScoreDTO {
        public string Username { get; set; }

        public int Score { get; set; }
        public override string ToString()
        {
            return $"{Username}: {Score}";
        }
    }

    public interface IConnectionHandler
    {
        public void HandleConnection(IWebSocketConnection connection);
        public void HandleMessage(IWebSocketConnection connection, string message);
        public void HandleClose(IWebSocketConnection connection);

    }

    public class ConnectionHandler : IConnectionHandler
    {
        private readonly IDbService _dbServices;
        public ConnectionHandler(IDbService dbService)
        {
            _dbServices = dbService;
        }

        private Dictionary<string, KeyValuePair<IWebSocketConnection, Dictionary<IWebSocketConnection, PlayerScore>>> gameDictionary = new();

        public void HandleConnection(IWebSocketConnection connection)
        {
            if (connection.ConnectionInfo.Headers.TryGetValue("QuizId", out string quizId) && !connection.ConnectionInfo.Headers.ContainsKey("SessionCode"))
            {
                string newSessionCode = SessionServices.GetNewSessionCode(gameDictionary.Keys.ToList());
                connection.ConnectionInfo.Headers.Add("SessionCode", newSessionCode);
                connection.Send(newSessionCode);
                
                gameDictionary.Add(newSessionCode, new KeyValuePair<IWebSocketConnection, Dictionary<IWebSocketConnection, PlayerScore>>(
                    connection,
                    new Dictionary<IWebSocketConnection, PlayerScore>()));
            }
            else if (connection.ConnectionInfo.Headers.TryGetValue("SessionCode", out string sessionCode) && connection.ConnectionInfo.Headers.TryGetValue("Username", out string username))
            {
                if (gameDictionary.TryGetValue(sessionCode, out KeyValuePair<IWebSocketConnection, Dictionary<IWebSocketConnection, PlayerScore>> game))
                {
                    if (game.Value.Values.Count() >= 20)
                    {
                        connection.Send("Sorry, lobby full");
                        connection.Close();
                    }
                    else
                    {
                        game.Key.ConnectionInfo.Headers.TryGetValue("QuizId", out string QuizId);
                        int.TryParse(QuizId, out int recordQuizId);
                        int id = _dbServices.AddNewPlayer(new QuizRecordDTO
                        {
                            PlayerName = username,
                            QuizId = recordQuizId,
                            Score = 0,
                            SessionId = sessionCode,
                        });

                        game.Value.Add(connection, new PlayerScore
                        {
                            Id = id,
                        });
                        gameDictionary[sessionCode] = game;
                        UpdateLobby(game);
                    }
                }
                else
                {
                    connection.Send("Incorrect headers");
                    connection.Close();
                }
            }
        }

        public void HandleMessage(IWebSocketConnection connection, string message)
        {
            if (connection.ConnectionInfo.Headers.TryGetValue("SessionCode", out string sessionCode))
            {
                var game = GetGameBySessionCode(sessionCode);
                if (int.TryParse(message, out int score) && !(game.Key == connection))
                {
                    if (game.Value[connection].Question != null && game.Value[connection].Answered == false && game.Value[connection].Question.QuestionMaxPoints >= score)
                    {
                        var quizRecord = _dbServices.GetQuizRecord(game.Value[connection].Id);
                        quizRecord.Score += score;
                        _dbServices.UpdateQuizRecord(quizRecord);
                        game.Value[connection].Answered = true;
                        gameDictionary[sessionCode] = game;
                    }
                }
                else if (connection.ConnectionInfo.Headers.TryGetValue("QuizId", out string QuizId) &&
                    int.TryParse(QuizId, out int intQuizId) && (game.Key == connection) && int.TryParse(message, out int QuestionNumber))
                {
                    var question = _dbServices.GetQuizQuestion(intQuizId, QuestionNumber);
                    if (question == null)
                    {
                        connection.Send("Bad question position");
                    }
                    foreach (var con in game.Value.Keys.ToList())
                    {
                        con.Send(JsonSerializer.Serialize(question));
                        game.Value[con].Answered = false;
                        game.Value[con].Question = question;
                    }
                }
                else if (message == "leaderboard")
                {
                    connection.Send(GetLeaderboard(sessionCode));
                    Console.WriteLine(GetLeaderboard(sessionCode));
                }
                else if (message == "close")
                {
                    HandleClose(connection);
                }
                else
                {
                    connection.Send("Bad message");
                }
            }
        }

        public async void HandleClose(IWebSocketConnection connection)
        {
            connection.ConnectionInfo.Headers.TryGetValue("SessionCode", out string sessionCode);
            var game = GetGameBySessionCode(sessionCode);
            if (game.Key == connection)
            {
                if (!(GetGameBySessionCode(sessionCode).Key == null) && connection.ConnectionInfo.Headers.TryGetValue("QuizId", out string quizId))
                {
                    if (connection.IsAvailable)
                        await connection.Send(GetLeaderboard(sessionCode));
                    

                    if (gameDictionary.ContainsKey(sessionCode) && gameDictionary[sessionCode].Value.Values.Count > 0)
                    {
                        connection.ConnectionInfo.Headers.TryGetValue("UserId", out string userId);
                        int.TryParse(quizId, out int intQuizId);
                        int.TryParse(userId, out int intUserId);
                        QuizHistoryDTO quizHistoryDTO = new QuizHistoryDTO()
                        {
                            PlayedAt = DateTime.Now,
                            QuizId = intQuizId,
                            WinnerName = GetWinnerName(sessionCode),
                            WinnerId = intUserId
                        };
                        _dbServices.AddQuizHistory(quizHistoryDTO);
                        if (connection.IsAvailable)
                        {
                            await connection.Send(quizHistoryDTO.ToString());
                            await connection.Send(quizHistoryDTO.WinnerName);
                        }
                        foreach (var con in game.Value.Keys.ToList())
                        {
                            if (con.IsAvailable)
                            {
                                await con.Send(GetLeaderboard(sessionCode));
                                con.Close();
                            }
                        }
                    }
                    connection.ConnectionInfo.Headers.Remove("QuizId");
                }
            }
            else
            {
                if (!(GetGameBySessionCode(sessionCode).Key == null))
                {
                    if (connection.IsAvailable)
                        await connection.Send(GetLeaderboard(sessionCode));
                    game.Value.Remove(connection);
                    UpdateLobby(game);
                }
            }
            if (connection.IsAvailable)
            {
                connection.Close();
            }
            if (game.Equals(default(KeyValuePair<IWebSocketConnection, Dictionary<IWebSocketConnection, PlayerScore>>)) || (!game.Key.IsAvailable && game.Value.All(x => !x.Key.IsAvailable)))
            {
                gameDictionary.Remove(sessionCode);
            }
        }

        private void UpdateLobby(KeyValuePair<IWebSocketConnection, Dictionary<IWebSocketConnection, PlayerScore>> game)
        {
            List<string> players = new();
            foreach (var player in game.Value.Values)
            {
                players.Add(_dbServices.GetQuizRecord(player.Id).PlayerName);
            }
            game.Key.Send(JsonSerializer.Serialize(players));
        }

        private string GetLeaderboard(string sessionCode)
        {
            var game = GetGameBySessionCode(sessionCode);
            if (!(game.Key == null))
            {
                List<PlayerScoreDTO> leaderboard = new();
                foreach (var player in game.Value.Values) {
                    var record = _dbServices.GetQuizRecord(player.Id);
                    if (record != null)
                    {
                        leaderboard.Add(new PlayerScoreDTO
                        {
                            Score = (int)record.Score,
                            Username = record.PlayerName
                        });
                    }
                }
                leaderboard = leaderboard.OrderByDescending(o => o.Score).ToList();
                
                return JsonSerializer.Serialize(leaderboard);

            }
            throw new Exception("Nes ne stima");
        }

        private string GetWinnerName(string sessionCode)
        {
            var leaderboard = _dbServices.GetQuizRecordsBySessionCode(sessionCode).OrderByDescending(x => x.Score).ToList();
            
            return leaderboard.ElementAt(0).PlayerName;
        }

        private KeyValuePair<IWebSocketConnection, Dictionary<IWebSocketConnection, PlayerScore>> GetGameBySessionCode(string sessionCode)
        {
            gameDictionary.TryGetValue(sessionCode, out KeyValuePair<IWebSocketConnection, Dictionary<IWebSocketConnection, PlayerScore>> game);
            return game;
        }
    }
}
