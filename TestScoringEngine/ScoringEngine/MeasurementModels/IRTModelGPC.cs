/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ScoringEngine.MeasurementModels
{
    public class IRTModelGPC : IRTModel
    {
        public IRTModelGPC()
            : base(IRTModelFactory.Model.IRTGPC)
        {
        }

        public override IRTModel DeepCopy()
        {
            IRTModelGPC copy = (IRTModelGPC)(this.MemberwiseClone());
            return copy;
        }

        public override void SetParameterCount(int parameterCount, int scorePoints)
        {
            if (parameterCount != scorePoints+1)
                throw new Exception("ScoringEngine.MeasurementModels.IRTGPC.SetParameterCount. The GPC model must have one more parameter than score points but this item has scorePoints = " + scorePoints.ToString() + " and " + parameterCount.ToString() + " parameters");
            _parameterCount = parameterCount;
            _parameters = new double[_parameterCount];
        }

        public override double D1LnlWrtTheta(double score, double theta)
        {
            double Da = 1.7 * _parameters[0];
            double bsum = 0;
            double eSum = 0;
            double emSum = 0;
            double e;
            for (int m = 1; m < _parameterCount; m++)
            {
                bsum += theta - _parameters[m];
                e = Math.Exp(Da * bsum);
                eSum += e;
                emSum += Da * m * e;
            }
            return Da * score  - emSum / (1 + eSum);
        }

        public override double D2LnlWrtTheta(double score, double theta)
        {
            double Da = 1.7 * _parameters[0];
            double bsum = 0;
            double eSum = 0;
            double emSum = 0;
            double emmSum = 0;
            double e;
            for (int m = 1; m < _parameterCount; m++)
            {
                bsum += theta - _parameters[m];
                e = Math.Exp(Da * bsum);
                eSum += e;
                emSum += Da * m * e;
                emmSum += Da * Da * m * m * e;
            }
            return (emSum*emSum - (1+eSum)*emmSum)/Math.Pow(1+eSum,2);
        }

        public override double ComputeProbability(double score, double theta)
        {
            double Da = 1.7 * _parameters[0];
            double[] parameterSums = new double[_parameterCount];
            parameterSums[0] = 0;
            for (int i = 1; i < _parameterCount; i++)
                parameterSums[i] = parameterSums[i - 1] + _parameters[i];

            int s = Convert.ToInt32(Math.Floor(score));
            double den = 1;
            for (int i = 1; i < _parameterCount; i++)
                den += Math.Exp(Da*(i*theta - parameterSums[i]));
            return Math.Exp(Da * (score * theta - parameterSums[s])) / den;
        }

        public override double ExpectedScore(double theta)
        {
            double expectedScore = 0.0;
            for (int score = 1; score < _parameterCount; score++)
                expectedScore += score * ComputeProbability(score, theta);
            return expectedScore;
        }

        public override void RescaleParameters(double slope, double intercept)
        {
        }

        public override double GetDifficulty()
        {
            double sum = 0.0;
            for (int i = 1; i < _parameterCount; i++)
                sum += _parameters[i];
            return sum / (_parameterCount - 1);
        }

        public override double GetSlope()
        {
            return _parameters[0];
        }

        public override double Information(double theta)
        {
            double Da = 1.7 * _parameters[0];
            double[] parameterSums = new double[_parameterCount];
            parameterSums[0] = 0;
            for (int i = 1; i < _parameterCount; i++)
                parameterSums[i] = parameterSums[i - 1] + _parameters[i];
            double den = 1;
            double sum1 = 0.0;
            double sum2 = 0.0;
            for (int i = 1; i < _parameterCount; i++)
            {
                double exp = Math.Exp(Da * (i * theta - parameterSums[i]));
                den += exp;
                sum1 += i*exp;
                sum2 += i*i*exp;
            }

            sum1 = sum1/den;
            return Da * Da * (sum2/den - sum1 * sum1);
        }

        public override double GuessProbability()
        {
            return 0.0;
        }

        public override void PrintDebugInfo(string itemName, double score, double theta, string filename)
        {
            TextWriter tw = new StreamWriter(filename, true);
            tw.Write(itemName + ",IRTGPC," + score.ToString() + "," + theta.ToString() + "," + ComputeProbability(score, theta) + "," + D1LnlWrtTheta(score, theta) + "," + D2LnlWrtTheta(score, theta));
            for (int i = 0; i < _parameterCount; i++)
                tw.Write(",p" + i.ToString() + "=" + _parameters[i].ToString());
            tw.WriteLine();
            tw.Close();
        }

        public override string ToString()
        {
            return "IRTGPC";
        }
    }
}
