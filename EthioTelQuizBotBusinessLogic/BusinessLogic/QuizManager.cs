using EthioTelQuizBotBusinessLogic.Interface;
using EthioTelQuizBotBusinessLogic.Models.DTO;
using EthioTelQuizBotBusinessLogic.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;

namespace EthioTelQuizBotBusinessLogic.BusinessLogic
{
    public class QuizManager : IQuizManager
    {
        private readonly AuthDbContext _authDbContext;
        public QuizManager(AuthDbContext dbContext)
        {
            _authDbContext = dbContext;
        }
        public async  Task<ResponseModel> AddQuestionAsync(AddQuestionDTO question)
        {
            Question questionString = new()
            {
                QuestionString = question.QuestionString,
                Point = question.Point,
                CorrectAnswer = question.CorrectAnswer,
            };
            
            ResponseModel result = null;
            try
            {
                var qnResult = await _authDbContext.Questions.AddAsync(questionString);
                
                List<Answer> ans = new();
                for(int i = 0; i < question.Answers.Count(); i ++)
                {
                    Answer answer = new()
                    {
                        Question = qnResult.Entity,
                        QuestionId = qnResult.Entity.Id,
                        Choice = question.Answers[i]
                    };
                    ans.Add(answer);
                }
                await _authDbContext.Answers.AddRangeAsync(ans);
                _authDbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                return new ResponseModel
                {
                    Result = "Could not add the question",
                    Success = false
                };
            }
            return new ResponseModel
            {
                Result = "question added",
                Success = true
            };
        }

        public async Task<ResponseModel> EditQuestionAsync(EditQuestionDTO question)
        {
            var existingQuestion = await _authDbContext.Questions
                .FirstOrDefaultAsync(q => q.Id == question.Id);

            if (existingQuestion == null)
            {
                return new ResponseModel
                {
                    Result = "Question not found",
                    Success = false
                };
            }

            existingQuestion.QuestionString = question.QuestionString;
            existingQuestion.Point = question.Point;
            var answers =  _authDbContext.Answers.Where(ans => ans.QuestionId == question.Id);

            try
            {
                 _authDbContext.Answers.RemoveRange(answers);
                var qnResult =  _authDbContext.Questions.Update(existingQuestion);
                Answer answer = new()
                {
                    Question = existingQuestion,
                    QuestionId = existingQuestion.Id
                };
                for (int i = 0; i < question.Answers.Count(); i++)
                {
                    answer.Choice = question.Answers[i];
                    await _authDbContext.Answers.AddAsync(answer);
                }
                await _authDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Result = "Could not update the question",
                    Success = false
                };
            }

            return new ResponseModel
            {
                Result = "Question updated",
                Success = true
            };
        }

        public List<Answer> GetAnswers()
        {
            return _authDbContext.Answers.ToList();
        }

        public Question? GetQuestion(int quesionId)
        {
            return _authDbContext.Questions.FirstOrDefault(qn => qn.Id == quesionId);
        }

        public List<Question> GetQuestions()
        {
            return _authDbContext.Questions.ToList();
        }

        public async Task<List<Quiz>> GetQuizes()
        {
            List<Quiz> quizzes = new List<Quiz>();
            var questions = await _authDbContext.Questions.ToListAsync();
            for (int i = 0; i < questions.Count; i++)
            {
                Quiz quiz = new Quiz();
                quiz.QuestionString = questions[i].QuestionString;
                quiz.QuestionId = questions[i].Id;
                quiz.Point = questions[i].Point;
                var result = await _authDbContext.Answers.Where(ans => ans.QuestionId == questions[i].Id).ToListAsync();
                List<String> answers = new List<string>();
                for (int j = 0; j < result.Count; j++)
                {
                    answers.Add(result[j].Choice);
                }
                quiz.Answers = answers.ToArray();
                quizzes.Add(quiz);
            }
            return quizzes;
        }

        public async Task<ResponseModel> RemoveQuestionAsync(int questionId)
        {
            var question = await _authDbContext.Questions.FirstOrDefaultAsync(q => q.Id == questionId);
            if (question == null)
            {
                return new ResponseModel
                {
                    Result = "Question not found",
                    Success = false
                };
            }
            var answers =  _authDbContext.Answers.Where(ans => ans.QuestionId == questionId);

            try
            {
                _authDbContext.Answers.RemoveRange(answers);
                _authDbContext.Questions.Remove(question);
                await _authDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Result = "Could not delete the question",
                    Success = false
                };
            }

            return new ResponseModel
            {
                Result = "Question and answers deleted",
                Success = true
            };
        }
    }
}
