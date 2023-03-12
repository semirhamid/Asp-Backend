using EthioTelQuizBotBusinessLogic.Interface;
using EthioTelQuizBotBusinessLogic.Models.DTO;
using EthioTelQuizBotBusinessLogic.Models.Entity;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace EthioTelQuizBotBusinessLogic.BusinessLogic;

public class UpdateHandler : IUpdateHandler

{
    private readonly ITelegramBotClient botClient;
    private readonly IQuizManager quizManager;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly AuthDbContext _authDbContext;

    public UpdateHandler(ILogger<UpdateHandler> logger, AuthDbContext authDbContext, ITelegramBotClient botClient, IQuizManager quizManager)
    {
        _authDbContext = authDbContext;
        _logger = logger;
        this.botClient = botClient;
        this.quizManager = quizManager;
    }
    public async Task HandleUpdateAsync(Update update)
    {

        if (update.Type == UpdateType.Message && update.Message != null)
        {

            this.MessageHandler(update);

        }
        else if (update.CallbackQuery != null)
        {
            if (update.CallbackQuery.Data == "startQuiz")
            {
                var quizz = await quizManager.GetQuizes();

                HashSet<int> answered = new HashSet<int>();
                var answeredProblems = await _authDbContext.Problems.Where(x => x.SubscriberId == update.CallbackQuery.Message.Chat.Id).ToListAsync();
                List<Quiz> quizzes = new();

                if(answeredProblems.Count < quizz.Count) 
                {
                    if (answeredProblems != null)
                    {
                        for (int i = 0; i < answeredProblems.Count(); i++)
                        {
                            answered.Add(answeredProblems[i].QuestionId);
                        }
                        for (int i = 0; i < quizz.Count(); i++)
                        {
                            if (!answered.Contains(quizz[i].QuestionId))
                            {
                                quizzes.Add(quizz[i]);
                            }
                        }
                        await SendQuizzesAsync(update.CallbackQuery.Message.Chat.Id, quizzes);
                        var replyMarkup = new InlineKeyboardMarkup(new[]
                            {
                new[] {  InlineKeyboardButton.WithCallbackData("Next", "Next") }
            });
                        var photo = await botClient.SendTextMessageAsync(
                            update.CallbackQuery.Message.Chat.Id,
                            "Previous Total Point:  " + GetScore(update.CallbackQuery.Message.Chat.Id), replyMarkup: replyMarkup);

                    }
                    else
                    {
                        quizzes = quizz;
                        await SendQuizzesAsync(update.CallbackQuery.Message.Chat.Id, quizzes);

                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "🎉🎉🎉 Congratulations! You have successfully solved all the questions! 🎉🎉🎉\n\n👀 Click here /mystatus to see your status and check out what you've achieved so far! 👀");

                }

            }
            else if (update.CallbackQuery.Data == "Next")
            {
                var quizz = await quizManager.GetQuizes();

                HashSet<int> answered = new HashSet<int>();
                var answeredProblems = await _authDbContext.Problems.Where(x => x.SubscriberId == update.CallbackQuery.Message.Chat.Id).ToListAsync();
                List<Quiz> quizzes = new();
                if (answeredProblems != null)
                {
                    for (int i = 0; i < answeredProblems.Count(); i++)
                    {
                        answered.Add(answeredProblems[i].QuestionId);
                    }
                    for (int i = 0; i < quizz.Count(); i++)
                    {
                        if (!answered.Contains(quizz[i].QuestionId))
                        {
                            quizzes.Add(quizz[i]);
                        }
                    }

                    if( quizz.Count - answeredProblems.Count == 0)
                    {
                        var replyMarkup = new InlineKeyboardMarkup(new[]
                        {
                new[] {  InlineKeyboardButton.WithCallbackData("View Result", "ViewResult") }
            });
                        var photo = await botClient.SendTextMessageAsync(
                            update.CallbackQuery.Message.Chat.Id,
                            "No more additional question exists !! Current Score: " + GetScore(update.CallbackQuery.Message.Chat.Id), replyMarkup: replyMarkup) ;
                    
                }else if (quizz.Count - answeredProblems.Count <= 4)
                    {
                        await SendQuizzesAsync(update.CallbackQuery.Message.Chat.Id, quizzes);
                        var replyMarkup = new InlineKeyboardMarkup(new[]
                       {
                new[] {  InlineKeyboardButton.WithCallbackData("View Result", "ViewResult") }
            });
                        var photo = await botClient.SendTextMessageAsync(
                            update.CallbackQuery.Message.Chat.Id, "View Your Rank /mystatus ");
                    }
                    else
                    {
                        await SendQuizzesAsync(update.CallbackQuery.Message.Chat.Id, quizzes);
                        var replyMarkup = new InlineKeyboardMarkup(new[]
                            {
                new[] {  InlineKeyboardButton.WithCallbackData("Next", "Next") }
            });
                        var photo = await botClient.SendTextMessageAsync(
                            update.CallbackQuery.Message.Chat.Id,
                            "Previous Total Point:  " + GetScore(update.CallbackQuery.Message.Chat.Id), replyMarkup: replyMarkup);
                    }
                    
                    

                }
                



            }
            else if (update.CallbackQuery.Data == "ViewResult")
            {
                using (var stream = System.IO.File.OpenRead("StaticFiles/profile.gif"))
                {
                    var questionCount = _authDbContext.Problems.Where(p => p.SubscriberId == update.Message.Chat.Id).Count();
                    var TotalQuestion = _authDbContext.Questions.Count();
                    var globalRank = GetSubscriberRank(update.Message.Chat.Id);
                    var totalTimeTaken = GetTotalTimeTaken(update.Message.Chat.Id);
                    var referralCount = GetCountOfReferral(update.Message.Chat.Id);
                    var totalScore = GetScore(update.Message.Chat.Id);
                    var totalMoney = totalScore * 10;

                    var caption = $"\"🎉\" Congratulations on using our Telegram quiz bot! Here are your current stats: \"🎉\"\n\n" +
                                  $"🔢 Number of questions solved: {questionCount} / {TotalQuestion}\n" +
                                  $"🌎 Global rank: {globalRank}\n" +
                                  $"⏰ Total time taken: {totalTimeTaken}\n" +
                                  $"🤝 Referral invite count: {referralCount}\n" +
                                  $"💰 Total score collected: {totalScore}\n" +
                                  $"💸 Total money earned: 🇪🇹 ETB {totalMoney} \n\n" +
                                  $"📝 We'd love to hear your feedback on our bot! Please type your message below and we'll get back to you as soon as possible.\n\n" +
                                  $"🎉 Keep up the good work! 🚀";

                    var gifInputFile = new InputMedia(stream, "profile.gif");
                    var sentMessage = await botClient.SendAnimationAsync(update.Message.Chat.Id, gifInputFile, caption: caption);
                }
            }

            else if (update.CallbackQuery.Message.Text != null)
            {
                try
                {
                    int questionId = int.Parse(update.CallbackQuery.Data.Split("|||")[1]);
                    long chatId = update.CallbackQuery.Message.Chat.Id;
                    var problem = await _authDbContext.Problems.FirstOrDefaultAsync(p => p.QuestionId == questionId && p.SubscriberId == chatId);
                    if (problem.FinishingTime == null)
                    {
                        problem.FinishingTime = DateTime.UtcNow;
                        await botClient.EditMessageTextAsync(
                            chatId: chatId,
                            messageId: update.CallbackQuery.Message.MessageId,
                            text: update.CallbackQuery.Message.Text + "\n Your Answer: " + update.CallbackQuery.Data.Split("|||")[0]
                        );
                        var ques = _authDbContext.Questions.FirstOrDefault(q => q.Id == problem.QuestionId);
                        if (ques != null && ques.CorrectAnswer == update.CallbackQuery.Data.Split("|||")[0])
                        {
                            problem.Score = this.CalculateScore((DateTime)problem.FinishingTime, problem.StartingTime, problem.Point);
                        }
                        _authDbContext.SaveChanges();
                    }


                }
                catch (Exception ex)
                {
                    return;
                }


            }
            return;

        }
    }

