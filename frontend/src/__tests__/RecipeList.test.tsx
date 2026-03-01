import { render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import '../i18n/index';
import RecipesPage from '../pages/Recipes';

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
  getRecipes:       jest.fn(() => Promise.resolve(mockRecipes)),
  generateRecipes:  jest.fn(() => Promise.resolve([])),
  saveRecipe:       jest.fn(() => Promise.resolve(mockRecipes[0])),
}));

function renderRecipes() {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>
        <RecipesPage />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('RecipeList', () => {
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
});
