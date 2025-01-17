using System.Runtime.InteropServices.JavaScript;

return;

/// <summary>
/// Calculation utils.
/// </summary>
public static partial class CalculationUtils
{
    private const double ErrorEpsilon = (double)0.0001m;
    private const double MinutesInHour = 60;

    #region Time

    /// <summary>
    /// Convert minutes to hours.
    /// </summary>
    /// <param name="minutes">Minutes.</param>
    [JSExport]
    [return: JSMarshalAs<JSType.Number>]
    public static double ConvertMinutesToHours([JSMarshalAs<JSType.Number>] double minutes)
        => minutes / MinutesInHour;

    /// <summary>
    /// Convert hours to minutes.
    /// </summary>
    /// <param name="hours">Hours.</param>
    [JSExport]
    [return: JSMarshalAs<JSType.Number>]
    public static double ConvertHoursToMinutes([JSMarshalAs<JSType.Number>] double hours)
        => hours * MinutesInHour;

    #endregion

    #region Primitives

    /// <summary>
    /// Safe divide the two specified values.
    /// </summary>
    /// <param name="firstValue">First value.</param>
    /// <param name="secondValue">Second value.</param>
    /// <param name="fallbackValue">Fallback value.</param>
    [JSExport]
    [return: JSMarshalAs<JSType.Number>]
    public static double SafeDivide(
        [JSMarshalAs<JSType.Number>] double firstValue,
        [JSMarshalAs<JSType.Number>] double secondValue,
        [JSMarshalAs<JSType.Number>] double? fallbackValue = null)
    {
        if (secondValue == 0)
        {
            return fallbackValue.GetValueOrDefault();
        }

        return firstValue / secondValue;
    }

    /// <summary>
    /// Check whether the two specified values nearly equal.
    /// </summary>
    /// <param name="firstValue">First value.</param>
    /// <param name="secondValue">Second value.</param>
    public static bool NearlyEquals(
        double firstValue,
        double secondValue)
        => Math.Abs(firstValue - secondValue) < ErrorEpsilon;

    /// <summary>
    /// Check whether the two specified values nearly equal.
    /// </summary>
    /// <param name="firstValue">First value.</param>
    /// <param name="secondValue">Second value.</param>
    public static bool NearlyEquals(
        double? firstValue,
        double? secondValue)
        => firstValue.HasValue != secondValue.HasValue
            ? false
            : NearlyEquals(firstValue.GetValueOrDefault(), secondValue.GetValueOrDefault());

    /// <summary>
    /// Get percent from amount.
    /// </summary>
    /// <param name="amount">Amount.</param>
    /// <param name="cost">Cost.</param>
    public static double GetPercentFromAmount(double amount, double cost)
        => SafeDivide(amount, cost);

    /// <summary>
    /// Get amount from percent.
    /// </summary>
    /// <param name="percent">Percent.</param>
    /// <param name="cost">Cost.</param>
    /// <remarks>
    /// We round the amounts because that's how the financial system works.
    /// We calculate values ​​to the nearest cent. And the user only sees 2 digits after the double point in the user interface.
    /// Only when we round amounts, the user sees predictable values ​​when we add up multiple totals.
    /// </remarks>
    public static double GetAmountFromPercent(double percent, double cost)
        => double.Round(percent * cost, 2);

    #endregion

    #region Contingency

    /// <summary>
    /// Calculate contingency percent value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyAmount">Contingency amount.</param>
    public static double CalculateContingencyPercent(
        double directCost,
        double contingencyAmount)
        => SafeDivide(contingencyAmount, directCost);

    /// <summary>
    /// Calculate contingency amount value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyPercent">Contingency percent.</param>
    public static double CalculateContingencyAmount(
        double directCost,
        double contingencyPercent)
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
    public static double CalculateEscalationPercent(
        double directCost,
        double contingencyAmount,
        double escalationAmount)
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
    public static double CalculateEscalationAmount(
        double directCost,
        double contingencyPercent,
        double escalationPercent)
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
    // public static double CalculateEffectiveEscalationPercent(int termOrder, double compoundEscalationPercent)
    // {
    //     return (double)Math.Pow((double)(1M + compoundEscalationPercent), termOrder) - 1M;
    // }

    #endregion

    #region Markup

    /// <summary>
    /// Calculate markup percent value.
    /// </summary>
    /// <param name="grossMarginPercent">Gross Margin percent.</param>
    public static double CalculateMarkupPercent(double grossMarginPercent)
    {
        return SafeDivide(grossMarginPercent, 1 - grossMarginPercent, fallbackValue: 1);
    }

    /// <summary>
    /// Calculate markup amount value.
    /// </summary>
    /// <param name="directCost">Cost.</param>
    /// <param name="contingencyPercent">Contingency percent.</param>
    /// <param name="aggregatedWEFSPercent">Aggregated WEFS percent.</param>
    /// <param name="markupPercent">Markup percent.</param>
    public static double CalculateMarkupAmount(
        double directCost,
        double contingencyPercent,
        double aggregatedWEFSPercent,
        double markupPercent)
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
    public static double CalculateGrossMarginPercent(
        double directCost,
        double contingencyAmount,
        double aggregatedWEFSAmount,
        double grossMarginAmount)
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
    public static double CalculateGrossMarginAmount(
        double directCost,
        double contingencyPercent,
        double aggregatedWEFSPercent,
        double grossMarginPercent)
    {
        var totalDirectCost = directCost
            + CalculateContingencyAmount(
                directCost: directCost,
                contingencyPercent: contingencyPercent)
            + CalculateAggregatedWEFSAmount(
                directCost: directCost,
                contingencyPercent: contingencyPercent,
                aggregatedWEFSPercent: aggregatedWEFSPercent);

        var sellPrice = SafeDivide(totalDirectCost, 1 - grossMarginPercent);
        return double.Round(sellPrice - totalDirectCost, 2);
    }

    #endregion

    #region Material WEFS

    /// <summary>
    /// Calculate Aggregated WEFS percent.
    /// </summary>
    /// <param name="directCost">Direct cost.</param>
    /// <param name="contingencyAmount">Contingency amount.</param>
    /// <param name="aggregatedWEFSAmount">Aggregated WEFS amount.</param>
    public static double CalculateAggregatedWEFSPercent(
        double directCost,
        double contingencyAmount,
        double aggregatedWEFSAmount)
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
    public static double CalculateAggregatedWEFSAmount(
        double directCost,
        double contingencyPercent,
        double aggregatedWEFSPercent)
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
    public static double CalculateTotalDirectCost(
        double directCost,
        double contingencyAmount,
        double escalationAmount,
        double materialWfsAmount)
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
    public static double CalculateTotalDirectCost(
        double directCost,
        double contingencyAmount,
        double aggregatedWefsAmount)
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
    public static double CalculateSellPrice(
        double directCost,
        double contingencyAmount,
        double escalationAmount,
        double materialWefsAmount,
        double grossMarginAmount)
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
