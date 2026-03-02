import { test, expect } from '@playwright/test';

/**
 * E2E happy-path: Register → Add inventory item → Generate recipes → Recipe list displayed.
 *
 * All backend API calls are intercepted with page.route() so no real server is required.
 * The app runs in its default Polish locale; we use stable IDs and route patterns to avoid
 * coupling tests to translated strings where possible.
 */

const HOUSEHOLD_ID = 'hh-e2e-001';
const USER_ID      = 'user-e2e-001';
const JWT_TOKEN    = 'mock.jwt.token';

const mockUser = {
  id: USER_ID,
  email: 'e2e@example.com',
  displayName: 'E2E Tester',
  locale: 'pl',
  householdId: HOUSEHOLD_ID,
  role: 'user',
};

const mockRecipes = [
  {
    id: 'r1',
    householdId: HOUSEHOLD_ID,
    title: 'Scrambled Eggs',
    prepTime: 5,
    cookTime: 10,
    servings: 2,
    ingredients: [{ name: 'Eggs', quantity: 3, unit: 'pcs' }],
    steps: ['Beat eggs', 'Cook on low heat'],
    nutrition: { calories: 200, protein: 14, carbs: 2, fat: 15 },
    rationale: 'Quick breakfast using your eggs',
  },
  {
    id: 'r2',
    householdId: HOUSEHOLD_ID,
    title: 'Egg Omelette',
    prepTime: 5,
    cookTime: 8,
    servings: 1,
    ingredients: [{ name: 'Eggs', quantity: 2, unit: 'pcs' }],
    steps: ['Beat eggs', 'Pour into pan'],
    nutrition: { calories: 150, protein: 12, carbs: 1, fat: 11 },
    rationale: 'Uses the eggs you have in stock',
  },
];

test.describe('Happy path: Register → Add item → Generate recipes', () => {
  test.beforeEach(async ({ page }) => {
    // Mock POST /api/auth/register
    await page.route('**/api/auth/register', (route) =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          token: JWT_TOKEN,
          userId: USER_ID,
          email: mockUser.email,
          displayName: mockUser.displayName,
          householdId: HOUSEHOLD_ID,
        }),
      }),
    );

    // Mock GET /api/auth/me
    await page.route('**/api/auth/me', (route) =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockUser),
      }),
    );

    // Mock GET/POST /api/households/:id/items
    await page.route(`**/api/households/${HOUSEHOLD_ID}/items`, (route) => {
      if (route.request().method() === 'GET') {
        return route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify([]),
        });
      }
      // POST — create item
      return route.fulfill({
        status: 201,
        contentType: 'application/json',
        body: JSON.stringify({
          id: 'item-1',
          householdId: HOUSEHOLD_ID,
          name: 'Eggs',
          quantity: 12,
          unit: 'pcs',
        }),
      });
    });

    // Mock GET /api/households/:id/recipes
    await page.route(`**/api/households/${HOUSEHOLD_ID}/recipes`, (route) => {
      if (route.request().method() === 'GET') {
        return route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify([]),
        });
      }
      return route.fulfill({
        status: 201,
        contentType: 'application/json',
        body: JSON.stringify(mockRecipes[0]),
      });
    });

    // Mock POST /api/households/:id/recipes/generate
    await page.route(`**/api/households/${HOUSEHOLD_ID}/recipes/generate`, (route) =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockRecipes),
      }),
    );

    // Mock notifications (catch-all for notification endpoints)
    await page.route('**/api/households/**/notifications**', (route) =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([]),
      }),
    );

    // Mock analytics
    await page.route('**/api/households/**/analytics**', (route) =>
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          totalItems: 0,
          recipesGenerated: 0,
          moneySaved: 0,
          topIngredients: [],
        }),
      }),
    );
  });

  test('registers a new account, adds an inventory item, generates recipes, and sees recipe cards', async ({
    page,
  }) => {
    // ── Step 1: Register ──────────────────────────────────────────────────────
    await page.goto('/register');

    // Use stable HTML IDs (reg-name, reg-email, reg-password) defined in Register.tsx
    await page.locator('#reg-name').fill('E2E Tester');
    await page.locator('#reg-email').fill('e2e@example.com');
    await page.locator('#reg-password').fill('password123');

    // Submit — button text is translated; use role selector
    await page.getByRole('button', { name: /zarejestruj|sign up|register/i }).click();

    // After successful registration, should redirect to /dashboard
    await expect(page).toHaveURL(/dashboard/, { timeout: 10_000 });

    // ── Step 2: Navigate to Inventory and add an item ─────────────────────────
    // Nav link text is "Zapasy" in Polish; use partial match
    await page.getByRole('link', { name: /zapasy|inventory/i }).click();
    await expect(page).toHaveURL(/inventory/);

    // Open the "Add Item" modal — button text "Dodaj produkt" / "Add item"
    await page.getByRole('button', { name: /dodaj|add item/i }).click();

    // Fill in item details using stable IDs from Inventory.tsx
    await page.locator('#item-name').fill('Eggs');
    await page.locator('#item-quantity').fill('12');
    await page.locator('#item-unit').selectOption('pcs');

    // Save — button text "Zapisz" / "Save"
    await page.getByRole('button', { name: /zapisz|save/i }).click();

    // ── Step 3: Navigate to Recipes and generate ──────────────────────────────
    // Nav link "Przepisy" / "Recipes"
    await page.getByRole('link', { name: /przepisy|recipes/i }).click();
    await expect(page).toHaveURL(/recipes/);

    // Click the generate button — "Generuj przepisy z lodówki" / "Generate recipes from my fridge"
    await page.getByRole('button', { name: /generuj|generate recipes/i }).click();

    // ── Step 4: Assert at least 2 recipe cards are displayed ──────────────────
    await expect(page.getByText('Scrambled Eggs')).toBeVisible({ timeout: 10_000 });
    await expect(page.getByText('Egg Omelette')).toBeVisible();
  });
});
