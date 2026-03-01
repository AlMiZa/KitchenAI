import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import i18n from '../i18n/index';
import RecipeDetailPage from '../pages/RecipeDetail';

// Run tests in English so selectors match
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

const mockRecipe = {
  id: 'r1',
  householdId: 'hid1',
  title: 'Test Omelette',
  prepTime: 5,
  cookTime: 10,
  servings: 2,
  ingredients: [
    { name: 'Eggs',   quantity: 3, unit: 'pcs' },
    { name: 'Cheese', quantity: 50, unit: 'g'  },
  ],
  steps: ['Beat eggs', 'Cook on low heat'],
  nutrition: { calories: 300, protein: 18, carbs: 5, fat: 22 },
  rationale: 'Simple and quick using fridge staples',
};

const checkResultWithMissing = {
  allAvailable: false,
  missingCount: 1,
  ingredients: [
    { name: 'Eggs',   quantity: 3, unit: 'pcs', status: 'available' as const },
    { name: 'Cheese', quantity: 50, unit: 'g',  status: 'missing'   as const },
  ],
};

jest.mock('../services/recipes', () => ({
  getRecipe:                jest.fn(() => Promise.resolve(mockRecipe)),
  checkRecipeAvailability:  jest.fn(() => Promise.resolve(checkResultWithMissing)),
  generateRecipes:          jest.fn(),
  getRecipes:               jest.fn(),
  saveRecipe:               jest.fn(),
}));

function renderDetail() {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter initialEntries={['/recipes/r1']}>
        <Routes>
          <Route path="/recipes/:recipeId" element={<RecipeDetailPage />} />
        </Routes>
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('RecipeView', () => {
  it('"Check against my inventory" button is visible', async () => {
    renderDetail();
    // Wait for recipe to load
    expect(await screen.findByText('Test Omelette')).toBeInTheDocument();
    // Check button present
    expect(
      screen.getByRole('button', { name: /check against my inventory/i }),
    ).toBeInTheDocument();
  });

  it('shows "Add Missing to Shopping List" CTA when check returns missing items', async () => {
    const user = userEvent.setup();
    renderDetail();

    await screen.findByText('Test Omelette');

    // Click the check button
    await user.click(
      screen.getByRole('button', { name: /check against my inventory/i }),
    );

    // The "Add missing" button should appear
    await waitFor(() => {
      expect(
        screen.getByRole('button', { name: /add missing/i }),
      ).toBeInTheDocument();
    });
  });
});

