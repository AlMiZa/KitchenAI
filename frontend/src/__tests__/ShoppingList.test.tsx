import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import i18n from '../i18n/index';
import ShoppingListPage from '../pages/ShoppingList';

beforeAll(async () => { await i18n.changeLanguage('en'); });
afterAll(async  () => { await i18n.changeLanguage('pl'); });

// Seed localStorage with items that match what RecipeDetail writes
beforeEach(() => {
  localStorage.setItem(
    'shoppingList',
    JSON.stringify(['Cheese', 'Butter']),
  );
});

afterEach(() => {
  localStorage.removeItem('shoppingList');
});

function renderShoppingList() {
  return render(
    <MemoryRouter>
      <ShoppingListPage />
    </MemoryRouter>,
  );
}

describe('ShoppingList', () => {
  it('renders missing items added from recipe detail with checkboxes', () => {
    renderShoppingList();

    expect(screen.getByText('Cheese')).toBeInTheDocument();
    expect(screen.getByText('Butter')).toBeInTheDocument();

    // Each item has a checkbox
    const checkboxes = screen.getAllByRole('checkbox');
    expect(checkboxes).toHaveLength(2);
  });

  it('allows manual addition of new items', async () => {
    const user = userEvent.setup();
    renderShoppingList();

    const input = screen.getByRole('textbox');
    await user.type(input, 'Milk');
    await user.click(screen.getByRole('button', { name: /add item/i }));

    expect(screen.getByText('Milk')).toBeInTheDocument();
    expect(screen.getAllByRole('checkbox')).toHaveLength(3);
  });

  it('marks an item as acquired when its checkbox is clicked', async () => {
    const user = userEvent.setup();
    renderShoppingList();

    const checkboxes = screen.getAllByRole('checkbox');
    expect(checkboxes[0]).not.toBeChecked();

    await user.click(checkboxes[0]);
    expect(checkboxes[0]).toBeChecked();
  });
});
