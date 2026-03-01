import { Outlet, NavLink } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

/** Admin-specific layout with dedicated navigation. */
export default function AdminLayout() {
  const { t } = useTranslation();

  const adminLinks = [
    { to: '/admin/api-keys', label: t('pages.admin.apiKeys') },
    { to: '/admin/metrics', label: t('pages.admin.metrics') },
  ];

  return (
    <div className="min-h-screen flex flex-col bg-gray-100">
      <header className="bg-gray-900 text-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex items-center justify-between h-16">
          <span className="text-xl font-bold text-yellow-400">
            {t('common.kitchenAI')} — {t('nav.admin')}
          </span>
          <nav className="flex gap-4" aria-label="Admin navigation">
            {adminLinks.map(({ to, label }) => (
              <NavLink
                key={to}
                to={to}
                className={({ isActive }) =>
                  `text-sm font-medium px-3 py-2 rounded-md transition-colors ${
                    isActive
                      ? 'bg-yellow-500 text-gray-900'
                      : 'text-gray-300 hover:text-white hover:bg-gray-700'
                  }`
                }
              >
                {label}
              </NavLink>
            ))}
          </nav>
        </div>
      </header>

      <main className="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <Outlet />
      </main>
    </div>
  );
}
