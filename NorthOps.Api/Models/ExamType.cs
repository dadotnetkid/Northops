using NorthOps.Api.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NorthOps.Api.Models
{
    public partial class Exam
    {
        public IEnumerable<Question> MBTIExams
        {
            get
            {
                return this.Questions.Where(m => m.Choices.Count() > 0).OrderBy(m => Convert.ToInt32(m.Title.Replace("MBTI", "")));
            }
        }
        public IEnumerable<Question> RandomQuestion
        {
            get { return this.Questions.Where(m => m.Choices.Count() > 0).OrderBy(m => Guid.NewGuid()); }
        }
        public IEnumerable<Question> QuestionsList
        {
            get
            {
                return this.IsRandom == true ? this.RandomQuestion : this.MBTIExams;
            }
        }

        public string Type
        {
            get
            {
                switch ((ExamTypes)this.ExamType)
                {
                    case ExamTypes.Multiple:
                        return "Multiple Choice";
                    case ExamTypes.TrueorFalse:
                        return "True or False";
                    case ExamTypes.MBTI:
                        return "Behavioural Test";
                    case ExamTypes.Listening:
                        return "Listening Skill";
                    case ExamTypes.TypingSkills:
                        return "Typing Skills";
                    default:
                        return "";
                }
            }
        }
        public string GetExamType(int type)
        {
            switch ((ExamTypes)this.ExamType)
            {
                case ExamTypes.Multiple:
                    return "Multiple Choice";
                case ExamTypes.TrueorFalse:
                    return "True or False";
                case ExamTypes.MBTI:
                    return "Behavioural Test";
                case ExamTypes.Listening:
                    return "Listening Skill";
                case ExamTypes.TypingSkills:
                    return "Typing Skills";
                default:
                    return "";
            }
        }
    }
    public enum ExamTypes
    {
        Multiple,
        TrueorFalse,
        MBTI,
        Listening,
        TypingSkills

    }

    
}
