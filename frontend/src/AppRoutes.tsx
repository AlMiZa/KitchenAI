import { Routes, Route, Navigate } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import AdminLayout from './layouts/AdminLayout';
import DashboardPage from './pages/Dashboard';
import InventoryPage from './pages/Inventory';
import RecipesPage from './pages/Recipes';
import RecipeDetailPage from './pages/RecipeDetail';
import ShoppingListPage from './pages/ShoppingList';
import SettingsPage from './pages/Settings';
import ApiKeysPage from './pages/admin/ApiKeys';
import MetricsPage from './pages/admin/Metrics';
import LoginPage from './pages/auth/Login';
import RegisterPage from './pages/auth/Register';

/** Route tree — consumed by App (BrowserRouter) and tests (MemoryRouter). */
export default function AppRoutes() {
  return (
    <Routes>
      {/* Auth pages (no layout wrapper) */}
      <Route path="login" element={<LoginPage />} />
      <Route path="register" element={<RegisterPage />} />

      {/* Main application routes */}
      <Route element={<MainLayout />}>
        <Route index element={<DashboardPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="inventory" element={<InventoryPage />} />
        <Route path="recipes" element={<RecipesPage />} />
        <Route path="recipes/:recipeId" element={<RecipeDetailPage />} />
        <Route path="shopping-list" element={<ShoppingListPage />} />
        <Route path="settings" element={<SettingsPage />} />
      </Route>

      {/* Admin routes */}
      <Route path="admin" element={<AdminLayout />}>
        <Route index element={<Navigate to="api-keys" replace />} />
        <Route path="api-keys" element={<ApiKeysPage />} />
        <Route path="metrics" element={<MetricsPage />} />
      </Route>
    </Routes>
  );
}
