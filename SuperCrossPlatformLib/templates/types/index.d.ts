declare module "calculation-utils" {
  export namespace CalculationUtils {
    export function ConvertMinutesToHours(minutes: string): string;
    export function ConvertHoursToMinutes(hours: string): string;
    export function SafeDivide(
      firstValue: string,
      secondValue: string,
      fallbackValue?: string
    ): string;
  }
}
