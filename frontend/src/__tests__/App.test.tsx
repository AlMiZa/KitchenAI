import { render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import AppRoutes from '../AppRoutes';

// Initialise i18n before tests
import '../i18n/index';

/** Renders route tree with required providers at the given path. */
function renderRoutes(initialPath = '/') {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter initialEntries={[initialPath]}>
        <AppRoutes />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

describe('App routing', () => {
  it('renders Dashboard page at root path', () => {
    renderRoutes('/');
    expect(screen.getByRole('heading', { name: /pulpit|dashboard/i })).toBeInTheDocument();
  });

  it('renders Inventory page', () => {
    renderRoutes('/inventory');
    expect(screen.getByRole('heading', { name: /zapasy|inventory/i })).toBeInTheDocument();
  });

  it('renders Recipes page', () => {
    renderRoutes('/recipes');
    expect(screen.getByRole('heading', { name: /przepisy|recipes/i })).toBeInTheDocument();
  });

  it('renders Shopping List page', () => {
    renderRoutes('/shopping-list');
    expect(screen.getByRole('heading', { name: /lista zakupów|shopping list/i })).toBeInTheDocument();
  });

  it('renders Settings page', () => {
    renderRoutes('/settings');
    expect(screen.getByRole('heading', { name: /ustawienia|settings/i })).toBeInTheDocument();
  });

  it('renders Admin API Keys page', () => {
    renderRoutes('/admin/api-keys');
    expect(screen.getByRole('heading', { name: /klucze api|api keys/i })).toBeInTheDocument();
  });

  it('renders Admin Metrics page', () => {
    renderRoutes('/admin/metrics');
    expect(screen.getByRole('heading', { name: /metryki|metrics/i })).toBeInTheDocument();
  });
});
