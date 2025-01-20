using ProtoBuf;

string proto = Serializer.GetProto<ICalculationUtils>();

using(StreamWriter writetext = new StreamWriter("ICalculationUtils.proto"))
{
    writetext.WriteLine(proto);
}

System.Console.WriteLine("Protobuf file generated");

[ProtoContract]
public interface ICalculationUtils
{
    string ConvertMinutesToHours(string minutes);
    string ConvertHoursToMinutes(string hours);
    string SafeDivide(string firstValue, string secondValue, string? fallbackValue = null);
    bool NearlyEquals(decimal firstValue, decimal secondValue);
    bool NearlyEquals(decimal? firstValue, decimal? secondValue);
    decimal GetPercentFromAmount(decimal amount, decimal cost);
    decimal GetAmountFromPercent(decimal percent, decimal cost);
    decimal CalculateContingencyPercent(decimal directCost, decimal contingencyAmount);
    decimal CalculateContingencyAmount(decimal directCost, decimal contingencyPercent);
    decimal CalculateEscalationPercent(decimal directCost, decimal contingencyAmount, decimal escalationAmount);
    decimal CalculateEscalationAmount(decimal directCost, decimal contingencyPercent, decimal escalationPercent);
    decimal CalculateCompoundEscalationPercent(int termOrder, decimal compoundEscalationPercent);
    decimal CalculateMarkupPercent(decimal grossMarginPercent);
    decimal CalculateMarkupAmount(decimal directCost, decimal contingencyPercent, decimal aggregatedWEFSPercent, decimal markupPercent);
    decimal CalculateGrossMarginPercent(decimal directCost, decimal contingencyAmount, decimal aggregatedWEFSAmount, decimal grossMarginAmount);
    decimal CalculateGrossMarginAmount(decimal directCost, decimal contingencyPercent, decimal aggregatedWEFSPercent, decimal grossMarginPercent);
    decimal CalculateAggregatedWEFSPercent(decimal directCost, decimal contingencyAmount, decimal aggregatedWEFSAmount);
    decimal CalculateAggregatedWEFSAmount(decimal directCost, decimal contingencyPercent, decimal aggregatedWEFSPercent);
    decimal CalculateTotalDirectCost(decimal directCost, decimal contingencyAmount, decimal escalationAmount, decimal materialWfsAmount);
    decimal CalculateTotalDirectCost(decimal directCost, decimal contingencyAmount, decimal aggregatedWefsAmount);
    decimal CalculateSellPrice(decimal directCost, decimal contingencyAmount, decimal escalationAmount, decimal materialWefsAmount, decimal grossMarginAmount);
}