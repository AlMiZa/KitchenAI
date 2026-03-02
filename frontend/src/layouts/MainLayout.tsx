import { Outlet, NavLink, useLocation } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import NotificationBell from '../components/NotificationBell';
import LanguageSwitcher from '../components/LanguageSwitcher';
import CookieBanner from '../components/CookieBanner';

/** Primary application layout with header navigation and footer. */
export default function MainLayout() {
  const { t } = useTranslation();
  const location = useLocation();

  const navLinks = [
    { to: '/', label: t('nav.dashboard'), end: true },
    { to: '/inventory', label: t('nav.inventory') },
    { to: '/recipes', label: t('nav.recipes') },
    { to: '/shopping-list', label: t('nav.shoppingList') },
    { to: '/settings', label: t('nav.settings') },
  ];

  // Derive a human-readable page label for the screen-reader live region.
  const currentPageLabel =
    navLinks.find(({ to, end }) =>
      end ? location.pathname === to : location.pathname.startsWith(to),
    )?.label ?? t('common.kitchenAI');

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <header className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex items-center justify-between h-16">
          <span className="text-xl font-bold text-green-600">{t('common.kitchenAI')}</span>

          <nav className="flex gap-1" aria-label="Main navigation">
            {navLinks.map(({ to, label, end }) => (
              <NavLink
                key={to}
                to={to}
                end={end}
                className={({ isActive }) =>
                  `text-sm font-medium px-3 py-2 rounded-md transition-colors ${
                    isActive
                      ? 'bg-green-100 text-green-700'
                      : 'text-gray-600 hover:text-green-600 hover:bg-gray-100'
                  }`
                }
              >
                {label}
              </NavLink>
            ))}
          </nav>

          {/* Right-side actions */}
          <div className="flex items-center gap-2">
            <LanguageSwitcher />
            <NotificationBell />
          </div>
        </div>
      </header>

      <main className="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Visually-hidden live region announces route changes to screen readers */}
        <div aria-live="polite" aria-atomic="true" className="sr-only">
          {currentPageLabel}
        </div>
        <Outlet />
      </main>

      <footer className="bg-white border-t border-gray-200 py-4 text-center text-sm text-gray-500">
        &copy; {new Date().getFullYear()} {t('common.kitchenAI')}
      </footer>

      <CookieBanner />
    </div>
  );
}
