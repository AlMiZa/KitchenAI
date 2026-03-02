import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import i18n from '../i18n/index';
import InventoryPage from '../pages/Inventory';

// Run tests in English so selectors match
beforeAll(async () => { await i18n.changeLanguage('en'); });
afterAll(async  () => { await i18n.changeLanguage('pl'); });

// Mock auth so householdId is available
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

// Mock the items service — return items in deliberately unsorted order
jest.mock('../services/items', () => ({
  getItems: jest.fn(() =>
    Promise.resolve([
      { id: '1', householdId: 'hid1', name: 'Milk',  quantity: 1,  unit: 'L',   expiryDate: '2099-12-10' },
      { id: '2', householdId: 'hid1', name: 'Eggs',  quantity: 12, unit: 'pcs', expiryDate: '2099-12-08' },
      { id: '3', householdId: 'hid1', name: 'Bread', quantity: 1,  unit: 'pcs', expiryDate: '2099-12-05' },
    ]),
  ),
  createItem: jest.fn(() => Promise.resolve({ id: '4', householdId: 'hid1', name: 'New', quantity: 1, unit: 'pcs' })),
  updateItem: jest.fn(() => Promise.resolve({})),
  deleteItem: jest.fn(() => Promise.resolve()),
}));

function renderInventory() {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>
        <InventoryPage />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('InventoryList', () => {
  it('renders items sorted ascending by expiry date', async () => {
    renderInventory();

    // Wait for items to appear
    await screen.findByText('Bread');

    const body = document.body.textContent ?? '';
    const breadIdx = body.indexOf('Bread');
    const eggsIdx  = body.indexOf('Eggs');
    const milkIdx  = body.indexOf('Milk');

    // Bread (2099-12-05) should appear before Eggs (12-08) before Milk (12-10)
    expect(breadIdx).toBeLessThan(eggsIdx);
    expect(eggsIdx).toBeLessThan(milkIdx);
  });

  it('unit dropdown only shows metric options', async () => {
    const user = userEvent.setup();
    renderInventory();

    await screen.findByText('Bread');

    await user.click(screen.getByRole('button', { name: /add item/i }));

    const unitSelect = screen.getByLabelText(/unit/i) as HTMLSelectElement;
    const options = Array.from(unitSelect.options).map((o) => o.value);

    expect(options).toEqual(['g', 'kg', 'ml', 'L', 'pcs']);
  });

  it('fractional qty input: accepts 0.5 and ignores alphabetic input', async () => {
    const user = userEvent.setup();
    renderInventory();

    // Wait for page to be ready
    await screen.findByText('Bread');

    // Open the Add Item modal
    await user.click(screen.getByRole('button', { name: /add item/i }));

    // The quantity input has id "item-quantity" and label "Quantity"
    const qtyInput = screen.getByLabelText(/quantity/i) as HTMLInputElement;

    // Type a valid fractional quantity
    await user.type(qtyInput, '0.5');
    expect(qtyInput).toHaveValue(0.5);

    // Clear then type alphabetic chars — number input ignores them
    await user.clear(qtyInput);
    await user.type(qtyInput, 'abc');
    expect(qtyInput.value).toBe('');
  });
});

// Suppress act() warnings from React Query's internal timer calls
afterEach(() => waitFor(() => {}));

