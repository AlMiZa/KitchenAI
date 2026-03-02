import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import i18n from '../i18n/index';
import SettingsPage from '../pages/Settings';

// Run tests in English so labels match
beforeAll(async () => { await i18n.changeLanguage('en'); });
afterAll(async  () => { await i18n.changeLanguage('pl'); });

jest.mock('../hooks/useAuth', () => ({
  useAuth: () => ({
    user: { id: '1', displayName: 'Tester', email: 't@t.com', locale: 'en', householdId: 'hid1' },
    householdId: 'hid1',
    loading: false,
    login: jest.fn(),
    register: jest.fn(),
    logout: jest.fn(),
  }),
}));

jest.mock('../services/auth', () => ({
  updateProfile:  jest.fn(() => Promise.resolve()),
  exportData:     jest.fn(() => Promise.resolve(new Blob())),
  deleteAccount:  jest.fn(() => Promise.resolve()),
}));

jest.mock('../services/notifications', () => ({
  subscribeNotifications: jest.fn(() => Promise.resolve()),
}));

function renderSettings() {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>
        <SettingsPage />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('Notification preferences form', () => {
  it('renders threshold input and channel toggles on the Notifications tab', async () => {
    const user = userEvent.setup();
    renderSettings();

    // Click the "Notifications" tab
    await user.click(screen.getByRole('tab', { name: /notifications/i }));

    // Threshold number input should be present
    expect(screen.getByLabelText(/expiry alert/i)).toBeInTheDocument();

    // Email and push toggles (checkboxes) should be present
    expect(screen.getByLabelText(/email notifications/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/push notifications/i)).toBeInTheDocument();
  });
});
