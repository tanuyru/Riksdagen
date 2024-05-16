using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.ML.Models
{
    public class PropositionModel
    {
        public const int IdCol = 0;
        public const int DateCol = 1;
        public const int SummaryCol = 7;
        public const int OrganCol = 4;
        public const int GuessedOrganCol = 5;
        public const int RegeringCol = 2;
        public const int LeftRightCol = 3;
        public const int IsRightCol = 6;
        [LoadColumn(IdCol)]
        public string Id { get; set; }
        [LoadColumn(DateCol)]
        public string Date { get; set; }
        [LoadColumn(SummaryCol)]
        public string Summary { get; set; }
        [LoadColumn(OrganCol)]
        public string Organ { get; set; }
        [LoadColumn(GuessedOrganCol)]
        public string GuessedOrgan { get; set; }

        [LoadColumn(RegeringCol)]
        public string Regering { get; set; }
        [LoadColumn(LeftRightCol)]
        public string LeftRight { get; set; }
        [LoadColumn(IsRightCol), ColumnName("Label")]
        public bool IsRight { get; set; }

        
    }

    public class PropositionPrediction : PropositionModel
    {

        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Score { get; set; }
    }
}