    private async Task MessageHandler(Update update)
    {
        long userId = update.Message.Chat.Id;
        if (update.Message != null)
        {
            if (update.Message.Chat != null)
            {
                var user = _authDbContext.Subscribers.FirstOrDefault(u => u.BotUserId == userId);
                if (user == null)
                {
                    await RegisterUser(update);
                }
            }
        }

        var message = update.Message;
        if (message.Text.StartsWith("/start") && message.Text.Count() > 8)
        {
            var invitedBy = message.Text.Split(' ')[1];
            var subscriber = _authDbContext.Subscribers.FirstOrDefault(s => s.ReferralLink == invitedBy);
            if(subscriber != null)
            {
                subscriber.ShareCount += 1;
                _authDbContext.Subscribers.Update(subscriber);
                _authDbContext.SaveChanges();
            }

            var welcomeMessage = "🎉 Welcome to our Telegram quiz bot! 🎉\r\n\r\n\U0001f9e0 Get ready to put your knowledge to the test and win rewards for every correct answer! Our trivia questions cover a wide range of categories, including general knowledge, history, science, pop culture, and more.\r\n\r\n💡 To start playing, simply type \"/start\" and our bot will guide you through the quiz. You'll receive points and rewards for every correct answer, so make sure you do your best!\r\n\r\n🤔 Don't worry if you don't know the answer to every question - this is a fun and educational experience for everyone. Play at your own pace and enjoy the challenge.\r\n\r\n👉 So, are you ready to put your knowledge to the test? Let's get started and see how many rewards you can earn! 💰🏆";

            using (var stream = new FileStream("StaticFiles/image.jpeg", FileMode.Open))
            {
                var file = new InputMedia(stream, "image.jpeg");

                var replyMarkup = new InlineKeyboardMarkup(new[]
                {
            new[] { InlineKeyboardButton.WithCallbackData("start Quiz", "startQuiz"), InlineKeyboardButton.WithCallbackData("Cancel", "cancel") }
        });
                var photo = await botClient.SendPhotoAsync(
                    message.Chat.Id,
                    file,
                    caption: welcomeMessage, replyMarkup: replyMarkup);

            }
        }
        else if (message.Text == "/start")
        {
            var welcomeMessage = "🎉 Welcome to our Telegram quiz bot! 🎉\r\n\r\n\U0001f9e0 Get ready to put your knowledge to the test and win rewards for every correct answer! Our trivia questions cover a wide range of categories, including general knowledge, history, science, pop culture, and more.\r\n\r\n💡 To start playing, simply type \"/start\" and our bot will guide you through the quiz. You'll receive points and rewards for every correct answer, so make sure you do your best!\r\n\r\n🤔 Don't worry if you don't know the answer to every question - this is a fun and educational experience for everyone. Play at your own pace and enjoy the challenge.\r\n\r\n👉 So, are you ready to put your knowledge to the test? Let's get started and see how many rewards you can earn! 💰🏆";

            using (var stream = new FileStream("StaticFiles/image.jpeg", FileMode.Open))
            {
                var file = new InputMedia(stream, "image.jpeg");

                var replyMarkup = new InlineKeyboardMarkup(new[]
                {
            new[] { InlineKeyboardButton.WithCallbackData("start Quiz", "startQuiz"), InlineKeyboardButton.WithCallbackData("Cancel", "cancel") }
        });
                var photo = await botClient.SendPhotoAsync(
                    message.Chat.Id,
                    file,
                    caption: welcomeMessage, replyMarkup: replyMarkup);

            }
        }
        else if (message.Text == "/help") {

            var helpMessage = "🤔 Need help using our Telegram quiz bot? Here are some basic commands to get you started:\r\n\r\n" +
                    "🏁 /start - Start the quiz and begin answering questions.\r\n" +
                    "🏆 /score - Get your current score and see how many rewards you've earned.\r\n" +
                    "ℹ️ /help - Display this message with a list of available commands.\r\n" +
                    "❌ /cancel - Cancel the current quiz and return to the main menu.\r\n\r\n" +
                    "👉 If you have any further questions or issues, please contact our support team by sending a message to @Support.";

            await botClient.SendTextMessageAsync(message.Chat.Id, helpMessage);
        }
        else if (message.Text == "/feedback")
        {

            var feedbackMessage = "📝 We'd love to hear your feedback on our Telegram quiz bot! Please type your message below and we'll get back to you as soon as possible.";

            await botClient.SendTextMessageAsync(message.Chat.Id, feedbackMessage);
        }
        else if (update.Message.Text == "/score")
        {
            // Handle /score command
            var score = this.GetScore(update.Message.Chat.Id);
            var scoreMessage = $"🏆 Your current score is: {score}. Keep up the good work!";
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, scoreMessage);
        }
        else if (update.Message.Text == "/leaderboard")
        {
          var result = GetTop3SubscribersByScore(_authDbContext);
        var leaderboard = ConcatenateNamesAndScores(result);
        var leaderboardMessage = "🏆 Here are the Top 3 Performers:\r\n\r\n";
            if (leaderboard.Count == 3)
            {
                
                leaderboardMessage += "🥇 " + leaderboard[0] + " points\r\n";
                leaderboardMessage += "🥈 " + leaderboard[1] + " points\r\n";
                leaderboardMessage += "🥉 " + leaderboard[2] + " points\r\n";
            }else if (leaderboard.Count == 2)
            {

                leaderboardMessage += "🥇 " + leaderboard[0] + " points\r\n";
                leaderboardMessage += "🥈 " + leaderboard[1] + " points\r\n";
            }else if (leaderboard.Count == 1)
            {

                leaderboardMessage += "🥇 " + leaderboard[0] + " points\r\n";

            }
            else
            {

                leaderboardMessage += "🥇 " ;
                
            }


            using (var stream = System.IO.File.OpenRead("StaticFiles/leadership.gif"))
            {
                var feedbackMessage = "📝 We'd love to hear your feedback on our Telegram quiz bot! Please type your message below and we'll get back to you as soon as possible.";
                var gifInputFile = new InputMedia(stream, "leadership.gif");
                var sentMessage = await botClient.SendAnimationAsync(message.Chat.Id, gifInputFile, caption: leaderboardMessage);

            }
        }
        else if (update.Message.Text == "/languages")
        {
            var replyMarkup = new InlineKeyboardMarkup(new[]
                        {
                new[] {  InlineKeyboardButton.WithCallbackData("🇬🇧 English", "English") ,InlineKeyboardButton.WithCallbackData("🇪🇹 አማርኛ", "Amharic") }
            });
            

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Please choose your language:",
                replyMarkup: replyMarkup
                );
        }
        else if (update.Message.Text == "/referral")
        {
            var chatId = update.Message.Chat.Id;
            long usersId = update.Message.From.Id;

            // Append the user ID to the invite link
            var userInviteLink = "https://t.me/ethioquiz_bot" + "?start=" + chatId.ToString();
            await botClient.SendTextMessageAsync(chatId, "🎉 Congratulations! Here is your invite link: \n\n"
        + "📎 " + userInviteLink);


        }
        else if(update.Message.Text == "/mystatus")
{

            using (var stream = System.IO.File.OpenRead("StaticFiles/profile.gif"))
            {
                var questionCount = _authDbContext.Problems.Where(p => p.SubscriberId == update.Message.Chat.Id).Count();
                var TotalQuestion = _authDbContext.Questions.Count();
                var globalRank = GetSubscriberRank(update.Message.Chat.Id);
                var totalTimeTaken = GetTotalTimeTaken(update.Message.Chat.Id); 
                var referralCount = GetCountOfReferral(update.Message.Chat.Id); 
                var totalScore = GetScore(update.Message.Chat.Id); 
                var totalMoney = totalScore * 10; 

                var caption = $"\"🎉\" Congratulations on using our Telegram quiz bot! Here are your current stats: \"🎉\"\n\n" +
                              $"🔢 Number of questions solved: {questionCount} / {TotalQuestion}\n" +
                              $"🌎 Global rank: {globalRank} / {_authDbContext.Subscribers.Count()}\n" +
                              $"⏰ Total time taken: {totalTimeTaken}\n" +
                              $"🤝 Referral invite count: {referralCount}\n" +
                              $"💰 Total score collected: {totalScore}\n" +
                              $"💸 Total money earned: 🇪🇹 ETB {totalMoney} \n\n" +
                              $"📝 We'd love to hear your feedback on our bot! Please type your message below and we'll get back to you as soon as possible.\n\n" +
                              $"🎉 Keep up the good work! 🚀";

                var gifInputFile = new InputMedia(stream, "profile.gif");
                var sentMessage = await botClient.SendAnimationAsync(message.Chat.Id, gifInputFile, caption: caption);
            }
        }
        else
        {

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "🎉🙌 Thank you for your feedback! We appreciate it greatly and will use it to make our Telegram quiz bot even better. 🚀✨");
        }


    }

    private async Task RegisterUser(Update update)
    {
        
        Subscriber sub = new()
        {
            BotUserId = 111111111111,
            FirstName = "unknown",
            LastName = "unknown",
            UserName = "unknown",
            Language = "en",
            Phone = "313121231",
            ReferralLink = update.Message.Chat.Id.ToString(),
            ShareCount = 0

        };
        if (update.Message != null)
        {
            if (update.Message.Chat != null)
            {

                if (update.Message.Chat.FirstName != null) sub.FirstName = update.Message.Chat.FirstName;
                if (update.Message.Chat.LastName != null) sub.LastName = update.Message.Chat.LastName;


            }
            if (update.Message.From != null)
            {

                if (update.Message.From.LanguageCode != null) sub.Language = update.Message.From.LanguageCode;
                if (update.Message.From.Username != null) sub.UserName = update.Message.From.Username;
                if (update.Message.From.Id != null) sub.BotUserId = update.Message.From.Id;
            }

        }

        _authDbContext.Subscribers.Add(sub);
        _authDbContext.SaveChanges();
    }

    private async Task SendQuizzesAsync(long chatId, List<Quiz> quizzes)
    {
        List<Quiz> RandomQuiz = this.GetRandomQuizzes(quizzes);
        using (var stream = System.IO.File.OpenRead("StaticFiles/countdown.gif"))
        {
            var gifInputFile = new InputMedia(stream, "countdown.gif");
            var sentMessage = await botClient.SendAnimationAsync(chatId, gifInputFile);

            // Wait for the specified delay before deleting the GIF
            await Task.Delay(5000);

            // Delete the GIF
            await botClient.DeleteMessageAsync(chatId, sentMessage.MessageId);
        }
        List<Problem> problems = new();
        foreach (var quiz in RandomQuiz)
        {
            var questionMessage = $"Question : {quiz.QuestionString}\n\n";
            var answerButtons = new List<InlineKeyboardButton>();
            problems.Add(new Problem() { Point = quiz.Point, QuestionId = quiz.QuestionId, StartingTime = DateTime.UtcNow, SubscriberId = chatId, Score = 0 });
            foreach (var answer in quiz.Answers)
            {
                answerButtons.Add(InlineKeyboardButton.WithCallbackData(answer, answer + "|||" + quiz.QuestionId));
            }

            var keyboard = new InlineKeyboardMarkup(answerButtons.Select(a => new[] { a }));

            var message = await botClient.SendTextMessageAsync(chatId, questionMessage, replyMarkup: keyboard);
        }
        _authDbContext.Problems.AddRange(problems);
        _authDbContext.SaveChanges();
    }

    private double GetScore(long chatId)
    {
        var userProblems = _authDbContext.Problems.Where(prob => prob.SubscriberId == chatId);
        double sum = 0;
        foreach (var problem in userProblems)
        {
            sum += problem.Score;
        }
        return sum;
    }

    private int GetSubscriberRank(long subscriberId)
    {
        var problems = _authDbContext.Problems
            .GroupBy(p => p.SubscriberId)
            .Select(g => new {
                SubscriberId = g.Key,
                Score = g.Sum(p => p.Score)
            })
            .OrderByDescending(x => x.Score)
            .ToList();

        int rank = problems.FindIndex(p => p.SubscriberId == subscriberId) + 1;

        return rank;
    }

    private List<Quiz> GetRandomQuizzes(List<Quiz> quizzes)
    {
        Random rand = new Random();
        List<Quiz> randomQuizzes = new List<Quiz>();
        int count = quizzes.Count;
        int index;
        while (randomQuizzes.Count < 4 && count > 0)
        {
            index = rand.Next(count);
            randomQuizzes.Add(quizzes[index]);
            quizzes[index] = quizzes[--count];
        }

        return randomQuizzes;
    }

    private int CalculateScore(DateTime utc1, DateTime utc2, double totalScore)
    {
        TimeSpan diff = utc1.Subtract(utc2);
        int secondsDiff = (int)diff.TotalSeconds;

        if (secondsDiff > 60)
        {
            return 1;
        }
        else
        {
            double percentage = (double)secondsDiff / 60 * totalScore;
            return (int)Math.Round(percentage);
        }
    }

    private List<(long subscriberId, double totalScore)> GetTop3SubscribersByScore(AuthDbContext authDbContext)
    {
        var top3Subscribers = authDbContext.Problems
            .GroupBy(p => p.SubscriberId)
            .Select(g => new { SubscriberId = g.Key, TotalScore = g.Sum(p => p.Score) })
            .OrderByDescending(s => s.TotalScore)
            .Take(3)
            .ToList();

        var result = top3Subscribers.Select(s => (s.SubscriberId, s.TotalScore)).ToList();

        return result;
    }

    private List<string> ConcatenateNamesAndScores(List<(long subscriberId, double totalScore)> subscribers)
    {
        List<string> namesAndScores = new List<string>();

        foreach (var subscriber in subscribers)
        {
            var sub = _authDbContext.Subscribers.FirstOrDefault(x => x.BotUserId == subscriber.subscriberId);
            if (sub != null)
            {
                string fullName = $"{sub.FirstName} {sub.LastName}";
                string nameAndScore = $"{fullName}: Score {subscriber.totalScore.ToString("0.0")}";
                namesAndScores.Add(nameAndScore);
            }
            
        }

        return namesAndScores;
    }
    private string GetTotalTimeTaken(long subscriberId)
    {
        var problems = _authDbContext.Problems.Where(p => p.SubscriberId == subscriberId && p.FinishingTime != null);

        var totalTimeTaken = TimeSpan.FromSeconds(problems.Sum(p => (p.FinishingTime - p.StartingTime).Value.TotalSeconds));

        return $"Your total time taken is {totalTimeTaken.ToString(@"hh\:mm\:ss")}";
    }

    private int GetCountOfReferral(long subscriberId)
    {
        int result = 0;
        var subscriber =  _authDbContext.Subscribers.FirstOrDefault(s => s.BotUserId == subscriberId);
        if(subscriber != null)
        {
            result += subscriber.ShareCount;
        }
        return result;
    }


}
