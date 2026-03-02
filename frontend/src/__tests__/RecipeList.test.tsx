import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import i18n from '../i18n/index';
import RecipesPage from '../pages/Recipes';
import * as recipesService from '../services/recipes';

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

const mockRecipes = [
  {
    id: 'r1',
    householdId: 'hid1',
    title: 'Pasta Carbonara',
    prepTime: 10,
    cookTime: 20,
    servings: 4,
    ingredients: [],
    steps: [],
    nutrition: { calories: 520, protein: 22, carbs: 65, fat: 18 },
    rationale: 'Uses your eggs and cheese from the fridge',
  },
  {
    id: 'r2',
    householdId: 'hid1',
    title: 'Egg Fried Rice',
    prepTime: 5,
    cookTime: 15,
    servings: 2,
    ingredients: [],
    steps: [],
    nutrition: { calories: 410, protein: 14, carbs: 52, fat: 11 },
    rationale: 'Great way to use leftover rice and eggs',
  },
];

jest.mock('../services/recipes', () => ({
  getRecipes:       jest.fn(() => Promise.resolve([])),
  generateRecipes:  jest.fn(() => Promise.resolve([])),
  saveRecipe:       jest.fn(() => Promise.resolve(null)),
}));

function renderRecipes() {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false }, mutations: { retry: false } } });
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>
        <RecipesPage />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('RecipeList', () => {
  beforeEach(() => {
    jest.mocked(recipesService.getRecipes).mockResolvedValue(mockRecipes);
    jest.mocked(recipesService.generateRecipes).mockResolvedValue([]);
    jest.mocked(recipesService.saveRecipe).mockResolvedValue(mockRecipes[0]);
  });

  it('renders ≥ 2 recipe cards each with title, nutrition summary, and rationale', async () => {
    renderRecipes();

    // Both titles appear
    expect(await screen.findByText('Pasta Carbonara')).toBeInTheDocument();
    expect(await screen.findByText('Egg Fried Rice')).toBeInTheDocument();

    // Nutrition values are present (calories)
    expect(screen.getByText('520')).toBeInTheDocument();
    expect(screen.getByText('410')).toBeInTheDocument();

    // Rationale text is in the DOM (inside <details> — always in DOM)
    expect(
      screen.getByText(/uses your eggs and cheese/i),
    ).toBeInTheDocument();
    expect(
      screen.getByText(/leftover rice/i),
    ).toBeInTheDocument();
  });

  it('"Generate recipes" button is rendered and clickable', async () => {
    renderRecipes();

    const btn = await screen.findByRole('button', { name: /generate recipes from my fridge/i });
    expect(btn).toBeInTheDocument();
    expect(btn).not.toBeDisabled();
  });

  it('shows a loading spinner while the generate mutation is pending', async () => {
    jest.mocked(recipesService.generateRecipes).mockImplementation(
      () => new Promise(() => { /* never resolves */ }),
    );

    const user = userEvent.setup();
    renderRecipes();

    const btn = await screen.findByRole('button', { name: /generate recipes from my fridge/i });
    await user.click(btn);

    await waitFor(() => {
      expect(document.querySelector('svg.animate-spin')).toBeInTheDocument();
    });

    expect(btn).toBeDisabled();
  });

  it('shows an error message with a retry button when generation fails', async () => {
    jest.mocked(recipesService.generateRecipes).mockRejectedValue(new Error('Network error'));

    const user = userEvent.setup();
    renderRecipes();

    const btn = await screen.findByRole('button', { name: /generate recipes from my fridge/i });
    await user.click(btn);

    await waitFor(() => {
      expect(
        screen.getByText(/failed to generate recipes/i),
      ).toBeInTheDocument();
    });

    expect(screen.getByRole('button', { name: /retry/i })).toBeInTheDocument();
  });

  it('rationale section is present in the DOM for each recipe (collapsible via <details>)', async () => {
    renderRecipes();

    await screen.findByText('Pasta Carbonara');

    const summaries = screen.getAllByText(/why this recipe\?/i);
    expect(summaries.length).toBeGreaterThanOrEqual(2);

    expect(
      screen.getByText(/uses your eggs and cheese/i),
    ).toBeInTheDocument();
  });
});
