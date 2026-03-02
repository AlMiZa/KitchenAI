import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import i18n from '../i18n/index';
import HouseholdSelector from '../components/HouseholdSelector';
import SettingsPage from '../pages/Settings';

// Run tests in English so selectors match
beforeAll(async () => { await i18n.changeLanguage('en'); });
afterAll(async () => { await i18n.changeLanguage('pl'); });

const mockSetActiveHouseholdId = jest.fn();

jest.mock('../hooks/useAuth', () => ({
  useAuth: () => ({
    user: { id: 'u1', displayName: 'Tester', email: 't@t.com', locale: 'en', householdId: 'hid1' },
    householdId: 'hid1',
    activeHouseholdId: 'hid1',
    setActiveHouseholdId: mockSetActiveHouseholdId,
    loading: false,
    login: jest.fn(),
    register: jest.fn(),
    logout: jest.fn(),
  }),
}));

jest.mock('../services/households', () => ({
  getHouseholds: jest.fn(() =>
    Promise.resolve([
      { id: 'hid1', name: 'Home Kitchen', memberCount: 2 },
      { id: 'hid2', name: 'Beach House', memberCount: 1 },
    ]),
  ),
  getHousehold: jest.fn(() =>
    Promise.resolve({ id: 'hid1', name: 'Home Kitchen', memberCount: 2 }),
  ),
  getHouseholdMembers: jest.fn(() =>
    Promise.resolve([
      { userId: 'u1', displayName: 'Alice', email: 'alice@example.com', role: 'owner' },
      { userId: 'u2', displayName: 'Bob', email: 'bob@example.com', role: 'member' },
    ]),
  ),
  getInviteLink: jest.fn(() =>
    Promise.resolve({ inviteLink: 'https://kitchenai.app/invite/abc123' }),
  ),
  leaveHousehold: jest.fn(() => Promise.resolve()),
}));

jest.mock('../services/auth', () => ({
  updateProfile: jest.fn(),
  exportData: jest.fn(),
  deleteAccount: jest.fn(),
  getMe: jest.fn(),
}));

jest.mock('../services/notifications', () => ({
  subscribeNotifications: jest.fn(),
}));

function makeQC() {
  return new QueryClient({ defaultOptions: { queries: { retry: false } } });
}

function renderSelector() {
  return render(
    <QueryClientProvider client={makeQC()}>
      <MemoryRouter>
        <HouseholdSelector />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

function renderSettings() {
  return render(
    <QueryClientProvider client={makeQC()}>
      <MemoryRouter>
        <SettingsPage />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('HouseholdSelector', () => {
  beforeEach(() => { mockSetActiveHouseholdId.mockReset(); });

  it('renders available households in a dropdown when multiple exist', async () => {
    renderSelector();
    const select = await screen.findByRole('combobox', { name: /household selector/i });
    expect(select).toBeInTheDocument();
    expect(screen.getByText('Home Kitchen')).toBeInTheDocument();
    expect(screen.getByText('Beach House')).toBeInTheDocument();
  });

  it('calls setActiveHouseholdId when a different household is selected', async () => {
    const user = userEvent.setup();
    renderSelector();
    const select = await screen.findByRole('combobox', { name: /household selector/i });
    await user.selectOptions(select, 'hid2');
    expect(mockSetActiveHouseholdId).toHaveBeenCalledWith('hid2');
  });
});

describe('Settings — Household tab', () => {
  it('displays member list with names and roles', async () => {
    renderSettings();
    await userEvent.setup().click(screen.getByRole('tab', { name: /household/i }));
    await waitFor(() => {
      expect(screen.getByText('Alice')).toBeInTheDocument();
      expect(screen.getByText('Bob')).toBeInTheDocument();
    });
    expect(screen.getByText('Owner')).toBeInTheDocument();
    expect(screen.getByText('Member')).toBeInTheDocument();
  });

  it('renders "Invite Member" button', async () => {
    renderSettings();
    await userEvent.setup().click(screen.getByRole('tab', { name: /household/i }));
    const inviteBtn = await screen.findByRole('button', { name: /invite member/i });
    expect(inviteBtn).toBeInTheDocument();
  });

  it('shows invite link dialog when "Invite Member" is clicked', async () => {
    const user = userEvent.setup();
    renderSettings();
    await user.click(screen.getByRole('tab', { name: /household/i }));
    const inviteBtn = await screen.findByRole('button', { name: /invite member/i });
    await user.click(inviteBtn);
    await waitFor(() => {
      expect(
        screen.getByDisplayValue('https://kitchenai.app/invite/abc123'),
      ).toBeInTheDocument();
    });
  });
});
