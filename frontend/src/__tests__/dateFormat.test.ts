import { formatDate } from '../utils/dateFormat';

describe('formatDate', () => {
  it('formats date as dd.MM.yyyy for Polish locale', () => {
    // 2024-03-15 in Polish: 15.03.2024
    expect(formatDate('2024-03-15', 'pl')).toBe('15.03.2024');
  });

  it('formats date as MM/dd/yyyy for English locale', () => {
    // 2024-03-15 in English (US): 03/15/2024
    expect(formatDate('2024-03-15', 'en')).toBe('03/15/2024');
  });

  it('handles pl-PL locale prefix', () => {
    expect(formatDate('2024-03-15', 'pl-PL')).toBe('15.03.2024');
  });
});
