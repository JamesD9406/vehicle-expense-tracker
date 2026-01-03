/**
 * Formats a DateOnly string from the backend (YYYY-MM-DD) to a localized date string.
 * Avoids timezone conversion issues by parsing as local date.
 */
export const formatDateOnly = (dateString: string): string => {
  // Split and parse as local date to avoid timezone conversion
  const [year, month, day] = dateString.split('T')[0].split('-');
  return new Date(
    parseInt(year),
    parseInt(month) - 1,
    parseInt(day)
  ).toLocaleDateString();
};

/**
 * Formats a number as CAD currency.
 */
export const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-CA', {
    style: 'currency',
    currency: 'CAD',
  }).format(amount);
};
