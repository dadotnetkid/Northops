using NorthOps.Portal.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthOps.Portal.Services
{
    public class ScoringService
    {
        List<ScoringSheet> scoringSheet = new List<ScoringSheet>();
        UnitOfWork unitOfWork = new UnitOfWork();
        NorthOps.Portal.Models.NorthOpsEntities db = new Models.NorthOpsEntities();
        public List<ScoringSheet> GetAllAnswer(string UserId)
        {
            scoringSheet = new List<ScoringSheet>();
            string[] rows = new string[10];
            int currentRow = 1;
            for (int col = 1; col <= 7; col++)
            {
                for (int row = 0; row <= 9; row++)
                {
                    rows[row] = "MBTI " + currentRow;
                    currentRow = currentRow + 7;
                }
                //here put the get unitwork query 
                foreach (var res in db.Users.Where(m => m.Id == UserId).FirstOrDefault().ApplicantAnswers.Where(m => m.Question.Exam.ExamName == "MBTI" && rows.Contains(m.Question.Title)))
                {

                    scoringSheet.Add(new ScoringSheet()
                    {
                        Column = col,
                        A = res.Choice.Choice1.StartsWith("a."),
                        B = res.Choice.Choice1.StartsWith("b.")
                    });



                    //here is the row number to be queried;
                }
                currentRow = col + 1;
            }
            return scoringSheet;
        }
        public List<MbtiResult> SumColumns(string UserId)
        {
            /*List<ScoreSheet> scoreSheet = new List<ScoreSheet>();
            for (var i = 1; i <= 7; i++)
            {
                var score = new ScoreSheet()
                {
                    Column = i,
                    A = scoringSheet.Where(m => m.A == true && m.Column == i).Count() + scoringSheet.Where(m => m.A == true && m.Column == i - 1).Count(),
                    B = scoringSheet.Where(m => m.B == true && m.Column == i).Count() + scoringSheet.Where(m => m.B == true && m.Column == i - 1).Count()
                };

             ;
            }*/
            var list = GetAllAnswer(UserId);
            List<MbtiResult> mbtiResult = new List<MbtiResult>();
            //List<string> Result = new List<string>();
            //col 1

            int A = 0;
            int B = 0;
            A = list.Where(m => m.A == true && m.Column == 1).Count();
            B = list.Where(m => m.B == true && m.Column == 1).Count();

            mbtiResult.Add(new MbtiResult() { Letter = A > B ? "E" : "I", Score = A > B ? A : B });



            //col 3
            A = list.Where(m => m.A == true && m.Column == 3).Count() + list.Where(m => m.A == true && m.Column == 2).Count();
            B = list.Where(m => m.B == true && m.Column == 3).Count() + list.Where(m => m.B == true && m.Column == 2).Count();
            mbtiResult.Add(new MbtiResult()
            {
                Letter = A > B ? "S" : "N",
                Score = A > B ? A : B
            });

            //col 5
            A = list.Where(m => m.A == true && m.Column == 5).Count() + list.Where(m => m.A == true && m.Column == 4).Count();
            B = list.Where(m => m.B == true && m.Column == 5).Count() + list.Where(m => m.B == true && m.Column == 4).Count();
            mbtiResult.Add(new MbtiResult()
            {
                Letter = A > B ? "T" : "F",
                Score = A > B ? A : B
            });

            //col 7
            A = list.Where(m => m.A == true && m.Column == 7).Count() + scoringSheet.Where(m => m.A == true && m.Column == 6).Count();
            B = list.Where(m => m.B == true && m.Column == 7).Count() + scoringSheet.Where(m => m.B == true && m.Column == 6).Count();
            mbtiResult.Add(new MbtiResult()
            {
                Letter = A > B ? "J" : "P",
                Score = A > B ? A : B
            });

            //scoringSheet.Where(m => m.B == true && m.Column == 6).Count()

            return mbtiResult;
        }

        public class ScoringSheet
        {
            public int Column { get; set; }
            public bool A { get; set; }
            public bool B { get; set; }
        }
        public class ScoreSheet
        {
            public int Column { get; set; }
            public int A { get; set; }
            public int B { get; set; }
        }
        public class MbtiResult
        {
            public string Letter { get; set; }
            public int Score { get; set; }
        }

    }
}