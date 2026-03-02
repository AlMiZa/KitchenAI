/**
 * Formats a date string using locale-aware formatting.
 * Polish (pl): dd.MM.yyyy (e.g. 15.03.2024)
 * English (en): MM/dd/yyyy (e.g. 03/15/2024)
 * Returns '—' for invalid or empty date strings.
 */
export function formatDate(dateStr: string, language: string): string {
  const d = new Date(dateStr);
  if (!dateStr || isNaN(d.getTime())) return '—';
  const locale = language.startsWith('pl') ? 'pl-PL' : 'en-US';
  return d.toLocaleDateString(locale, {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
}

/**
 * Formats a datetime string using locale-aware formatting.
 * Returns '—' for invalid or empty date strings.
 */
export function formatDateTime(dateStr: string, language: string): string {
  const d = new Date(dateStr);
  if (!dateStr || isNaN(d.getTime())) return '—';
  const locale = language.startsWith('pl') ? 'pl-PL' : 'en-US';
  return d.toLocaleString(locale);
}
