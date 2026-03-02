import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import i18n from '../i18n/index';
import NotificationBell from '../components/NotificationBell';

// Run tests in English so aria-labels match
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

const mockNotifications = [
  { id: '1', type: 'expiry_warning', message: 'Milk expires in 2 days', read: false, createdAt: '2024-06-01T10:00:00Z' },
  { id: '2', type: 'info',           message: 'Recipe generated',        read: false, createdAt: '2024-06-01T09:00:00Z' },
  { id: '3', type: 'info',           message: 'Already read item',       read: true,  createdAt: '2024-06-01T08:00:00Z' },
];

jest.mock('../services/notifications', () => ({
  getNotifications: jest.fn(() => Promise.resolve(mockNotifications)),
  markAllRead:      jest.fn(() => Promise.resolve()),
}));

const mockNavigate = jest.fn();
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: () => mockNavigate,
}));

function renderBell() {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>
        <NotificationBell />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('NotificationBell', () => {
  beforeEach(() => { mockNavigate.mockReset(); });

  it('badge shows unread count (2 out of 3 notifications are unread)', async () => {
    renderBell();
    // Wait for the badge to show "2"
    const badge = await screen.findByText('2');
    expect(badge).toBeInTheDocument();
  });

  it('opens panel and lists notifications when bell is clicked', async () => {
    const user = userEvent.setup();
    renderBell();

    // Wait for bell to be ready (badge appears)
    await screen.findByText('2');

    // Click bell button (aria-label="Notifications" in EN)
    await user.click(screen.getByRole('button', { name: /notifications/i }));

    // Panel appears with notification messages
    await waitFor(() => {
      expect(screen.getByText('Milk expires in 2 days')).toBeInTheDocument();
      expect(screen.getByText('Recipe generated')).toBeInTheDocument();
      expect(screen.getByText('Already read item')).toBeInTheDocument();
    });
  });

  it('clicking a notification navigates to inventory with expiringSoon filter', async () => {
    const user = userEvent.setup();
    renderBell();

    // Wait for badge and open the panel
    await screen.findByText('2');
    await user.click(screen.getByRole('button', { name: /notifications/i }));

    // Wait for notifications to appear and click the first one
    const notifButton = await screen.findByText('Milk expires in 2 days');
    await user.click(notifButton);

    expect(mockNavigate).toHaveBeenCalledWith('/inventory?expiringSoon=true');
  });
});

