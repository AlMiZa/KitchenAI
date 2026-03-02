import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import i18n from '../i18n/index';
import DashboardPage from '../pages/Dashboard';
import * as analyticsService from '../services/analytics';
import * as itemsService from '../services/items';

beforeAll(async () => { await i18n.changeLanguage('en'); });
afterAll(async () => { await i18n.changeLanguage('pl'); });

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

jest.mock('../components/HouseholdSelector', () => () => <div data-testid="household-selector" />);
jest.mock('../components/LanguageSwitcher', () => () => <div data-testid="language-switcher" />);

jest.mock('../services/analytics', () => ({
  getAnalytics: jest.fn(),
}));

jest.mock('../services/items', () => ({
  getItems: jest.fn(),
  createItem: jest.fn(),
}));

const mockAnalytics = {
  expiringSoonCount: 3,
  moneySaved: 12.5,
  recipesGenerated: 7,
  topIngredients: ['Milk', 'Eggs', 'Bread'],
};

const mockItems = [
  { id: '1', householdId: 'hid1', name: 'Yogurt', quantity: 1, unit: 'pcs', expiryDate: '2099-12-01' },
];

function renderDashboard() {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>
        <DashboardPage />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('Dashboard', () => {
  beforeEach(() => {
    jest.resetAllMocks();
  });

  it('shows loading skeletons while data is fetching', () => {
    (analyticsService.getAnalytics as jest.Mock).mockReturnValue(new Promise(() => {}));
    (itemsService.getItems as jest.Mock).mockReturnValue(new Promise(() => {}));

    renderDashboard();

    const skeletons = document.querySelectorAll('.animate-pulse');
    expect(skeletons.length).toBeGreaterThan(0);
  });

  it('renders analytics cards with correct data', async () => {
    (analyticsService.getAnalytics as jest.Mock).mockResolvedValue(mockAnalytics);
    (itemsService.getItems as jest.Mock).mockResolvedValue(mockItems);

    renderDashboard();

    await screen.findByText('3');
    expect(screen.getByText('$12.50')).toBeInTheDocument();
    expect(screen.getByText('7')).toBeInTheDocument();
    expect(screen.getByText('Milk')).toBeInTheDocument();
    expect(screen.getByText('Yogurt')).toBeInTheDocument();
  });

  it('shows error banner when analytics fetch fails', async () => {
    (analyticsService.getAnalytics as jest.Mock).mockRejectedValue(new Error('Network error'));
    (itemsService.getItems as jest.Mock).mockResolvedValue([]);

    renderDashboard();

    await screen.findByText('Could not load analytics data.');
    expect(screen.getByRole('button', { name: /retry/i })).toBeInTheDocument();
  });

  it('renders quick add item form and submits', async () => {
    (analyticsService.getAnalytics as jest.Mock).mockResolvedValue(mockAnalytics);
    (itemsService.getItems as jest.Mock).mockResolvedValue([]);
    (itemsService.createItem as jest.Mock).mockResolvedValue({
      id: '99', householdId: 'hid1', name: 'NewItem', quantity: 1, unit: 'pcs',
    });

    const user = userEvent.setup();
    renderDashboard();

    const input = await screen.findByPlaceholderText('Item name…');
    const addBtn = screen.getByRole('button', { name: /add item/i });

    expect(addBtn).toBeDisabled();

    await user.type(input, 'NewItem');
    expect(addBtn).not.toBeDisabled();

    await user.click(addBtn);
    await waitFor(() =>
      expect(itemsService.createItem).toHaveBeenCalledWith('hid1', {
        name: 'NewItem', quantity: 1, unit: 'pcs',
      }),
    );
  });
});

afterEach(() => waitFor(() => {}));
