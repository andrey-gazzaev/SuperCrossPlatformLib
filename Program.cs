
using System.Runtime.InteropServices.JavaScript;
using ProtoBuf;

string proto = Serializer.GetProto<ICalculationUtils>();

using(StreamWriter writetext = new StreamWriter("ICalculationUtils.proto"))
{
    writetext.WriteLine(proto);
}

System.Console.WriteLine("Protobuf file generated");

return;


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

/// <summary>
/// Calculation utils.
/// </summary>
public static partial class CalculationUtils
{
    private const decimal ErrorEpsilon = 0.0001m;
    private const decimal MinutesInHour = 60;

    #region Time

    /// <summary>
    /// Convert minutes to hours.
    /// </summary>
    /// <param name="minutes">Minutes.</param>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ConvertMinutesToHours([JSMarshalAs<JSType.String>] string minutes)
        => Convert.ToString(Convert.ToDecimal(minutes) / MinutesInHour);

    /// <summary>
    /// Convert hours to minutes.
    /// </summary>
    /// <param name="hours">Hours.</param>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ConvertHoursToMinutes([JSMarshalAs<JSType.String>] string hours)
        => Convert.ToString(Convert.ToDecimal(hours) * MinutesInHour);

    #endregion

    #region Primitives

    /// <summary>
    /// Safe divide the two specified values.
    /// </summary>
    /// <param name="firstValue">First value.</param>
    /// <param name="secondValue">Second value.</param>
    /// <param name="fallbackValue">Fallback value.</param>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SafeDivide(
        [JSMarshalAs<JSType.String>] string firstValue,
        [JSMarshalAs<JSType.String>] string secondValue,
        [JSMarshalAs<JSType.String>] string? fallbackValue = null)
    {
        if (Convert.ToDecimal(secondValue) == decimal.Zero)
        {
            return fallbackValue;
        }

        return Convert.ToString(decimal.Divide(Convert.ToDecimal(firstValue), Convert.ToDecimal(secondValue)));
    }

    /// <summary>
    /// Check whether the two specified values nearly equal.
    /// </summary>
    /// <param name="firstValue">First value.</param>
    /// <param name="secondValue">Second value.</param>
    public static bool NearlyEquals(
        decimal firstValue,
        decimal secondValue)
        => Math.Abs(firstValue - secondValue) < ErrorEpsilon;

    /// <summary>
    /// Check whether the two specified values nearly equal.
    /// </summary>
    /// <param name="firstValue">First value.</param>
    /// <param name="secondValue">Second value.</param>
    public static bool NearlyEquals(
        decimal? firstValue,
        decimal? secondValue)
        => firstValue.HasValue != secondValue.HasValue
            ? false
            : NearlyEquals(firstValue.GetValueOrDefault(), secondValue.GetValueOrDefault());

    /// <summary>
    /// Get percent from amount.
    /// </summary>
    /// <param name="amount">Amount.</param>
    /// <param name="cost">Cost.</param>
    public static decimal GetPercentFromAmount(decimal amount, decimal cost)
        => Convert.ToDecimal(SafeDivide(Convert.ToString(amount), Convert.ToString(cost)));

    /// <summary>
    /// Get amount from percent.
    /// </summary>
    /// <param name="percent">Percent.</param>
    /// <param name="cost">Cost.</param>
    /// <remarks>
    /// We round the amounts because that's how the financial system works.
    /// We calculate values ​​to the nearest cent. And the user only sees 2 digits after the decimal point in the user interface.
    /// Only when we round amounts, the user sees predictable values ​​when we add up multiple totals.
    /// </remarks>
    public static decimal GetAmountFromPercent(decimal percent, decimal cost)
        => decimal.Round(percent * cost, 2);

    #endregion

    #region Contingency

    /// <summary>
    /// Calculate contingency percent value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyAmount">Contingency amount.</param>
    public static decimal CalculateContingencyPercent(
        decimal directCost,
        decimal contingencyAmount)
        => Convert.ToDecimal(SafeDivide(Convert.ToString(contingencyAmount), Convert.ToString(directCost))); 

    /// <summary>
    /// Calculate contingency amount value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyPercent">Contingency percent.</param>
    public static decimal CalculateContingencyAmount(
        decimal directCost,
        decimal contingencyPercent)
        => GetAmountFromPercent(
            cost: directCost,
            percent: contingencyPercent);

    #endregion

    #region Escalation

    /// <summary>
    /// Calculate escalation percent value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyAmount">Contingency amount.</param>
    /// <param name="escalationAmount">Escalation amount.</param>
    public static decimal CalculateEscalationPercent(
        decimal directCost,
        decimal contingencyAmount,
        decimal escalationAmount)
    {
        return GetPercentFromAmount(
            amount: escalationAmount,
            cost: directCost + contingencyAmount);
    }

    /// <summary>
    /// Calculate escalation amount value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyPercent">Contingency percent.</param>
    /// <param name="escalationPercent">Escalation percent.</param>
    public static decimal CalculateEscalationAmount(
        decimal directCost,
        decimal contingencyPercent,
        decimal escalationPercent)
        => GetAmountFromPercent(
            percent: escalationPercent,
            cost: directCost + CalculateContingencyAmount(
                directCost: directCost,
                contingencyPercent: contingencyPercent));

    #endregion

    #region CompoundEscalation

    // /// <summary>
    // /// Calculate effective escalation percent.
    // /// </summary>
    // /// <param name="termOrder">Term order.</param>
    // /// <param name="compoundEscalationPercent">Compound escalation percent.</param>
    // public static decimal CalculateEffectiveEscalationPercent(int termOrder, decimal compoundEscalationPercent)
    // {
    //     return (decimal)Math.Pow((decimal)(1M + compoundEscalationPercent), termOrder) - 1M;
    // }

    #endregion

    #region Markup

    /// <summary>
    /// Calculate markup percent value.
    /// </summary>
    /// <param name="grossMarginPercent">Gross Margin percent.</param>
    public static decimal CalculateMarkupPercent(decimal grossMarginPercent)
    {
        return Convert.ToDecimal(SafeDivide(Convert.ToString(grossMarginPercent), Convert.ToString(1 - grossMarginPercent), fallbackValue: Convert.ToString(1)));
    }

    /// <summary>
    /// Calculate markup amount value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyPercent">Contingency percent.</param>
    /// <param name="aggregatedWEFSPercent">Aggregated WEFS percent.</param>
    /// <param name="markupPercent">Markup percent.</param>
    public static decimal CalculateMarkupAmount(
        decimal directCost,
        decimal contingencyPercent,
        decimal aggregatedWEFSPercent,
        decimal markupPercent)
    {
        var totalDirectCost = directCost
            + CalculateContingencyAmount(
                directCost: directCost,
                contingencyPercent: contingencyPercent)
            + CalculateAggregatedWEFSAmount(
                directCost: directCost,
                contingencyPercent: contingencyPercent,
                aggregatedWEFSPercent: aggregatedWEFSPercent);

        return GetAmountFromPercent(percent: markupPercent, cost: totalDirectCost);
    }

    #endregion

    #region Gross Margin

    /// <summary>
    /// Calculate gross margin percent value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyAmount">Contingency amount.</param>
    /// <param name="aggregatedWEFSAmount">Aggregated WEFS amount.</param>
    /// <param name="grossMarginAmount">Gross Margin amount.</param>
    public static decimal CalculateGrossMarginPercent(
        decimal directCost,
        decimal contingencyAmount,
        decimal aggregatedWEFSAmount,
        decimal grossMarginAmount)
    {
        var totalDirectCost = directCost + contingencyAmount + aggregatedWEFSAmount;

        var sellPrice = totalDirectCost + grossMarginAmount;

        return GetPercentFromAmount(cost: sellPrice, amount: grossMarginAmount);
    }

    /// <summary>
    /// Calculate gross margin amount value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyPercent">Contingency percent.</param>
    /// <param name="aggregatedWEFSPercent">Aggregated WEFS percent.</param>
    /// <param name="grossMarginPercent">Gross Margin percent.</param>
    public static decimal CalculateGrossMarginAmount(
        decimal directCost,
        decimal contingencyPercent,
        decimal aggregatedWEFSPercent,
        decimal grossMarginPercent)
    {
        var totalDirectCost = directCost
            + CalculateContingencyAmount(
                directCost: directCost,
                contingencyPercent: contingencyPercent)
            + CalculateAggregatedWEFSAmount(
                directCost: directCost,
                contingencyPercent: contingencyPercent,
                aggregatedWEFSPercent: aggregatedWEFSPercent);

        var sellPrice = SafeDivide(Convert.ToString(totalDirectCost), Convert.ToString(1 - grossMarginPercent));
        return decimal.Round(Convert.ToDecimal(sellPrice) - totalDirectCost, 2);
    }

    #endregion

    #region Material WEFS

    /// <summary>
    /// Calculate Aggregated WEFS percent.
    /// </summary>
    /// <param name="directCost">Direct cost.</param>
    /// <param name="contingencyAmount">Contingency amount.</param>
    /// <param name="aggregatedWEFSAmount">Aggregated WEFS amount.</param>
    public static decimal CalculateAggregatedWEFSPercent(
        decimal directCost,
        decimal contingencyAmount,
        decimal aggregatedWEFSAmount)
    {
        return GetPercentFromAmount(
            amount: aggregatedWEFSAmount,
            cost: directCost + contingencyAmount);
    }

    /// <summary>
    /// Calculate Aggregate WEFS amount.
    /// </summary>
    /// <param name="directCost">Direct cost.</param>
    /// <param name="contingencyPercent">Contingency percent.</param>
    /// <param name="aggregatedWEFSPercent">Aggregate WEFS percent.</param>
    public static decimal CalculateAggregatedWEFSAmount(
        decimal directCost,
        decimal contingencyPercent,
        decimal aggregatedWEFSPercent)
    {
        var contingencyAmount = CalculateContingencyAmount(
            directCost: directCost,
            contingencyPercent: contingencyPercent);

        return GetAmountFromPercent(
            percent: aggregatedWEFSPercent,
            cost: directCost + contingencyAmount);
    }

    #endregion

    #region Totals

    /// <summary>
    /// Calculate total direct cost amount.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyAmount">Contingency amount.</param>
    /// <param name="escalationAmount">Escalation amount.</param>
    /// <param name="materialWfsAmount">Material WEFS amount.</param>
    public static decimal CalculateTotalDirectCost(
        decimal directCost,
        decimal contingencyAmount,
        decimal escalationAmount,
        decimal materialWfsAmount)
    {
        return directCost
            + contingencyAmount
            + escalationAmount
            + materialWfsAmount;
    }

    /// <summary>
    /// Calculate total direct cost amount.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyAmount">Contingency amount.</param>
    /// <param name="aggregatedWefsAmount">Material WFS amount.</param>
    public static decimal CalculateTotalDirectCost(
        decimal directCost,
        decimal contingencyAmount,
        decimal aggregatedWefsAmount)
    {
        return directCost
               + contingencyAmount
               + aggregatedWefsAmount;
    }

    /// <summary>
    /// Calculate sell price amount.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyAmount">Contingency amount.</param>
    /// <param name="escalationAmount">Escalation amount.</param>
    /// <param name="materialWefsAmount">Material WEFS amount.</param>
    /// <param name="grossMarginAmount">Gross Margin amount.</param>
    public static decimal CalculateSellPrice(
        decimal directCost,
        decimal contingencyAmount,
        decimal escalationAmount,
        decimal materialWefsAmount,
        decimal grossMarginAmount)
    {
        var totalDirectCost = CalculateTotalDirectCost(
            directCost: directCost,
            contingencyAmount: contingencyAmount,
            escalationAmount: escalationAmount,
            materialWfsAmount: materialWefsAmount);

        return totalDirectCost + grossMarginAmount;
    }

    #endregion
}
